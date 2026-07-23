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
        var qualifiedCars = payload.Cars
            .Select(car =>
            {
                car.Images = car.Images
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();
                return car;
            })
            .Where(car => car.Images.Count >= 3)
            .ToList();
        var maxCars = configuration.GetValue("MiotoSeed:MaxCars", qualifiedCars.Count);
        var seededCount = 0;
        var updatedCount = 0;
        var rejectedCount = payload.Cars.Count - qualifiedCars.Count;

        foreach (var seedCar in qualifiedCars.Take(maxCars))
        {
            var brand = await EnsureBrandAsync(dbContext, seedCar.Brand, cancellationToken);
            var model = await EnsureModelAsync(dbContext, brand, seedCar.Model, cancellationToken);
            var type = await EnsureTypeAsync(dbContext, seedCar.CarType, cancellationToken);
            var car = await dbContext.Cars
                .Include(item => item.Images)
                .SingleOrDefaultAsync(
                    item => item.LicensePlate == seedCar.LicensePlate,
                    cancellationToken
                );

            if (car is null)
            {
                car = new Car
                {
                    LicensePlate = seedCar.LicensePlate,
                    CreatedAt = DateTime.UtcNow
                };
                dbContext.Cars.Add(car);
                seededCount++;
            }
            else
            {
                updatedCount++;
            }

            car.OwnerId = owner.Id;
            car.CarBrandId = brand.Id;
            car.CarModelId = model.Id;
            car.CarTypeId = type.Id;
            car.Name = seedCar.Title;
            car.Year = seedCar.Year.HasValue ? (short?)seedCar.Year.Value : null;
            car.Color = null;
            car.KilometersDriven = Math.Max(0, seedCar.KilometersDriven);
            car.Description = seedCar.Description;
            car.Location = seedCar.Address;
            car.DailyPrice = seedCar.PricePerDay;
            car.PricePerHour = seedCar.PricePerHours;
            car.InsuranceFeePerDay = 0;
            car.DeliveryFee = 0;
            car.DepositAmount = 0;
            car.Status = ParseStatus(seedCar.Status);
            car.SeatCount = seedCar.Seats;
            car.TransmissionType = ParseTransmission(seedCar.Transmission);
            car.FuelType = ParseFuel(seedCar.FuelType);

            await dbContext.SaveChangesAsync(cancellationToken);
            await SeedImagesAsync(dbContext, car, seedCar, dataRoot, cancellationToken);
        }

        logger.LogInformation(
            "Mioto car seed completed. Seeded {SeededCount}, updated {UpdatedCount}, rejected {RejectedCount} cars with fewer than 3 distinct image paths.",
            seededCount,
            updatedCount,
            rejectedCount
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

        var desiredUrls = new List<string>();
        foreach (var relativeImagePath in seedCar.Images)
        {
            var sourcePath = Path.GetFullPath(
                Path.Combine(
                    dataRoot,
                    relativeImagePath.Replace('/', Path.DirectorySeparatorChar)
                )
            );
            var fileName = Path.GetFileName(relativeImagePath);
            var destinationPath = Path.Combine(destinationFolder, fileName);
            if (File.Exists(sourcePath))
            {
                File.Copy(sourcePath, destinationPath, overwrite: true);
            }
            else if (!File.Exists(destinationPath))
            {
                logger.LogWarning(
                    "Seed image was skipped because both source and packaged files are missing: {SourcePath}.",
                    sourcePath
                );
                continue;
            }

            desiredUrls.Add(
                $"/uploads/seed/cars/{car.LicensePlate.Replace("-", "")}/{fileName}"
            );
        }

        desiredUrls = desiredUrls
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
        if (desiredUrls.Count < 3)
        {
            throw new InvalidOperationException(
                $"Mioto seed car '{seedCar.LicensePlate}' must resolve to at least 3 distinct images."
            );
        }

        var existingImages = car.Images.ToList();
        foreach (var duplicate in existingImages
            .GroupBy(image => image.ImageUrl, StringComparer.OrdinalIgnoreCase)
            .SelectMany(group => group.Skip(1))
            .ToList())
        {
            dbContext.CarImages.Remove(duplicate);
            existingImages.Remove(duplicate);
        }

        var desiredUrlSet = desiredUrls.ToHashSet(StringComparer.OrdinalIgnoreCase);
        foreach (var staleImage in existingImages
            .Where(image => !desiredUrlSet.Contains(image.ImageUrl))
            .ToList())
        {
            dbContext.CarImages.Remove(staleImage);
            existingImages.Remove(staleImage);
        }

        for (var index = 0; index < desiredUrls.Count; index++)
        {
            var publicUrl = desiredUrls[index];
            var image = existingImages.FirstOrDefault(item =>
                string.Equals(item.ImageUrl, publicUrl, StringComparison.OrdinalIgnoreCase)
            );
            if (image is null)
            {
                image = new CarImage
                {
                    CarId = car.Id,
                    ImageUrl = publicUrl
                };
                dbContext.CarImages.Add(image);
                existingImages.Add(image);
            }

            image.IsPrimary = index == 0;
            image.DisplayOrder = index + 1;
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
