using Repositories.Interfaces;
using Repositories.Models;
using Services.Interfaces;
using Services.Models.Admin;

namespace Services.Implementations;

public class AdminReportService(IAdminReportRepository repository) : IAdminReportService
{
    private const int MaximumRangeDays = 366;

    public async Task<AdminRevenueReportResponse> GetRevenueAsync(
        DateOnly from,
        DateOnly to,
        int page = 1,
        int pageSize = 5,
        string? search = null,
        string? bookingStatus = null,
        string? paymentStatus = null,
        CancellationToken cancellationToken = default)
    {
        if (from > to)
        {
            throw new AdminReportValidationException("from must be on or before to.");
        }

        if (to.DayNumber - from.DayNumber + 1 > MaximumRangeDays)
        {
            throw new AdminReportValidationException($"Date range cannot exceed {MaximumRangeDays} days.");
        }

        if (page < 1)
        {
            throw new AdminReportValidationException("page must be at least 1.");
        }

        if (pageSize is < 1 or > 50)
        {
            throw new AdminReportValidationException("pageSize must be between 1 and 50.");
        }

        var snapshots = await repository.GetRevenueSnapshotsAsync(from, to, cancellationToken);
        var revenueStatusByDate = (await repository.GetRevenueStatusByDateAsync(from, to, cancellationToken))
            .ToDictionary(item => item.Date);
        var revenueByHour = from == to
            ? await repository.GetRevenueByHourAsync(from, cancellationToken)
            : Array.Empty<AdminHourlyRevenueRecord>();
        var revenueByHourLookup = revenueByHour.ToDictionary(item => item.Hour);
        var totalBookingItems = await repository.CountBookingsAsync(from, to, search, bookingStatus, paymentStatus, cancellationToken);
        var totalPages = Math.Max(1, (int)Math.Ceiling(totalBookingItems / (double)pageSize));
        var resolvedPage = Math.Min(page, totalPages);
        var recentBookings = await repository.GetBookingsPageAsync(
            from,
            to,
            (resolvedPage - 1) * pageSize,
            pageSize,
            search,
            bookingStatus,
            paymentStatus,
            cancellationToken);
        var totalBookings = snapshots.Sum(item => item.TotalBookings);
        var completedBookings = snapshots.Sum(item => item.CompletedBookings);
        var cancelledBookings = snapshots.Sum(item => item.CancelledBookings);
        var grossRevenue = snapshots.Sum(item => item.GrossRevenue);
        var pendingPaymentAmount = revenueStatusByDate.Values.Sum(item => item.PendingPaymentAmount);
        var cancelledAmount = revenueStatusByDate.Values.Sum(item => item.CancelledAmount);
        var paidBookings = revenueStatusByDate.Values.Sum(item => item.PaidBookings);
        var pendingPaymentBookings = revenueStatusByDate.Values.Sum(item => item.PendingPaymentBookings);
        var cancelledWithoutPaymentBookings = revenueStatusByDate.Values.Sum(item => item.CancelledWithoutPaymentBookings);

        return new AdminRevenueReportResponse
        {
            From = from,
            To = to,
            Summary = new AdminRevenueSummary
            {
                GrossRevenue = grossRevenue,
                PendingPaymentAmount = pendingPaymentAmount,
                CancelledAmount = cancelledAmount,
                NetRevenue = snapshots.Sum(item => item.NetRevenue),
                DepositCollected = snapshots.Sum(item => item.DepositCollected),
                TotalBookings = totalBookings,
                PaidBookings = paidBookings,
                PendingPaymentBookings = pendingPaymentBookings,
                CancelledWithoutPaymentBookings = cancelledWithoutPaymentBookings,
                CompletedBookings = completedBookings,
                CancelledBookings = cancelledBookings,
                AverageOrderValue = paidBookings == 0 ? 0 : decimal.Round(grossRevenue / paidBookings, 2),
                CancelRate = totalBookings == 0 ? 0 : decimal.Round(cancelledBookings * 100m / totalBookings, 2)
            },
            Daily = snapshots.Select(item => new AdminRevenueDailyItem
            {
                Date = item.SnapshotDate,
                GrossRevenue = item.GrossRevenue,
                PendingPaymentAmount = revenueStatusByDate.GetValueOrDefault(item.SnapshotDate)?.PendingPaymentAmount ?? 0,
                CancelledAmount = revenueStatusByDate.GetValueOrDefault(item.SnapshotDate)?.CancelledAmount ?? 0,
                NetRevenue = item.NetRevenue,
                DepositCollected = item.DepositCollected,
                TotalBookings = item.TotalBookings,
                PaidBookings = revenueStatusByDate.GetValueOrDefault(item.SnapshotDate)?.PaidBookings ?? 0,
                PendingPaymentBookings = revenueStatusByDate.GetValueOrDefault(item.SnapshotDate)?.PendingPaymentBookings ?? 0,
                CompletedBookings = item.CompletedBookings,
                CancelledBookings = item.CancelledBookings
            }).ToList(),
            Hourly = Enumerable.Range(0, 24).Select(hour => new AdminRevenueHourlyItem
            {
                Hour = hour,
                GrossRevenue = revenueByHourLookup.GetValueOrDefault(hour)?.GrossRevenue ?? 0,
                PaidTransactions = revenueByHourLookup.GetValueOrDefault(hour)?.PaidTransactions ?? 0
            }).ToList(),
            Pagination = new AdminRevenuePagination
            {
                Page = resolvedPage,
                PageSize = pageSize,
                TotalItems = totalBookingItems,
                TotalPages = totalPages
            },
            RecentBookings = recentBookings.Select(item => new AdminRevenueRecentBooking
            {
                Id = item.Id,
                BookingCode = item.BookingCode,
                CustomerName = item.CustomerName,
                CarName = item.CarName,
                PickupDate = item.PickupDate,
                TotalAmount = item.TotalAmount,
                BookingStatus = NormalizeBookingStatus(item.BookingStatus),
                PaymentStatus = NormalizePaymentStatus(item.PaymentStatus)
            }).ToList()
        };
    }

    private static string NormalizeBookingStatus(string value) => value.Trim().ToLowerInvariant() switch
    {
        "pendingapproval" => "pending",
        "waitingdeposit" => "pending",
        "waitingpickup" => "approved",
        "inprogress" => "approved",
        "returnrequested" => "approved",
        "completed" => "completed",
        "rejected" => "rejected",
        "cancelled" => "cancelled",
        "expired" => "cancelled",
        _ => "pending"
    };

    private static string NormalizePaymentStatus(string value) => value.Trim().ToLowerInvariant() switch
    {
        "success" => "success",
        "failed" => "failed",
        "cancelled" => "failed",
        _ => "pending"
    };
}

public sealed class AdminReportValidationException(string message) : Exception(message);
