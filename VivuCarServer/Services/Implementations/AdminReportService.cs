using Repositories.Interfaces;
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
        var totalBookingItems = await repository.CountBookingsAsync(from, to, cancellationToken);
        var totalPages = Math.Max(1, (int)Math.Ceiling(totalBookingItems / (double)pageSize));
        var resolvedPage = Math.Min(page, totalPages);
        var recentBookings = await repository.GetBookingsPageAsync(
            from,
            to,
            (resolvedPage - 1) * pageSize,
            pageSize,
            cancellationToken);
        var totalBookings = snapshots.Sum(item => item.TotalBookings);
        var completedBookings = snapshots.Sum(item => item.CompletedBookings);
        var cancelledBookings = snapshots.Sum(item => item.CancelledBookings);
        var grossRevenue = snapshots.Sum(item => item.GrossRevenue);

        return new AdminRevenueReportResponse
        {
            From = from,
            To = to,
            Summary = new AdminRevenueSummary
            {
                GrossRevenue = grossRevenue,
                NetRevenue = snapshots.Sum(item => item.NetRevenue),
                DepositCollected = snapshots.Sum(item => item.DepositCollected),
                TotalBookings = totalBookings,
                CompletedBookings = completedBookings,
                CancelledBookings = cancelledBookings,
                AverageOrderValue = completedBookings == 0 ? 0 : decimal.Round(grossRevenue / completedBookings, 2),
                CancelRate = totalBookings == 0 ? 0 : decimal.Round(cancelledBookings * 100m / totalBookings, 2)
            },
            Daily = snapshots.Select(item => new AdminRevenueDailyItem
            {
                Date = item.SnapshotDate,
                GrossRevenue = item.GrossRevenue,
                NetRevenue = item.NetRevenue,
                DepositCollected = item.DepositCollected,
                TotalBookings = item.TotalBookings,
                CompletedBookings = item.CompletedBookings,
                CancelledBookings = item.CancelledBookings
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
