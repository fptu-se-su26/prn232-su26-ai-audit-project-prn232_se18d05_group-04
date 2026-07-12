using System.Text.Json;
using System.Text.Json.Serialization;
using BusinessObjects.Data;
using BusinessObjects.Enums;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;

namespace API.HostedServices;

public class MiotoCarSeedHostedService(
    IServiceScopeFactory scopeFactory,
    IWebHostEnvironment environment,
    IConfiguration configuration,
    ILogger<MiotoCarSeedHostedService> logger
) : IHostedService
{
    private const string SeedOwnerEmail = "owner02@vivucar.local";
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (!configuration.GetValue("MiotoSeed:Enabled", false))
        {
            logger.LogInformation("Mioto car seed skipped because MiotoSeed:Enabled is false.");
            return;
        }

        var seedFile = ResolveSeedFilePath();
        if (!File.Exists(seedFile))
        {
            logger.LogWarning("Mioto car seed skipped because seed file was not found: {SeedFile}.", seedFile);
            return;
        }

        var payload = await ReadSeedPayloadAsync(seedFile, cancellationToken);
        if (payload.Cars.Count == 0)
        {
            logger.LogWarning("Mioto car seed skipped because seed file has no cars: {SeedFile}.", seedFile);
            return;
        }

        await using var scope = scopeFactory.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<VivuCarDbContext>();
        var owner = await EnsureSeedOwnerAsync(dbContext, cancellationToken);
        var dataRoot = ResolveDataRoot(payload);
        var maxCars = configuration.GetValue("MiotoSeed:MaxCars", payload.Cars.Count);
        var seededCount = 0;
        var skippedCount = 0;

        foreach (var seedCar in payload.Cars.Take(maxCars))
        {
            if (await dbContext.Cars.AnyAsync(car => car.LicensePlate == seedCar.LicensePlate, cancellationToken))
            {
                skippedCount++;
                continue;
            }

            var brand = await EnsureBrandAsync(dbContext, seedCar.Brand, cancellationToken);
            var model = await EnsureModelAsync(dbContext, brand, seedCar.Model, cancellationToken);
            var type = await EnsureTypeAsync(dbContext, seedCar.CarType, cancellationToken);

            var car = new Car
            {
                OwnerId = owner.Id,
                CarBrandId = brand.Id,
                CarModelId = model.Id,
                CarTypeId = type.Id,
                Name = seedCar.Title,
                LicensePlate = seedCar.LicensePlate,
                Year = seedCar.Year.HasValue ? (short?)seedCar.Year.Value : null,
                Color = null,
                KilometersDriven = Math.Max(0, seedCar.KilometersDriven),
                Description = seedCar.Description,
                Location = seedCar.Address,
                DailyPrice = seedCar.PricePerDay,
                PricePerHour = seedCar.PricePerHours,
                InsuranceFeePerDay = 0,
                DeliveryFee = 0,
                DepositAmount = 0,
                Status = ParseStatus(seedCar.Status),
                SeatCount = seedCar.Seats,
                TransmissionType = ParseTransmission(seedCar.Transmission),
                FuelType = ParseFuel(seedCar.FuelType),
                CreatedAt = DateTime.UtcNow
            };

            dbContext.Cars.Add(car);
            await dbContext.SaveChangesAsync(cancellationToken);

            await SeedImagesAsync(dbContext, car, seedCar, dataRoot, cancellationToken);
            seededCount++;
        }

        logger.LogInformation(
            "Mioto car seed completed. Seeded {SeededCount} cars, skipped {SkippedCount} existing cars.",
            seededCount,
            skippedCount
        );
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private string ResolveSeedFilePath()
    {
        var configured = configuration["MiotoSeed:SeedFile"];
        if (!string.IsNullOrWhiteSpace(configured))
        {
            return Path.GetFullPath(configured);
        }

        return Path.Combine(environment.ContentRootPath, "SeedData", "mioto-cars.seed.json");
    }

    private string ResolveDataRoot(MiotoSeedPayload payload)
    {
        var configured = configuration["MiotoSeed:DataRoot"];
        if (!string.IsNullOrWhiteSpace(configured))
        {
            return Path.GetFullPath(configured);
        }

        return string.IsNullOrWhiteSpace(payload.Source)
            ? environment.ContentRootPath
            : Path.GetDirectoryName(Path.GetFullPath(payload.Source)) ?? environment.ContentRootPath;
    }

    private static async Task<MiotoSeedPayload> ReadSeedPayloadAsync(string seedFile, CancellationToken cancellationToken)
    {
        await using var stream = File.OpenRead(seedFile);
        return await JsonSerializer.DeserializeAsync<MiotoSeedPayload>(stream, JsonOptions, cancellationToken)
            ?? new MiotoSeedPayload();
    }

    private static async Task<AppUser> EnsureSeedOwnerAsync(VivuCarDbContext dbContext, CancellationToken cancellationToken)
    {
        var owner = await dbContext.Users.SingleOrDefaultAsync(user => user.Email == SeedOwnerEmail, cancellationToken);
        if (owner is not null)
        {
            return owner;
        }

        owner = new AppUser
        {
            Email = SeedOwnerEmail,
            PasswordHash = "MiotoSeedOwner_NoInteractiveLogin",
            FullName = "Mioto Seed Owner",
            PhoneNumber = "0900000999",
            Role = UserRole.CarOwner,
            Status = UserStatus.Active,
            TokenVersion = 1,
            CreatedAt = DateTime.UtcNow
        };
        dbContext.Users.Add(owner);
        await dbContext.SaveChangesAsync(cancellationToken);
        return owner;
    }

    private static async Task<CarBrand> EnsureBrandAsync(VivuCarDbContext dbContext, string brandName, CancellationToken cancellationToken)
    {
        var normalized = brandName.Trim();
        var brand = await dbContext.CarBrands.SingleOrDefaultAsync(item => item.Name == normalized, cancellationToken);
        if (brand is not null) return brand;

        brand = new CarBrand { Name = normalized, IsActive = true };
        dbContext.CarBrands.Add(brand);
        await dbContext.SaveChangesAsync(cancellationToken);
        return brand;
    }

    private static async Task<CarModel> EnsureModelAsync(VivuCarDbContext dbContext, CarBrand brand, string modelName, CancellationToken cancellationToken)
    {
        var normalized = modelName.Trim();
        var model = await dbContext.CarModels.SingleOrDefaultAsync(
            item => item.CarBrandId == brand.Id && item.Name == normalized,
            cancellationToken
        );
        if (model is not null) return model;

        model = new CarModel { CarBrandId = brand.Id, Name = normalized, IsActive = true };
        dbContext.CarModels.Add(model);
        await dbContext.SaveChangesAsync(cancellationToken);
        return model;
    }

    private static async Task<CarType> EnsureTypeAsync(VivuCarDbContext dbContext, string typeName, CancellationToken cancellationToken)
    {
        var normalized = typeName.Trim();
        var type = await dbContext.CarTypes.SingleOrDefaultAsync(item => item.Name == normalized, cancellationToken);
        if (type is not null) return type;

        type = new CarType { Name = normalized, IsActive = true };
        dbContext.CarTypes.Add(type);
        await dbContext.SaveChangesAsync(cancellationToken);
        return type;
    }

    private async Task SeedImagesAsync(
        VivuCarDbContext dbContext,
        Car car,
        MiotoSeedCar seedCar,
        string dataRoot,
        CancellationToken cancellationToken
    )
    {
        var webRoot = environment.WebRootPath ?? Path.Combine(environment.ContentRootPath, "wwwroot");
        var destinationFolder = Path.Combine(webRoot, "uploads", "seed", "cars", car.LicensePlate.Replace("-", ""));
        Directory.CreateDirectory(destinationFolder);

        var displayOrder = 1;
        foreach (var relativeImagePath in seedCar.Images)
        {
            var sourcePath = Path.GetFullPath(Path.Combine(dataRoot, relativeImagePath.Replace('/', Path.DirectorySeparatorChar)));
            if (!File.Exists(sourcePath))
            {
                logger.LogWarning("Seed image was skipped because source file was missing: {SourcePath}.", sourcePath);
                continue;
            }

            var extension = Path.GetExtension(sourcePath);
            var fileName = $"{displayOrder:00}{extension}";
            var destinationPath = Path.Combine(destinationFolder, fileName);
            if (!File.Exists(destinationPath))
            {
                File.Copy(sourcePath, destinationPath);
            }

            var publicUrl = $"/uploads/seed/cars/{car.LicensePlate.Replace("-", "")}/{fileName}";
            dbContext.CarImages.Add(new CarImage
            {
                CarId = car.Id,
                ImageUrl = publicUrl,
                IsPrimary = displayOrder == 1,
                DisplayOrder = displayOrder
            });
            displayOrder++;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static CarStatus ParseStatus(string value) => value.Trim().ToLowerInvariant() switch
    {
        "rented" => CarStatus.Rented,
        "maintenance" => CarStatus.Maintenance,
        "blocked" => CarStatus.Blocked,
        "unavailable" => CarStatus.Unavailable,
        _ => CarStatus.Available
    };

    private static TransmissionType ParseTransmission(string value) => value.Trim().ToLowerInvariant() switch
    {
        "manual" => TransmissionType.Manual,
        "cvt" => TransmissionType.Cvt,
        _ => TransmissionType.Automatic
    };

    private static FuelType ParseFuel(string value) => value.Trim().ToLowerInvariant() switch
    {
        "diesel" => FuelType.Diesel,
        "electric" => FuelType.Electric,
        "hybrid" => FuelType.Hybrid,
        _ => FuelType.Gasoline
    };

    private sealed class MiotoSeedPayload
    {
        public string? Source { get; set; }
        public List<MiotoSeedCar> Cars { get; set; } = [];
    }

    private sealed class MiotoSeedCar
    {
        [JsonPropertyName("source_order")]
        public string SourceOrder { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int? Year { get; set; }
        [JsonPropertyName("car_type")]
        public string CarType { get; set; } = "Sedan";
        [JsonPropertyName("license_plate")]
        public string LicensePlate { get; set; } = string.Empty;
        public int Seats { get; set; }
        [JsonPropertyName("kilometers_driven")]
        public int KilometersDriven { get; set; }
        public string Transmission { get; set; } = "automatic";
        [JsonPropertyName("fuel_type")]
        public string FuelType { get; set; } = "gasoline";
        [JsonPropertyName("price_per_day")]
        public decimal PricePerDay { get; set; }
        [JsonPropertyName("price_per_hours")]
        public decimal PricePerHours { get; set; }
        public string Address { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = "available";
        public List<string> Images { get; set; } = [];
    }
}
