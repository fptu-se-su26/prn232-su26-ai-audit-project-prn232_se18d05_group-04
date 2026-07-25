using BusinessObjects.Data;
using BusinessObjects.Data.Seed;
using BusinessObjects.Enums;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;

namespace API.HostedServices;

/// <summary>
/// Seeds essential demo data: users, cars, bookings, payments, and incidents.
/// Safe to run multiple times (idempotent checks before inserting).
/// </summary>
public class AppDbSeederHostedService(
    IServiceScopeFactory scopeFactory,
    ILogger<AppDbSeederHostedService> logger
) : IHostedService
{
    // Password123!
    private const string DefaultPasswordHash =
        "AQAAAAIAAYagAAAAEJn3SErB4UJSTrOTrtphxBkS/fG69fareXbTJkKAzNAuzibKzreYyYlZE0iTw5gDbQ==";

    // Legacy demo plates are retained because bookings reference them. Metadata and
    // galleries are synchronized to distinct catalog cars so each car matches its images.
    private static readonly IReadOnlyDictionary<string, string> LegacyCarCatalogSlugs =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["43A-56789"] = "vinfast-vf3-2025",
            ["43A-99911"] = "vinfast-vf3-2026",
            ["43A-88888"] = "vinfast-vf7-eco-2026",
            ["43A-22233"] = "vinfast-limo-green-2025",
            ["43A-66677"] = "vinfast-vf7-plus-2025",
            ["43A-77788"] = "mazda-cx8-premium-2023",
            ["43A-12345"] = "vinfast-vf5-2024",
            ["43B-67890"] = "vinfast-vf6-eco-2024",
            ["43C-11111"] = "vinfast-vf5-2025",
            ["43D-22222"] = "vinfast-vf7-2025",
            ["43E-33333"] = "vinfast-vf6-plus-2024",
            ["43F-44444"] = "vinfast-minio-green-2026"
        };

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<VivuCarDbContext>();

        logger.LogInformation("[Seeder] Starting data seed...");

        // ── 1. Users ─────────────────────────────────────────────────────────
        var admin     = await EnsureUser(db, "admin@vivucar.local",     "System Admin",       UserRole.Admin,     "0900000001", cancellationToken);
        var customer1 = await EnsureUser(db, "customer01@vivucar.local", "Nguyễn Văn An",     UserRole.Customer,  "0900000002", cancellationToken);
        var customer2 = await EnsureUser(db, "customer02@vivucar.local", "Trần Thị Bình",     UserRole.Customer,  "0900000003", cancellationToken);
        var customer3 = await EnsureUser(db, "customer03@vivucar.local", "Khách hàng GPLX 03", UserRole.Customer,  "0900000004", cancellationToken);
        var owner1    = await EnsureUser(db, "owner01@vivucar.local",   "Lê Hoàng Nam",       UserRole.CarOwner,  "0900000006", cancellationToken);
        var owner2    = await EnsureUser(db, "owner02@vivucar.local",   "Đặng Hoàng Long",    UserRole.CarOwner,  "0900000007", cancellationToken);

        logger.LogInformation("[Seeder] Users ready.");

        // ── 2. Car meta ───────────────────────────────────────────────────────
        var brandToyota  = await EnsureCarBrand(db, "Toyota",  cancellationToken);
        var brandHonda   = await EnsureCarBrand(db, "Honda",   cancellationToken);
        var brandKia     = await EnsureCarBrand(db, "Kia",     cancellationToken);
        var brandMazda   = await EnsureCarBrand(db, "Mazda",   cancellationToken);
        var brandHyundai = await EnsureCarBrand(db, "Hyundai", cancellationToken);

        var modelVios    = await EnsureCarModel(db, brandToyota,  "Vios",    cancellationToken);
        var modelCamry   = await EnsureCarModel(db, brandToyota,  "Camry",   cancellationToken);
        var modelCivic   = await EnsureCarModel(db, brandHonda,   "Civic",   cancellationToken);
        var modelCRV     = await EnsureCarModel(db, brandHonda,   "CR-V",    cancellationToken);
        var modelSportage= await EnsureCarModel(db, brandKia,     "Sportage",cancellationToken);
        var modelMazda3  = await EnsureCarModel(db, brandMazda,   "Mazda 3", cancellationToken);
        var modelTucson  = await EnsureCarModel(db, brandHyundai, "Tucson",  cancellationToken);

        var typeSuv   = await EnsureCarType(db, "SUV",   cancellationToken);
        var typeSedan = await EnsureCarType(db, "Sedan", cancellationToken);
        var typeHatch = await EnsureCarType(db, "Hatchback", cancellationToken);

        logger.LogInformation("[Seeder] Car meta (brands/models/types) ready.");

        // ── 3. Cars ───────────────────────────────────────────────────────────
        var car1 = await EnsureCar(db, owner2, brandToyota, modelVios, typeSedan,
            name: "Toyota Vios 2022", plate: "43A-12345",
            dailyPrice: 650_000, location: "Hải Châu, Đà Nẵng",
            seats: 5, transmission: TransmissionType.Automatic, fuel: FuelType.Gasoline,
            status: CarStatus.Available, year: 2022, km: 28_000,
            description: "Xe sạch, vận hành ổn định, tiết kiệm nhiên liệu. Phù hợp cho gia đình.",
            cancellationToken: cancellationToken);

        var car2 = await EnsureCar(db, owner2, brandHonda, modelCRV, typeSuv,
            name: "Honda CR-V 2023", plate: "43B-67890",
            dailyPrice: 980_000, location: "Ngũ Hành Sơn, Đà Nẵng",
            seats: 5, transmission: TransmissionType.Automatic, fuel: FuelType.Gasoline,
            status: CarStatus.Available, year: 2023, km: 12_000,
            description: "SUV rộng rãi, trang bị đầy đủ tiện nghi cao cấp. Phù hợp đi phượt.",
            cancellationToken: cancellationToken);

        var car3 = await EnsureCar(db, owner2, brandKia, modelSportage, typeSuv,
            name: "Kia Sportage 2022", plate: "43C-11111",
            dailyPrice: 900_000, location: "Liên Chiểu, Đà Nẵng",
            seats: 5, transmission: TransmissionType.Automatic, fuel: FuelType.Gasoline,
            status: CarStatus.Rented, year: 2022, km: 42_000,
            description: "SUV cỡ trung, mạnh mẽ và tiện dụng.",
            cancellationToken: cancellationToken);

        var car4 = await EnsureCar(db, owner2, brandMazda, modelMazda3, typeSedan,
            name: "Mazda 3 2021", plate: "43D-22222",
            dailyPrice: 700_000, location: "Sơn Trà, Đà Nẵng",
            seats: 5, transmission: TransmissionType.Automatic, fuel: FuelType.Gasoline,
            status: CarStatus.Available, year: 2021, km: 55_000,
            description: "Sedan thể thao, thiết kế đẹp, phù hợp đi công tác.",
            cancellationToken: cancellationToken);

        var car5 = await EnsureCar(db, owner1, brandHyundai, modelTucson, typeSuv,
            name: "Hyundai Tucson 2023", plate: "43E-33333",
            dailyPrice: 950_000, location: "Cẩm Lệ, Đà Nẵng",
            seats: 5, transmission: TransmissionType.Automatic, fuel: FuelType.Gasoline,
            status: CarStatus.Maintenance, year: 2023, km: 8_000,
            description: "SUV hiện đại, sang trọng. Đang bảo trì định kỳ.",
            cancellationToken: cancellationToken);

        var car6 = await EnsureCar(db, owner1, brandToyota, modelCamry, typeSedan,
            name: "Toyota Camry 2022", plate: "43F-44444",
            dailyPrice: 1_100_000, location: "Thanh Khê, Đà Nẵng",
            seats: 5, transmission: TransmissionType.Automatic, fuel: FuelType.Gasoline,
            status: CarStatus.Available, year: 2022, km: 22_000,
            description: "Sedan hạng D sang trọng, êm ái, phù hợp công tác dài ngày.",
            cancellationToken: cancellationToken);

        var legacyCatalogSync = await CarSeed.SynchronizeCarsFromCatalogAsync(
            db,
            LegacyCarCatalogSlugs,
            cancellationToken
        );
        logger.LogInformation(
            "[Seeder] Legacy cars synchronized: {CarsUpdated} cars, {ImagesAdded} images added, {ImagesRemoved} stale or duplicate images removed.",
            legacyCatalogSync.CarsUpdated,
            legacyCatalogSync.ImagesAdded,
            legacyCatalogSync.ImagesRemoved
        );

        logger.LogInformation("[Seeder] Cars ready.");

        // ── 4. Voucher ────────────────────────────────────────────────────────
        await EnsureVoucher(db, cancellationToken);
        await EnsureModerationSamples(db, customer1, customer2, customer3, cancellationToken);

        // ── 5. Bookings & Payments & Incidents ────────────────────────────────
        logger.LogInformation("[Seeder] Seeding bookings...");
        await SeedBookingsAsync(db, customer1, customer2, owner1, owner2, car1, car2, car3, car4, car6, cancellationToken);
        var snapshotsChanged = await BaseSeed.RefreshRevenueSnapshotsAsync(db, cancellationToken);
        logger.LogInformation("[Seeder] Revenue snapshots synchronized: {SnapshotsChanged} changed.", snapshotsChanged);

        logger.LogInformation("[Seeder] Seed completed successfully.");
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    // ─── Seed helpers ─────────────────────────────────────────────────────────

    private async Task SeedBookingsAsync(
        VivuCarDbContext db,
        AppUser customer1, AppUser customer2,
        AppUser owner1, AppUser owner2,
        Car car1, Car car2, Car car3, Car car4, Car car6,
        CancellationToken ct)
    {
        var now = DateTime.UtcNow;

        // Booking 1: PendingApproval (chờ chủ xe duyệt)
        var code1 = "BK-PEND-001";
        var b1 = await db.Bookings.FirstOrDefaultAsync(b => b.BookingCode == code1, ct);
        if (b1 == null)
        {
            b1 = new Booking
            {
                BookingCode   = code1,
                CustomerId    = customer1.Id,
                CarId         = car1.Id,
                StartDateTime = now.AddDays(3),
                EndDateTime   = now.AddDays(5),
                PickupLocation  = "123 Lê Duẩn, Hải Châu, Đà Nẵng",
                ReturnLocation  = "123 Lê Duẩn, Hải Châu, Đà Nẵng",
                BasePrice     = car1.DailyPrice * 2,
                TotalAmount   = car1.DailyPrice * 2,
                RemainingAmount = car1.DailyPrice * 2,
                Status        = BookingStatus.PendingApproval,
                CreatedAt     = now.AddHours(-2)
            };
            db.Bookings.Add(b1);
        }

        // Booking 1.5: PendingApproval (chờ chủ xe duyệt - Đơn mới để test)
        var code15 = "BK-PEND-010";
        var b15 = await db.Bookings.FirstOrDefaultAsync(b => b.BookingCode == code15, ct);
        if (b15 == null)
        {
            b15 = new Booking
            {
                BookingCode   = code15,
                CustomerId    = customer2.Id,
                CarId         = car2.Id,
                StartDateTime = now.AddDays(4),
                EndDateTime   = now.AddDays(7),
                PickupLocation  = "123 Lê Duẩn, Hải Châu, Đà Nẵng",
                ReturnLocation  = "123 Lê Duẩn, Hải Châu, Đà Nẵng",
                BasePrice     = car2.DailyPrice * 3,
                TotalAmount   = car2.DailyPrice * 3,
                RemainingAmount = car2.DailyPrice * 3,
                Status        = BookingStatus.PendingApproval,
                CreatedAt     = now.AddHours(-1)
            };
            db.Bookings.Add(b15);
        }

        // Booking 2: WaitingDeposit (đã duyệt, chờ đặt cọc)
        var code2 = "BK-WDEP-002";
        var b2 = await db.Bookings.FirstOrDefaultAsync(b => b.BookingCode == code2, ct);
        if (b2 == null)
        {
            b2 = new Booking
            {
                BookingCode   = code2,
                CustomerId    = customer2.Id,
                CarId         = car4.Id,
                StartDateTime = now.AddDays(6),
                EndDateTime   = now.AddDays(8),
                PickupLocation  = "456 Nguyễn Văn Linh, Sơn Trà, Đà Nẵng",
                ReturnLocation  = "456 Nguyễn Văn Linh, Sơn Trà, Đà Nẵng",
                BasePrice     = car4.DailyPrice * 2,
                TotalAmount   = car4.DailyPrice * 2,
                RemainingAmount = car4.DailyPrice * 2,
                Status        = BookingStatus.WaitingDeposit,
                CreatedAt     = now.AddDays(-1)
            };
            db.Bookings.Add(b2);
        }

        // Booking 3: WaitingPickup (đã cọc, chờ nhận xe)
        var depositAmt3 = car2.DailyPrice * 3 * 0.3m;
        var code3 = "BK-WPICK-003";
        var b3 = await db.Bookings.FirstOrDefaultAsync(b => b.BookingCode == code3, ct);
        if (b3 == null)
        {
            b3 = new Booking
            {
                BookingCode   = code3,
                CustomerId    = customer1.Id,
                CarId         = car2.Id,
                StartDateTime = now.AddDays(2),
                EndDateTime   = now.AddDays(5),
                PickupLocation  = "789 Ngũ Hành Sơn, Đà Nẵng",
                ReturnLocation  = "789 Ngũ Hành Sơn, Đà Nẵng",
                BasePrice     = car2.DailyPrice * 3,
                DepositAmount = depositAmt3,
                TotalAmount   = car2.DailyPrice * 3,
                RemainingAmount = car2.DailyPrice * 3 - depositAmt3,
                Status        = BookingStatus.WaitingPickup,
                CreatedAt     = now.AddDays(-2)
            };
            db.Bookings.Add(b3);
        }

        // Booking 4: InProgress (đang thuê — xe car3 đã Rented)
        var depositAmt4 = car3.DailyPrice * 3 * 0.3m;
        var code4 = "BK-INPROG-004";
        var b4 = await db.Bookings.FirstOrDefaultAsync(b => b.BookingCode == code4, ct);
        if (b4 == null)
        {
            b4 = new Booking
            {
                BookingCode   = code4,
                CustomerId    = customer2.Id,
                CarId         = car3.Id,
                StartDateTime = now.AddDays(-1),
                EndDateTime   = now.AddDays(2),
                PickupLocation  = "321 Liên Chiểu, Đà Nẵng",
                ReturnLocation  = "321 Liên Chiểu, Đà Nẵng",
                BasePrice     = car3.DailyPrice * 3,
                DepositAmount = depositAmt4,
                TotalAmount   = car3.DailyPrice * 3,
                RemainingAmount = car3.DailyPrice * 3 - depositAmt4,
                Status        = BookingStatus.InProgress,
                CreatedAt     = now.AddDays(-3)
            };
            db.Bookings.Add(b4);
        }

        // Booking 5: ReturnRequested (khách đã yêu cầu trả xe)
        var depositAmt5 = car1.DailyPrice * 2 * 0.3m;
        var code5 = "BK-RETRN-005";
        var b5 = await db.Bookings.FirstOrDefaultAsync(b => b.BookingCode == code5, ct);
        if (b5 == null)
        {
            b5 = new Booking
            {
                BookingCode   = code5,
                CustomerId    = customer1.Id,
                CarId         = car1.Id,
                StartDateTime = now.AddDays(-5),
                EndDateTime   = now.AddDays(-1),
                PickupLocation  = "55 Trần Phú, Hải Châu, Đà Nẵng",
                ReturnLocation  = "55 Trần Phú, Hải Châu, Đà Nẵng",
                BasePrice     = car1.DailyPrice * 4,
                DepositAmount = depositAmt5,
                TotalAmount   = car1.DailyPrice * 4,
                RemainingAmount = car1.DailyPrice * 4 - depositAmt5,
                Status        = BookingStatus.ReturnRequested,
                CreatedAt     = now.AddDays(-7)
            };
            db.Bookings.Add(b5);
        }

        // Booking 6: Completed (hoàn thành)
        var depositAmt6 = car6.DailyPrice * 2 * 0.3m;
        var code6 = "BK-DONE-006";
        var b6 = await db.Bookings.FirstOrDefaultAsync(b => b.BookingCode == code6, ct);
        if (b6 == null)
        {
            b6 = new Booking
            {
                BookingCode   = code6,
                CustomerId    = customer1.Id,
                CarId         = car6.Id,
                StartDateTime = now.AddDays(-14),
                EndDateTime   = now.AddDays(-12),
                PickupLocation  = "10 Hùng Vương, Thanh Khê, Đà Nẵng",
                ReturnLocation  = "10 Hùng Vương, Thanh Khê, Đà Nẵng",
                BasePrice     = car6.DailyPrice * 2,
                DepositAmount = depositAmt6,
                TotalAmount   = car6.DailyPrice * 2,
                RemainingAmount = 0,
                Status        = BookingStatus.Completed,
                CreatedAt     = now.AddDays(-15),
                UpdatedAt     = now.AddDays(-12)
            };
            db.Bookings.Add(b6);
        }

        // Booking 7: Cancelled
        var code7 = "BK-CNCL-007";
        var b7 = await db.Bookings.FirstOrDefaultAsync(b => b.BookingCode == code7, ct);
        if (b7 == null)
        {
            b7 = new Booking
            {
                BookingCode   = code7,
                CustomerId    = customer2.Id,
                CarId         = car4.Id,
                StartDateTime = now.AddDays(-10),
                EndDateTime   = now.AddDays(-8),
                PickupLocation  = "Sơn Trà, Đà Nẵng",
                ReturnLocation  = "Sơn Trà, Đà Nẵng",
                BasePrice     = car4.DailyPrice * 2,
                TotalAmount   = car4.DailyPrice * 2,
                RemainingAmount = car4.DailyPrice * 2,
                Status        = BookingStatus.Cancelled,
                CancellationReason = "Khách thay đổi kế hoạch chuyến đi.",
                CancelledAt   = now.AddDays(-11),
                CreatedAt     = now.AddDays(-12)
            };
            db.Bookings.Add(b7);
        }

        // Booking 8: Completed (owner2 - car1 - completed 5 ngày trước)
        var depositAmt8 = car1.DailyPrice * 3 * 0.3m;
        var code8 = "BK-DONE-008";
        var b8 = await db.Bookings.FirstOrDefaultAsync(b => b.BookingCode == code8, ct);
        if (b8 == null)
        {
            b8 = new Booking
            {
                BookingCode     = code8,
                CustomerId      = customer1.Id,
                CarId           = car1.Id,
                StartDateTime   = now.AddDays(-20),
                EndDateTime     = now.AddDays(-17),
                PickupLocation  = "123 Lê Duẩn, Hải Châu, Đà Nẵng",
                ReturnLocation  = "123 Lê Duẩn, Hải Châu, Đà Nẵng",
                BasePrice       = car1.DailyPrice * 3,
                DepositAmount   = depositAmt8,
                TotalAmount     = car1.DailyPrice * 3,
                RemainingAmount = 0,
                Status          = BookingStatus.Completed,
                CreatedAt       = now.AddDays(-21),
                UpdatedAt       = now.AddDays(-17)
            };
            db.Bookings.Add(b8);
        }

        // Booking 9: Completed (owner2 - car2 - completed 8 ngày trước)
        var depositAmt9 = car2.DailyPrice * 2 * 0.3m;
        var code9 = "BK-DONE-009";
        var b9 = await db.Bookings.FirstOrDefaultAsync(b => b.BookingCode == code9, ct);
        if (b9 == null)
        {
            b9 = new Booking
            {
                BookingCode     = code9,
                CustomerId      = customer2.Id,
                CarId           = car2.Id,
                StartDateTime   = now.AddDays(-30),
                EndDateTime     = now.AddDays(-28),
                PickupLocation  = "789 Ngũ Hành Sơn, Đà Nẵng",
                ReturnLocation  = "789 Ngũ Hành Sơn, Đà Nẵng",
                BasePrice       = car2.DailyPrice * 2,
                DepositAmount   = depositAmt9,
                TotalAmount     = car2.DailyPrice * 2,
                RemainingAmount = 0,
                Status          = BookingStatus.Completed,
                CreatedAt       = now.AddDays(-31),
                UpdatedAt       = now.AddDays(-28)
            };
            db.Bookings.Add(b9);
        }

        // Booking 11: ReturnRequested (để test tính năng trả xe)
        var code11 = "BK-RETRN-011";
        var b11 = await db.Bookings.FirstOrDefaultAsync(b => b.BookingCode == code11, ct);
        if (b11 == null)
        {
            var depositAmt11 = car2.DailyPrice * 3 * 0.3m;
            b11 = new Booking
            {
                BookingCode   = code11,
                CustomerId    = customer2.Id,
                CarId         = car2.Id,
                StartDateTime = now.AddDays(-3),
                EndDateTime   = now.AddDays(1),
                PickupLocation  = "Sân bay Đà Nẵng",
                ReturnLocation  = "Sân bay Đà Nẵng",
                BasePrice     = car2.DailyPrice * 4,
                DepositAmount = depositAmt11,
                TotalAmount   = car2.DailyPrice * 4,
                RemainingAmount = car2.DailyPrice * 4 - depositAmt11,
                Status        = BookingStatus.ReturnRequested,
                CreatedAt     = now.AddDays(-4)
            };
            db.Bookings.Add(b11);
        }

        // Add 5 more ReturnRequested bookings for testing
        for (int i = 12; i <= 16; i++)
        {
            var code = $"BK-RETRN-0{i}";
            var b = await db.Bookings.FirstOrDefaultAsync(bk => bk.BookingCode == code, ct);
            if (b == null)
            {
                var depositAmt = car2.DailyPrice * 3 * 0.3m;
                b = new Booking
                {
                    BookingCode   = code,
                    CustomerId    = customer2.Id,
                    CarId         = car2.Id,
                    StartDateTime = now.AddDays(-3),
                    EndDateTime   = now.AddDays(1),
                    PickupLocation  = "Sân bay Đà Nẵng",
                    ReturnLocation  = "Sân bay Đà Nẵng",
                    BasePrice     = car2.DailyPrice * 4,
                    DepositAmount = depositAmt,
                    TotalAmount   = car2.DailyPrice * 4,
                    RemainingAmount = car2.DailyPrice * 4 - depositAmt,
                    Status        = BookingStatus.ReturnRequested,
                    CreatedAt     = now.AddDays(-4)
                };
                db.Bookings.Add(b);
            }
        }

        await db.SaveChangesAsync(ct);

        // ── Payments ──────────────────────────────────────────────────────────

        if (!await db.PaymentTransactions.AnyAsync(p => p.TransactionCode == "VNP-B3-DEP01", ct))
        {
            db.PaymentTransactions.Add(new PaymentTransaction
            {
                BookingId       = b3.Id,
                Amount          = depositAmt3,
                PaymentProvider = PaymentProvider.VNPay,
                Status          = PaymentStatus.Success,
                TransactionCode = "VNP-B3-DEP01",
                PaidAt          = now.AddDays(-2),
                CreatedAt       = now.AddDays(-2)
            });
        }

        if (!await db.PaymentTransactions.AnyAsync(p => p.TransactionCode == "MOMO-B4-DEP01", ct))
        {
            db.PaymentTransactions.Add(new PaymentTransaction
            {
                BookingId       = b4.Id,
                Amount          = depositAmt4,
                PaymentProvider = PaymentProvider.MoMo,
                Status          = PaymentStatus.Success,
                TransactionCode = "MOMO-B4-DEP01",
                PaidAt          = now.AddDays(-3),
                CreatedAt       = now.AddDays(-3)
            });
        }

        if (!await db.PaymentTransactions.AnyAsync(p => p.TransactionCode == "VNP-B5-DEP01", ct))
        {
            db.PaymentTransactions.Add(new PaymentTransaction
            {
                BookingId       = b5.Id,
                Amount          = depositAmt5,
                PaymentProvider = PaymentProvider.VNPay,
                Status          = PaymentStatus.Success,
                TransactionCode = "VNP-B5-DEP01",
                PaidAt          = now.AddDays(-7),
                CreatedAt       = now.AddDays(-7)
            });
        }

        if (!await db.PaymentTransactions.AnyAsync(p => p.TransactionCode == "VNP-B6-DEP01", ct))
        {
            db.PaymentTransactions.Add(new PaymentTransaction
            {
                BookingId       = b6.Id,
                Amount          = depositAmt6,
                PaymentProvider = PaymentProvider.VNPay,
                Status          = PaymentStatus.Success,
                TransactionCode = "VNP-B6-DEP01",
                PaidAt          = now.AddDays(-15),
                CreatedAt       = now.AddDays(-15)
            });
        }

        if (!await db.PaymentTransactions.AnyAsync(p => p.TransactionCode == "CASH-B6-REM01", ct))
        {
            db.PaymentTransactions.Add(new PaymentTransaction
            {
                BookingId       = b6.Id,
                Amount          = car6.DailyPrice * 2 - depositAmt6,
                PaymentProvider = PaymentProvider.MoMo,
                Status          = PaymentStatus.Success,
                TransactionCode = "CASH-B6-REM01",
                PaidAt          = now.AddDays(-12),
                CreatedAt       = now.AddDays(-12)
            });
        }

        // Payment for b8
        if (!await db.PaymentTransactions.AnyAsync(p => p.TransactionCode == "VNP-B8-DEP01", ct))
        {
            db.PaymentTransactions.Add(new PaymentTransaction
            {
                BookingId       = b8.Id,
                Amount          = depositAmt8,
                PaymentProvider = PaymentProvider.VNPay,
                Status          = PaymentStatus.Success,
                TransactionCode = "VNP-B8-DEP01",
                PaidAt          = now.AddDays(-21),
                CreatedAt       = now.AddDays(-21)
            });
        }
        if (!await db.PaymentTransactions.AnyAsync(p => p.TransactionCode == "VNP-B8-REM01", ct))
        {
            db.PaymentTransactions.Add(new PaymentTransaction
            {
                BookingId       = b8.Id,
                Amount          = car1.DailyPrice * 3 - depositAmt8,
                PaymentProvider = PaymentProvider.VNPay,
                Status          = PaymentStatus.Success,
                TransactionCode = "VNP-B8-REM01",
                PaidAt          = now.AddDays(-17),
                CreatedAt       = now.AddDays(-17)
            });
        }
        // Payment for b9
        if (!await db.PaymentTransactions.AnyAsync(p => p.TransactionCode == "VNP-B9-DEP01", ct))
        {
            db.PaymentTransactions.Add(new PaymentTransaction
            {
                BookingId       = b9.Id,
                Amount          = depositAmt9,
                PaymentProvider = PaymentProvider.MoMo,
                Status          = PaymentStatus.Success,
                TransactionCode = "VNP-B9-DEP01",
                PaidAt          = now.AddDays(-31),
                CreatedAt       = now.AddDays(-31)
            });
        }
        if (!await db.PaymentTransactions.AnyAsync(p => p.TransactionCode == "VNP-B9-REM01", ct))
        {
            db.PaymentTransactions.Add(new PaymentTransaction
            {
                BookingId       = b9.Id,
                Amount          = car2.DailyPrice * 2 - depositAmt9,
                PaymentProvider = PaymentProvider.MoMo,
                Status          = PaymentStatus.Success,
                TransactionCode = "VNP-B9-REM01",
                PaidAt          = now.AddDays(-28),
                CreatedAt       = now.AddDays(-28)
            });
        }

        await db.SaveChangesAsync(ct);

        // ── Incidents ─────────────────────────────────────────────────────────

        if (!await db.IncidentReports.AnyAsync(i => i.BookingId == b4.Id, ct))
        {
            db.IncidentReports.Add(new IncidentReport
            {
                BookingId   = b4.Id,
                ReporterId  = owner2.Id,
                Title       = "Trầy xước nhỏ ở cản trước",
                Description = "Khách báo cáo xe bị trầy xước nhỏ ở cản trước phải khi đang thuê. Đã ghi nhận và chụp ảnh hiện trường.",
                Status      = IncidentStatus.Open,
                CreatedAt   = now.AddHours(-6)
            });
        }

        if (!await db.IncidentReports.AnyAsync(i => i.BookingId == b5.Id, ct))
        {
            db.IncidentReports.Add(new IncidentReport
            {
                BookingId   = b5.Id,
                ReporterId  = owner2.Id,
                Title       = "Khách trả xe trễ giờ",
                Description = "Khách trả xe muộn 2 tiếng so với hợp đồng. Đang xem xét phụ phí quá giờ theo quy định.",
                Status      = IncidentStatus.InReview,
                CreatedAt   = now.AddDays(-2)
            });
        }

        // b6 thuộc car6 của owner1 — reporter là owner1
        if (!await db.IncidentReports.AnyAsync(i => i.BookingId == b6.Id, ct))
        {
            db.IncidentReports.Add(new IncidentReport
            {
                BookingId   = b6.Id,
                ReporterId  = owner1.Id,
                Title       = "Xe thiếu nhiên liệu khi trả",
                Description = "Xe về thiếu nhiên liệu so với lúc bàn giao. Khách đã bồi thường 150,000đ tiền xăng.",
                Status      = IncidentStatus.Resolved,
                CreatedAt   = now.AddDays(-13),
                ResolvedAt  = now.AddDays(-12)
            });
        }

        // Sự cố 4: Phạt nguội (b8 - owner2 - Open)
        if (!await db.IncidentReports.AnyAsync(i => i.BookingId == b8.Id, ct))
        {
            db.IncidentReports.Add(new IncidentReport
            {
                BookingId   = b8.Id,
                ReporterId  = owner2.Id,
                Title       = "Phạt nguội vượt tốc độ",
                Description = "Xe nhận được thông báo phạt nguội do vượt tốc độ trên đường cao tốc Đà Nẵng - Quảng Ngãi trong thời gian khách thuê. Đang chờ khách xác nhận và thanh toán phí phạt 1,200,000đ.",
                Status      = IncidentStatus.Open,
                CreatedAt   = now.AddDays(-14)
            });
        }

        // Sự cố 5: Bể lốp xe (b9 - owner2 - Resolved)
        if (!await db.IncidentReports.AnyAsync(i => i.BookingId == b9.Id && i.Title == "Bể lốp xe giữa đường", ct))
        {
            db.IncidentReports.Add(new IncidentReport
            {
                BookingId   = b9.Id,
                ReporterId  = owner2.Id,
                Title       = "Bể lốp xe giữa đường",
                Description = "Khách báo cáo xe bị bể lốp sau khi qua đèo Hải Vân. Đã gọi dịch vụ cứu hộ. Chi phí thay lốp 850,000đ đã được khách thanh toán qua chuyển khoản.",
                Status      = IncidentStatus.Resolved,
                CreatedAt   = now.AddDays(-29),
                ResolvedAt  = now.AddDays(-28)
            });
        }

        // Sự cố 6: Nội thất bị bẩn (b9 - owner2 - InReview)
        if (!await db.IncidentReports.AnyAsync(i => i.BookingId == b9.Id && i.Title == "Nội thất bị dơ nghiêm trọng", ct))
        {
            db.IncidentReports.Add(new IncidentReport
            {
                BookingId   = b9.Id,
                ReporterId  = owner2.Id,
                Title       = "Nội thất bị dơ nghiêm trọng",
                Description = "Sau khi nhận xe, phát hiện ghế da và thảm nội thất bị dính thức ăn, gây mùi khó chịu. Chi phí vệ sinh chuyên sâu ước tính 500,000đ. Đang đàm phán với khách hàng về khoản bồi thường.",
                Status      = IncidentStatus.InReview,
                CreatedAt   = now.AddDays(-27)
            });
        }

        // Sự cố 7: Mất bản đồ và hộp sơ cứu (b5 - owner2 - Closed)
        if (!await db.IncidentReports.AnyAsync(i => i.BookingId == b5.Id && i.Title == "Mất phụ kiện trong xe", ct))
        {
            db.IncidentReports.Add(new IncidentReport
            {
                BookingId   = b5.Id,
                ReporterId  = owner2.Id,
                Title       = "Mất phụ kiện trong xe",
                Description = "Kiểm tra xe sau khi khách trả, phát hiện hộp sơ cứu và bản đồ du lịch tặng kèm đã bị mất. Khách đã bồi thường 300,000đ qua MoMo. Sự cố đã được xử lý.",
                Status      = IncidentStatus.Closed,
                CreatedAt   = now.AddDays(-8),
                ResolvedAt  = now.AddDays(-6)
            });
        }

        await db.SaveChangesAsync(ct);
        logger.LogInformation("[Seeder] Bookings, payments, and incidents seeded.");
    }

    // ─── Ensure helpers ────────────────────────────────────────────────────────

    private async Task<AppUser> EnsureUser(
        VivuCarDbContext db, string email, string fullName,
        UserRole role, string phone, CancellationToken ct)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Email == email, ct);
        if (user != null) return user;

        user = new AppUser
        {
            Email        = email,
            FullName     = fullName,
            Role         = role,
            Status       = UserStatus.Active,
            PhoneNumber  = phone,
            Address      = "Đà Nẵng",
            TokenVersion = 1,
            PasswordHash = DefaultPasswordHash,
            CreatedAt    = DateTime.UtcNow
        };
        db.Users.Add(user);
        await db.SaveChangesAsync(ct);
        return user;
    }

    private static async Task<CarBrand> EnsureCarBrand(VivuCarDbContext db, string name, CancellationToken ct)
    {
        var brand = await db.CarBrands.FirstOrDefaultAsync(b => b.Name == name, ct);
        if (brand != null) return brand;
        brand = new CarBrand { Name = name, IsActive = true };
        db.CarBrands.Add(brand);
        await db.SaveChangesAsync(ct);
        return brand;
    }

    private static async Task<CarModel> EnsureCarModel(VivuCarDbContext db, CarBrand brand, string name, CancellationToken ct)
    {
        var model = await db.CarModels.FirstOrDefaultAsync(m => m.CarBrandId == brand.Id && m.Name == name, ct);
        if (model != null) return model;
        model = new CarModel { CarBrandId = brand.Id, Name = name, IsActive = true };
        db.CarModels.Add(model);
        await db.SaveChangesAsync(ct);
        return model;
    }

    private static async Task<CarType> EnsureCarType(VivuCarDbContext db, string name, CancellationToken ct)
    {
        var type = await db.CarTypes.FirstOrDefaultAsync(t => t.Name == name, ct);
        if (type != null) return type;
        type = new CarType { Name = name, IsActive = true };
        db.CarTypes.Add(type);
        await db.SaveChangesAsync(ct);
        return type;
    }

    private static async Task<Car> EnsureCar(
        VivuCarDbContext db,
        AppUser owner, CarBrand brand, CarModel model, CarType type,
        string name, string plate, decimal dailyPrice, string location,
        int seats, TransmissionType transmission, FuelType fuel,
        CarStatus status, short year, int km, string description,
        CancellationToken cancellationToken)
    {
        var car = await db.Cars.FirstOrDefaultAsync(c => c.LicensePlate == plate, cancellationToken);
        if (car != null) return car;

        car = new Car
        {
            OwnerId          = owner.Id,
            CarBrandId       = brand.Id,
            CarModelId       = model.Id,
            CarTypeId        = type.Id,
            Name             = name,
            LicensePlate     = plate,
            Year             = year,
            KilometersDriven = km,
            Description      = description,
            Location         = location,
            DailyPrice       = dailyPrice,
            PricePerHour     = Math.Round((dailyPrice / 8) / 1000) * 1000,
            InsuranceFeePerDay = 0,
            DeliveryFee      = 0,
            DepositAmount    = Math.Round((dailyPrice * 0.3m) / 1000) * 1000,
            Status           = status,
            SeatCount        = seats,
            TransmissionType = transmission,
            FuelType         = fuel,
            CreatedAt        = DateTime.UtcNow
        };
        db.Cars.Add(car);
        await db.SaveChangesAsync(cancellationToken);
        return car;
    }

    private static async Task EnsureVoucher(VivuCarDbContext db, CancellationToken ct)
    {
        if (await db.Vouchers.AnyAsync(v => v.Code == "VIVUCAR10", ct)) return;
        db.Vouchers.Add(new Voucher
        {
            Name           = "VivuCar 10%",
            Code           = "VIVUCAR10",
            DiscountType   = DiscountType.Percentage,
            DiscountValue  = 10,
            MinOrderAmount = 500_000,
            MaxDiscount    = 200_000,
            Quantity       = 100,
            ExpiresAt      = DateTime.UtcNow.AddMonths(3),
            CreatedAt      = DateTime.UtcNow
        });
        await db.SaveChangesAsync(ct);
    }

    private static async Task EnsureModerationSamples(
        VivuCarDbContext db,
        AppUser pendingCustomer,
        AppUser approvedCustomer,
        AppUser realImageCustomer,
        CancellationToken ct)
    {
        if (!await db.DriverDocuments.AnyAsync(document => document.UserId == pendingCustomer.Id, ct))
        {
            db.DriverDocuments.Add(new DriverDocument
            {
                UserId = pendingCustomer.Id,
                CitizenIdNumber = "048204009731",
                CitizenIdFrontImageUrl = null,
                CitizenIdBackImageUrl = null,
                DriverLicenseNumber = "790204013579",
                DriverLicenseFrontImageUrl = "/images/seed/gplx-front.svg",
                DriverLicenseBackImageUrl = "/images/seed/gplx-back.svg",
                VerificationStatus = DocumentVerificationStatus.Pending,
                CreatedAt = DateTime.UtcNow.AddDays(-2)
            });
        }

        if (!await db.DriverDocuments.AnyAsync(document => document.UserId == approvedCustomer.Id, ct))
        {
            db.DriverDocuments.Add(new DriverDocument
            {
                UserId = approvedCustomer.Id,
                CitizenIdNumber = "048198006248",
                CitizenIdFrontImageUrl = null,
                CitizenIdBackImageUrl = null,
                DriverLicenseNumber = "790198027461",
                DriverLicenseFrontImageUrl = "/images/seed/gplx-front.svg",
                DriverLicenseBackImageUrl = "/images/seed/gplx-back.svg",
                VerificationStatus = DocumentVerificationStatus.Approved,
                CreatedAt = DateTime.UtcNow.AddDays(-9),
                UpdatedAt = DateTime.UtcNow.AddDays(-8)
            });
        }

        if (!await db.DriverDocuments.AnyAsync(document => document.UserId == realImageCustomer.Id, ct))
        {
            db.DriverDocuments.Add(new DriverDocument
            {
                UserId = realImageCustomer.Id,
                CitizenIdNumber = "SEED-CITIZEN-0003",
                CitizenIdFrontImageUrl = null,
                CitizenIdBackImageUrl = null,
                DriverLicenseNumber = "SEED-GPLX-0003",
                DriverLicenseFrontImageUrl = "https://res.cloudinary.com/dtm5a4bwr/image/upload/v1784713351/z8069770352586_6c9c3b59a42a3bc40256c0a4b3586883_gkacdr.jpg",
                DriverLicenseBackImageUrl = null,
                VerificationStatus = DocumentVerificationStatus.Pending,
                CreatedAt = DateTime.UtcNow
            });
        }
        await db.SaveChangesAsync(ct);
    }
}
