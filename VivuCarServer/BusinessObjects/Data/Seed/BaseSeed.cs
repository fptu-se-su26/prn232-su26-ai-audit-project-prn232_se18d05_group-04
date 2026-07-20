using BusinessObjects.Enums;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;

namespace BusinessObjects.Data.Seed;

public sealed record BaseSeedResult(
    int BookingsAdded,
    int ReviewsAdded,
    int PaymentsAdded,
    int RevenueSnapshotsAdded
);

public sealed record SeedSummary(
    int UsersAdded,
    int CarsAdded,
    int ImagesAdded,
    int BookingsAdded,
    int ReviewsAdded,
    int PaymentsAdded,
    int RevenueSnapshotsAdded
);

public static class BaseSeed
{
    public static async Task<BaseSeedResult> SeedAsync(
        VivuCarDbContext dbContext,
        IReadOnlyList<Car> seededCars,
        CancellationToken cancellationToken = default
    )
    {
        var customer = await dbContext.Users
            .Where(user => user.Role == UserRole.Customer)
            .OrderBy(user => user.Id)
            .FirstOrDefaultAsync(cancellationToken);

        var bookingsAdded = 0;
        var reviewsAdded = 0;
        var paymentsAdded = 0;
        if (customer is not null)
        {
            (bookingsAdded, reviewsAdded, paymentsAdded) = await SeedCompletedTripsAsync(
                dbContext,
                customer,
                seededCars,
                cancellationToken
            );
        }

        var snapshotsAdded = await SeedRevenueSnapshotsAsync(dbContext, cancellationToken);
        return new BaseSeedResult(bookingsAdded, reviewsAdded, paymentsAdded, snapshotsAdded);
    }

    private static async Task<(int Bookings, int Reviews, int Payments)> SeedCompletedTripsAsync(
        VivuCarDbContext dbContext,
        AppUser customer,
        IReadOnlyList<Car> seededCars,
        CancellationToken cancellationToken
    )
    {
        var bookingsAdded = 0;
        var reviewsAdded = 0;
        var paymentsAdded = 0;

        foreach (var item in seededCars.OrderBy(car => car.LicensePlate).Take(5).Select((car, index) => (car, index)))
        {
            var code = $"SEED-BK-{item.index + 1:000}";
            var booking = await dbContext.Bookings.SingleOrDefaultAsync(
                entity => entity.BookingCode == code,
                cancellationToken
            );

            if (booking is null)
            {
                var start = new DateTime(2026, 6, 1, 8, 0, 0, DateTimeKind.Utc).AddDays(item.index * 4);
                var end = start.AddDays(3);
                booking = new Booking
                {
                    BookingCode = code,
                    CustomerId = customer.Id,
                    CarId = item.car.Id,
                    StartDateTime = start,
                    EndDateTime = end,
                    PickupLocation = item.car.Location,
                    ReturnLocation = item.car.Location,
                    BasePrice = item.car.DailyPrice * 3,
                    InsuranceFee = item.car.InsuranceFeePerDay * 3,
                    DeliveryFee = item.car.DeliveryFee,
                    DiscountAmount = item.index % 2 == 0 ? 100000m : 0m,
                    DepositAmount = item.car.DepositAmount,
                    TotalAmount = item.car.DailyPrice * 3
                        + item.car.InsuranceFeePerDay * 3
                        + item.car.DeliveryFee
                        - (item.index % 2 == 0 ? 100000m : 0m),
                    RemainingAmount = 0m,
                    Status = BookingStatus.Completed,
                    CreatedAt = start.AddDays(-2)
                };
                dbContext.Bookings.Add(booking);
                await dbContext.SaveChangesAsync(cancellationToken);
                bookingsAdded++;
            }

            if (!await dbContext.Reviews.AnyAsync(review => review.BookingId == booking.Id, cancellationToken))
            {
                dbContext.Reviews.Add(new Review
                {
                    BookingId = booking.Id,
                    CustomerId = customer.Id,
                    CarId = booking.CarId,
                    Rating = item.index is 0 or 3 ? 5 : 4,
                    Comment = item.index % 2 == 0
                        ? "Clean car, accurate listing and friendly owner."
                        : "Comfortable trip and straightforward pickup process.",
                    CreatedAt = booking.EndDateTime.AddHours(6)
                });
                reviewsAdded++;
            }

            var transactionCode = $"SEED-TXN-{item.index + 1:000}";
            if (!await dbContext.PaymentTransactions.AnyAsync(
                    payment => payment.TransactionCode == transactionCode,
                    cancellationToken
                ))
            {
                dbContext.PaymentTransactions.Add(new PaymentTransaction
                {
                    BookingId = booking.Id,
                    PaymentProvider = item.index % 2 == 0 ? PaymentProvider.VNPay : PaymentProvider.MoMo,
                    TransactionCode = transactionCode,
                    Amount = booking.TotalAmount,
                    Status = PaymentStatus.Success,
                    PaidAt = booking.CreatedAt.AddHours(2),
                    RawResponse = "{\"seed\":true,\"status\":\"success\"}",
                    CreatedAt = booking.CreatedAt.AddHours(1)
                });
                paymentsAdded++;
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return (bookingsAdded, reviewsAdded, paymentsAdded);
    }

    private static async Task<int> SeedRevenueSnapshotsAsync(
        VivuCarDbContext dbContext,
        CancellationToken cancellationToken
    )
    {
        var added = 0;
        for (var index = 0; index < 5; index++)
        {
            var date = new DateOnly(2026, 6, 20 + index);
            if (await dbContext.DailyRevenueSnapshots.AnyAsync(
                    snapshot => snapshot.SnapshotDate == date,
                    cancellationToken
                )) continue;

            var gross = 2250000m + index * 475000m;
            dbContext.DailyRevenueSnapshots.Add(new DailyRevenueSnapshot
            {
                SnapshotDate = date,
                TotalBookings = 3 + index,
                CompletedBookings = 2 + index,
                CancelledBookings = index % 2,
                GrossRevenue = gross,
                NetRevenue = decimal.Round(gross * 0.9m, 0),
                DepositCollected = 3000000m + index * 500000m,
                GeneratedAt = date.ToDateTime(new TimeOnly(23, 0), DateTimeKind.Utc)
            });
            added++;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return added;
    }
}