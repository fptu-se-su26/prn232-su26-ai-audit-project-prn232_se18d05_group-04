using System.Globalization;
using BusinessObjects.Data;
using BusinessObjects.Enums;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;

namespace API.HostedServices;

public class BookingExpirationHostedService(
    IServiceScopeFactory scopeFactory,
    ILogger<BookingExpirationHostedService> logger
) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Booking Expiration Hosted Service started.");

        // Run every 30 seconds
        var checkInterval = TimeSpan.FromSeconds(30);

        using var timer = new PeriodicTimer(checkInterval);

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            await ExpireBookingsAsync(stoppingToken);
        }
    }

    private async Task ExpireBookingsAsync(CancellationToken cancellationToken)
    {
        try
        {
            await using var scope = scopeFactory.CreateAsyncScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<VivuCarDbContext>();

            var expirationThreshold = DateTime.UtcNow.AddMinutes(-15);

            // Fetch bookings in WaitingDeposit state that have exceeded the 15-minute payment window
            var expiredBookings = await dbContext.Bookings
                .Include(b => b.AvailabilityBlocks)
                .Include(b => b.BookingVoucher)
                .Where(b => b.Status == BookingStatus.WaitingDeposit && b.UpdatedAt < expirationThreshold)
                .ToListAsync(cancellationToken);

            if (expiredBookings.Count > 0)
            {
                logger.LogInformation("Found {Count} expired waiting deposit bookings to process.", expiredBookings.Count);

                foreach (var booking in expiredBookings)
                {
                    var oldStatus = booking.Status;
                    booking.Status = BookingStatus.Expired;
                    booking.CancellationReason = "Hết hạn thanh toán cọc 15 phút";
                    booking.CancelledAt = DateTime.UtcNow;
                    booking.UpdatedAt = DateTime.UtcNow;

                    // Release availability blocks
                    if (booking.AvailabilityBlocks.Count > 0)
                    {
                        dbContext.CarAvailabilityBlocks.RemoveRange(booking.AvailabilityBlocks);
                    }

                    // Log status history
                    booking.StatusHistories.Add(new BookingStatusHistory
                    {
                        OldStatus = oldStatus,
                        NewStatus = BookingStatus.Expired,
                        ChangedByUserId = 0, // System
                        Note = "Đã tự động hủy đơn đặt xe do hết hạn cọc 15 phút.",
                        CreatedAt = DateTime.UtcNow
                    });

                    // Restore voucher usage count
                    if (booking.BookingVoucher != null)
                    {
                        var voucher = await dbContext.Vouchers
                            .SingleOrDefaultAsync(v => v.Code.ToUpper() == booking.BookingVoucher.Code.ToUpper(), cancellationToken);
                        if (voucher != null && voucher.UsedCount > 0)
                        {
                            voucher.UsedCount--;
                        }
                    }
                }

                var updated = await dbContext.SaveChangesAsync(cancellationToken);
                logger.LogInformation("Successfully expired {Count} bookings. Rows affected: {Rows}", expiredBookings.Count, updated);
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            // Normal shutdown
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Error occurred while executing Booking Expiration background task.");
        }
    }
}
