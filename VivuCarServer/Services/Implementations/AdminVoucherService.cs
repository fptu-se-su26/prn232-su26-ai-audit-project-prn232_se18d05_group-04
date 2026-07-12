using BusinessObjects.Enums;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using Services.Interfaces;
using Services.Models.Admin;
using System.Text.RegularExpressions;

namespace Services.Implementations;

public partial class AdminVoucherService(IAdminVoucherRepository repository) : IAdminVoucherService
{
    public async Task<PagedResult<AdminVoucherResponse>> GetListAsync(AdminVoucherListQuery request, CancellationToken cancellationToken = default)
    {
        var page = Math.Max(1, request.Page);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);
        var now = DateTime.UtcNow;
        var query = repository.Query();

        if (!string.IsNullOrWhiteSpace(request.Keyword))
        {
            var keyword = request.Keyword.Trim();
            query = query.Where(voucher => voucher.Code.Contains(keyword) || voucher.Name.Contains(keyword));
        }
        if (TryParseType(request.DiscountType, out var type)) query = query.Where(voucher => voucher.DiscountType == type);
        query = request.Status?.Trim().ToLowerInvariant() switch
        {
            "expired" => query.Where(voucher => voucher.ExpiresAt.HasValue && voucher.ExpiresAt < now),
            "used_up" => query.Where(voucher => (!voucher.ExpiresAt.HasValue || voucher.ExpiresAt >= now) && voucher.BookingVouchers.Count >= voucher.Quantity),
            "active" => query.Where(voucher => (!voucher.ExpiresAt.HasValue || voucher.ExpiresAt >= now) && voucher.BookingVouchers.Count < voucher.Quantity),
            _ => query
        };

        var total = await query.CountAsync(cancellationToken);
        var vouchers = await query.OrderByDescending(voucher => voucher.CreatedAt)
            .ThenBy(voucher => voucher.Code)
            .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
        var items = vouchers.Select(Map).ToList();
        return new PagedResult<AdminVoucherResponse>(items, page, pageSize, total, (int)Math.Ceiling(total / (double)pageSize));
    }

    public async Task<AdminVoucherResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var voucher = await repository.GetByIdAsync(id, true, cancellationToken);
        return voucher is null ? null : Map(voucher);
    }

    public async Task<AdminVoucherResponse> CreateAsync(AdminVoucherUpsertRequest request, CancellationToken cancellationToken = default)
    {
        var type = await ValidateAsync(request, null, cancellationToken);
        var voucher = new Voucher { CreatedAt = DateTime.UtcNow };
        Apply(voucher, request, type);
        await repository.AddAsync(voucher, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        return Map(voucher);
    }

    public async Task<AdminVoucherResponse?> UpdateAsync(int id, AdminVoucherUpsertRequest request, CancellationToken cancellationToken = default)
    {
        var voucher = await repository.GetByIdAsync(id, true, cancellationToken);
        if (voucher is null) return null;
        var type = await ValidateAsync(request, id, cancellationToken);
        if (request.Quantity < voucher.BookingVouchers.Count)
            throw Error("quantity", "Quantity cannot be lower than the usage count.");
        Apply(voucher, request, type);
        await repository.SaveChangesAsync(cancellationToken);
        return Map(voucher);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var voucher = await repository.GetByIdAsync(id, true, cancellationToken);
        if (voucher is null) return false;
        if (voucher.BookingVouchers.Count > 0)
            throw new AdminVoucherServiceException(409, "A used voucher cannot be deleted.");
        repository.Remove(voucher);
        await repository.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<AdminVoucherPerformanceResponse?> GetPerformanceAsync(int id, CancellationToken cancellationToken = default)
    {
        var voucher = await repository.GetByIdAsync(id, true, cancellationToken);
        if (voucher is null) return null;
        var usages = voucher.BookingVouchers.OrderByDescending(usage => usage.AppliedAt).ToList();
        var rate = voucher.Quantity == 0 ? 100m : Math.Min(100m, Math.Round(usages.Count * 100m / voucher.Quantity, 1));
        var daily = usages.GroupBy(usage => usage.AppliedAt.Date).OrderBy(group => group.Key)
            .Select(group => new AdminVoucherDailyUsage(group.Key, group.Count(), group.Sum(usage => usage.DiscountAmount))).ToList();
        var recent = usages.Take(10).Select(usage => new AdminVoucherRecentUsage(
            usage.BookingId, usage.Booking.BookingCode, usage.Booking.Customer.FullName,
            usage.DiscountAmount, usage.AppliedAt)).ToList();
        return new AdminVoucherPerformanceResponse(Map(voucher), rate,
            usages.Sum(usage => usage.Booking.TotalAmount), usages.Sum(usage => usage.DiscountAmount), daily, recent);
    }

    private async Task<DiscountType> ValidateAsync(AdminVoucherUpsertRequest request, int? excludeId, CancellationToken cancellationToken)
    {
        var errors = new Dictionary<string, string[]>();
        var name = request.Name.Trim();
        var code = request.Code.Trim().ToUpperInvariant();
        if (string.IsNullOrWhiteSpace(name) || name.Length > 100) errors["name"] = ["Name must contain 1 to 100 characters."];
        if (string.IsNullOrWhiteSpace(code) || code.Length > 50 || !VoucherCodeRegex().IsMatch(code)) errors["code"] = ["Code may only contain A-Z, 0-9, hyphens and underscores."];
        else if (await repository.CodeExistsAsync(code, excludeId, cancellationToken)) errors["code"] = ["Voucher code already exists."];
        if (!TryParseType(request.DiscountType, out var type)) errors["discount_type"] = ["Discount type must be percentage or fixed."];
        if (request.DiscountValue <= 0 || (type == DiscountType.Percentage && request.DiscountValue > 100)) errors["discount_value"] = ["Discount value is invalid."];
        if (request.MinOrderAmount < 0) errors["min_order_amount"] = ["Minimum order amount cannot be negative."];
        if (request.MaxDiscount <= 0) errors["max_discount"] = ["Maximum discount must be greater than zero."];
        if (request.Quantity < 0) errors["quantity"] = ["Quantity cannot be negative."];
        if (errors.Count > 0) throw new AdminVoucherServiceException(400, "Voucher data is invalid.", errors);
        return type;
    }

    private static void Apply(Voucher voucher, AdminVoucherUpsertRequest request, DiscountType type)
    {
        voucher.Name = request.Name.Trim(); voucher.Code = request.Code.Trim().ToUpperInvariant();
        voucher.DiscountType = type; voucher.DiscountValue = request.DiscountValue;
        voucher.MinOrderAmount = request.MinOrderAmount; voucher.MaxDiscount = request.MaxDiscount;
        voucher.Quantity = request.Quantity; voucher.ExpiresAt = request.ExpiresAt?.ToUniversalTime();
    }

    private static AdminVoucherResponse Map(Voucher voucher)
    {
        var used = voucher.BookingVouchers.Count;
        var status = voucher.ExpiresAt.HasValue && voucher.ExpiresAt < DateTime.UtcNow ? "expired" : used >= voucher.Quantity ? "used_up" : "active";
        return new(voucher.Id, voucher.Name, voucher.Code, voucher.DiscountType == DiscountType.Percentage ? "percentage" : "fixed",
            voucher.DiscountValue, voucher.MinOrderAmount, voucher.MaxDiscount, voucher.Quantity, used,
            Math.Max(0, voucher.Quantity - used), status, voucher.ExpiresAt, voucher.CreatedAt);
    }

    private static bool TryParseType(string? value, out DiscountType type)
    {
        type = value?.Trim().ToLowerInvariant() == "fixed" ? DiscountType.Fixed : DiscountType.Percentage;
        return value?.Trim().ToLowerInvariant() is "percentage" or "fixed";
    }

    private static AdminVoucherServiceException Error(string field, string message)
        => new(400, "Voucher data is invalid.", new Dictionary<string, string[]> { [field] = [message] });

    [GeneratedRegex("^[A-Z0-9_-]+$")]
    private static partial Regex VoucherCodeRegex();
}

