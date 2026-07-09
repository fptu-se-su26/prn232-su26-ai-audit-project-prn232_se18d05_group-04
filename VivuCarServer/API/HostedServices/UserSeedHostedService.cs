using BusinessObjects.Data;
using BusinessObjects.Enums;
using BusinessObjects.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.HostedServices;

public class UserSeedHostedService(
    IServiceScopeFactory scopeFactory,
    IConfiguration configuration,
    ILogger<UserSeedHostedService> logger
) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<VivuCarDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<AppUser>>();

        await SeedUserAsync(
            dbContext,
            passwordHasher,
            "SEED_ADMIN",
            UserRole.Admin,
            "System Admin",
            "0900000001",
            cancellationToken
        );
        await SeedUserAsync(
            dbContext,
            passwordHasher,
            "SEED_CUSTOMER",
            UserRole.Customer,
            "Default Customer",
            "0900000002",
            cancellationToken
        );
        await SeedUserAsync(
            dbContext,
            passwordHasher,
            "SEED_CAR_OWNER",
            UserRole.CarOwner,
            "Default Car Owner",
            "0900000003",
            cancellationToken
        );

        await SeedCarTestDataAsync(dbContext, cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private async Task SeedUserAsync(
        VivuCarDbContext dbContext,
        IPasswordHasher<AppUser> passwordHasher,
        string configurationPrefix,
        UserRole role,
        string defaultFullName,
        string defaultPhoneNumber,
        CancellationToken cancellationToken
    )
    {
        var email = configuration[$"{configurationPrefix}_EMAIL"]?.Trim().ToLowerInvariant();
        var password = configuration[$"{configurationPrefix}_PASSWORD"];

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            logger.LogWarning(
                "{Role} seed account was skipped because {EmailKey} or {PasswordKey} is missing.",
                role,
                $"{configurationPrefix}_EMAIL",
                $"{configurationPrefix}_PASSWORD"
            );
            return;
        }

        var existingUser = await dbContext.Users.SingleOrDefaultAsync(
            user => user.Email == email,
            cancellationToken
        );

        if (existingUser is not null)
        {
            if (existingUser.PasswordHash.StartsWith("SeedPasswordHash_", StringComparison.Ordinal))
            {
                existingUser.PasswordHash = passwordHasher.HashPassword(existingUser, password);
                existingUser.UpdatedAt = DateTime.UtcNow;
                await dbContext.SaveChangesAsync(cancellationToken);

                logger.LogInformation(
                    "Legacy placeholder password hash was upgraded for {Email}.",
                    email
                );
            }

            return;
        }

        var user = new AppUser
        {
            Email = email,
            FullName =
                configuration[$"{configurationPrefix}_FULL_NAME"]?.Trim() ?? defaultFullName,
            PhoneNumber =
                configuration[$"{configurationPrefix}_PHONE_NUMBER"]?.Trim()
                ?? defaultPhoneNumber,
            Role = role,
            Status = UserStatus.Active,
            TokenVersion = 1,
            CreatedAt = DateTime.UtcNow,
        };

        user.PasswordHash = passwordHasher.HashPassword(user, password);

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Default {Role} account {Email} was seeded.", role, email);
    }

    private async Task SeedCarTestDataAsync(VivuCarDbContext dbContext, CancellationToken cancellationToken)
    {
        // 1. Get owner
        var owner = await dbContext.Users.FirstOrDefaultAsync(u => u.Role == UserRole.CarOwner, cancellationToken);
        if (owner == null) return;
        
        // 2. Add Brands if not exists
        var brands = new List<CarBrand>
        {
            new() { Name = "Honda", IsActive = true },
            new() { Name = "Hyundai", IsActive = true },
            new() { Name = "Kia", IsActive = true },
            new() { Name = "VinFast", IsActive = true },
            new() { Name = "Mazda", IsActive = true }
        };
        
        foreach (var brand in brands)
        {
            if (!await dbContext.CarBrands.AnyAsync(b => b.Name == brand.Name, cancellationToken))
            {
                dbContext.CarBrands.Add(brand);
            }
        }
        await dbContext.SaveChangesAsync(cancellationToken);
        
        // 3. Add Models if not exists
        var honda = await dbContext.CarBrands.FirstAsync(b => b.Name == "Honda", cancellationToken);
        var hyundai = await dbContext.CarBrands.FirstAsync(b => b.Name == "Hyundai", cancellationToken);
        var kia = await dbContext.CarBrands.FirstAsync(b => b.Name == "Kia", cancellationToken);
        var vinfast = await dbContext.CarBrands.FirstAsync(b => b.Name == "VinFast", cancellationToken);
        var mazda = await dbContext.CarBrands.FirstAsync(b => b.Name == "Mazda", cancellationToken);

        var models = new List<CarModel>
        {
            new() { CarBrandId = honda.Id, Name = "Civic", IsActive = true },
            new() { CarBrandId = honda.Id, Name = "City", IsActive = true },
            new() { CarBrandId = hyundai.Id, Name = "Accent", IsActive = true },
            new() { CarBrandId = hyundai.Id, Name = "Santa Fe", IsActive = true },
            new() { CarBrandId = kia.Id, Name = "Seltos", IsActive = true },
            new() { CarBrandId = vinfast.Id, Name = "VF8", IsActive = true },
            new() { CarBrandId = mazda.Id, Name = "CX-5", IsActive = true }
        };

        foreach (var model in models)
        {
            if (!await dbContext.CarModels.AnyAsync(m => m.Name == model.Name && m.CarBrandId == model.CarBrandId, cancellationToken))
            {
                dbContext.CarModels.Add(model);
            }
        }
        await dbContext.SaveChangesAsync(cancellationToken);

        // 4. Add Cars if we only have the default one
        var carCount = await dbContext.Cars.CountAsync(cancellationToken);
        if (carCount <= 1)
        {
            var modelCivic = await dbContext.CarModels.FirstAsync(m => m.Name == "Civic", cancellationToken);
            var modelAccent = await dbContext.CarModels.FirstAsync(m => m.Name == "Accent", cancellationToken);
            var modelSantaFe = await dbContext.CarModels.FirstAsync(m => m.Name == "Santa Fe", cancellationToken);
            var modelSeltos = await dbContext.CarModels.FirstAsync(m => m.Name == "Seltos", cancellationToken);
            var modelVF8 = await dbContext.CarModels.FirstAsync(m => m.Name == "VF8", cancellationToken);
            var modelCX5 = await dbContext.CarModels.FirstAsync(m => m.Name == "CX-5", cancellationToken);

            var newCars = new List<Car>
            {
                new()
                {
                    OwnerId = owner.Id,
                    CarBrandId = honda.Id,
                    CarModelId = modelCivic.Id,
                    Name = "Honda Civic 2023",
                    LicensePlate = "43A-56789",
                    Description = "Xe sedan thể thao, cảm giác lái bốc, nội thất hiện đại.",
                    Location = "Thanh Khê, Đà Nẵng",
                    DailyPrice = 800000m,
                    InsuranceFeePerDay = 60000m,
                    DeliveryFee = 20000m,
                    DepositAmount = 2000000m,
                    Status = CarStatus.Available,
                    SeatCount = 5,
                    TransmissionType = TransmissionType.Automatic,
                    FuelType = FuelType.Gasoline,
                    CreatedAt = DateTime.UtcNow.AddDays(-10)
                },
                new()
                {
                    OwnerId = owner.Id,
                    CarBrandId = hyundai.Id,
                    CarModelId = modelAccent.Id,
                    Name = "Hyundai Accent 2022",
                    LicensePlate = "43A-99911",
                    Description = "Xe đi kiểng, nội thất xe cực sạch sẽ và mới tinh.",
                    Location = "Sơn Trà, Đà Nẵng",
                    DailyPrice = 700000m,
                    InsuranceFeePerDay = 50000m,
                    DeliveryFee = 15000m,
                    DepositAmount = 1500000m,
                    Status = CarStatus.Available,
                    SeatCount = 5,
                    TransmissionType = TransmissionType.Automatic,
                    FuelType = FuelType.Gasoline,
                    CreatedAt = DateTime.UtcNow.AddDays(-8)
                },
                new()
                {
                    OwnerId = owner.Id,
                    CarBrandId = hyundai.Id,
                    CarModelId = modelSantaFe.Id,
                    Name = "Hyundai Santa Fe 2023",
                    LicensePlate = "43A-88888",
                    Description = "Dòng xe SUV 7 chỗ sang trọng, rộng rãi cho gia đình đi du lịch.",
                    Location = "Ngũ Hành Sơn, Đà Nẵng",
                    DailyPrice = 1500000m,
                    InsuranceFeePerDay = 100000m,
                    DeliveryFee = 50000m,
                    DepositAmount = 5000000m,
                    Status = CarStatus.Available,
                    SeatCount = 7,
                    TransmissionType = TransmissionType.Automatic,
                    FuelType = FuelType.Diesel,
                    CreatedAt = DateTime.UtcNow.AddDays(-15)
                },
                new()
                {
                    OwnerId = owner.Id,
                    CarBrandId = kia.Id,
                    CarModelId = modelSeltos.Id,
                    Name = "Kia Seltos 2021",
                    LicensePlate = "43A-22233",
                    Description = "Xe gầm cao 5 chỗ đô thị năng động, thời trang.",
                    Location = "Liên Chiểu, Đà Nẵng",
                    DailyPrice = 900000m,
                    InsuranceFeePerDay = 70000m,
                    DeliveryFee = 30000m,
                    DepositAmount = 2000000m,
                    Status = CarStatus.Available,
                    SeatCount = 5,
                    TransmissionType = TransmissionType.Automatic,
                    FuelType = FuelType.Gasoline,
                    CreatedAt = DateTime.UtcNow.AddDays(-5)
                },
                new()
                {
                    OwnerId = owner.Id,
                    CarBrandId = vinfast.Id,
                    CarModelId = modelVF8.Id,
                    Name = "VinFast VF8 2023",
                    LicensePlate = "43A-66677",
                    Description = "Xe điện VinFast thông minh, tăng tốc nhanh, bảo vệ môi trường.",
                    Location = "Hải Châu, Đà Nẵng",
                    DailyPrice = 1200000m,
                    InsuranceFeePerDay = 90000m,
                    DeliveryFee = 40000m,
                    DepositAmount = 3000000m,
                    Status = CarStatus.Available,
                    SeatCount = 5,
                    TransmissionType = TransmissionType.Automatic,
                    FuelType = FuelType.Electric,
                    CreatedAt = DateTime.UtcNow.AddDays(-3)
                },
                new()
                {
                    OwnerId = owner.Id,
                    CarBrandId = mazda.Id,
                    CarModelId = modelCX5.Id,
                    Name = "Mazda CX-5 2022",
                    LicensePlate = "43A-77788",
                    Description = "Kiểu dáng Kodo sang trọng, nội thất bọc da êm ái.",
                    Location = "Cẩm Lệ, Đà Nẵng",
                    DailyPrice = 1100000m,
                    InsuranceFeePerDay = 80000m,
                    DeliveryFee = 35000m,
                    DepositAmount = 2500000m,
                    Status = CarStatus.Available,
                    SeatCount = 5,
                    TransmissionType = TransmissionType.Automatic,
                    FuelType = FuelType.Gasoline,
                    CreatedAt = DateTime.UtcNow.AddDays(-12)
                }
            };

            dbContext.Cars.AddRange(newCars);
            await dbContext.SaveChangesAsync(cancellationToken);

            // Add images and reviews
            var seededCars = await dbContext.Cars.Where(c => c.Id > 1).ToListAsync(cancellationToken);
            var customer = await dbContext.Users.FirstOrDefaultAsync(u => u.Role == UserRole.Customer, cancellationToken);
            
            foreach (var car in seededCars)
            {
                // Add placeholder images
                dbContext.CarImages.Add(new CarImage
                {
                    CarId = car.Id,
                    ImageUrl = "https://images.unsplash.com/photo-1549399542-7e3f8b79c341?q=80&w=400&auto=format&fit=crop",
                    IsPrimary = true,
                    DisplayOrder = 1
                });

                // Add a completed booking for this car to allow reviewing it
                if (customer != null)
                {
                    var booking = new Booking
                    {
                        BookingCode = $"BC-{Guid.NewGuid().ToString("N")[..8].ToUpper()}",
                        CustomerId = customer.Id,
                        CarId = car.Id,
                        StartDateTime = DateTime.UtcNow.AddDays(-5),
                        EndDateTime = DateTime.UtcNow.AddDays(-2),
                        PickupLocation = car.Location,
                        ReturnLocation = car.Location,
                        BasePrice = car.DailyPrice * 3,
                        InsuranceFee = car.InsuranceFeePerDay * 3,
                        DeliveryFee = car.DeliveryFee,
                        DiscountAmount = 0m,
                        DepositAmount = car.DepositAmount,
                        TotalAmount = (car.DailyPrice + car.InsuranceFeePerDay) * 3 + car.DeliveryFee,
                        RemainingAmount = 0m,
                        Status = BookingStatus.Completed,
                        CreatedAt = DateTime.UtcNow.AddDays(-6)
                    };
                    
                    dbContext.Bookings.Add(booking);
                    await dbContext.SaveChangesAsync(cancellationToken);

                    var rating = car.Name.Contains("Santa Fe") || car.Name.Contains("VF8") ? 5 : 4;
                    dbContext.Reviews.Add(new Review
                    {
                        CarId = car.Id,
                        CustomerId = customer.Id,
                        BookingId = booking.Id, // Referencing the newly created Booking Id
                        Rating = rating,
                        Comment = $"Xe chạy rất tốt, sạch sẽ và chủ xe {owner.FullName} siêu nhiệt tình!",
                        CreatedAt = DateTime.UtcNow.AddDays(-2)
                    });
                }
            }
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        else
        {
            // Backfill bookings and reviews if cars exist but reviews do not
            var reviewCount = await dbContext.Reviews.CountAsync(cancellationToken);
            if (reviewCount == 0)
            {
                var seededCars = await dbContext.Cars.Where(c => c.Id > 1).ToListAsync(cancellationToken);
                var customer = await dbContext.Users.FirstOrDefaultAsync(u => u.Role == UserRole.Customer, cancellationToken);
                
                foreach (var car in seededCars)
                {
                    if (customer != null)
                    {
                        var booking = new Booking
                        {
                            BookingCode = $"BC-{Guid.NewGuid().ToString("N")[..8].ToUpper()}",
                            CustomerId = customer.Id,
                            CarId = car.Id,
                            StartDateTime = DateTime.UtcNow.AddDays(-5),
                            EndDateTime = DateTime.UtcNow.AddDays(-2),
                            PickupLocation = car.Location,
                            ReturnLocation = car.Location,
                            BasePrice = car.DailyPrice * 3,
                            InsuranceFee = car.InsuranceFeePerDay * 3,
                            DeliveryFee = car.DeliveryFee,
                            DiscountAmount = 0m,
                            DepositAmount = car.DepositAmount,
                            TotalAmount = (car.DailyPrice + car.InsuranceFeePerDay) * 3 + car.DeliveryFee,
                            RemainingAmount = 0m,
                            Status = BookingStatus.Completed,
                            CreatedAt = DateTime.UtcNow.AddDays(-6)
                        };
                        
                        dbContext.Bookings.Add(booking);
                        await dbContext.SaveChangesAsync(cancellationToken);

                        var rating = car.Name.Contains("Santa Fe") || car.Name.Contains("VF8") ? 5 : 4;
                        dbContext.Reviews.Add(new Review
                        {
                            CarId = car.Id,
                            CustomerId = customer.Id,
                            BookingId = booking.Id,
                            Rating = rating,
                            Comment = $"Xe chạy rất tốt, sạch sẽ và chủ xe {owner.FullName} siêu nhiệt tình!",
                            CreatedAt = DateTime.UtcNow.AddDays(-2)
                        });
                    }
                }
                await dbContext.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
