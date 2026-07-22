using System.Globalization;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using BusinessObjects.Enums;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;

namespace BusinessObjects.Data.Seed;

public sealed record CarSeedResult(
    int CarsAdded,
    int ImagesAdded,
    IReadOnlyList<Car> Cars
);

public sealed record CarCatalogSyncResult(
    int CarsUpdated,
    int ImagesAdded,
    int ImagesRemoved
);

public static class CarSeed
{
    public const int CarCount = 30;
    public const int MinimumImagesPerCar = 3;
    private const string CloudinaryManifestName = "cloudinary-car-images.json";
    private static readonly DateTime SeedCreatedAt = new(2026, 7, 1, 0, 0, 0, DateTimeKind.Utc);
    private static readonly string[] Colors = ["White", "Black", "Silver", "Gray", "Blue", "Red"];
    private static readonly string[] Locations =
    [
        "Hai Chau, Da Nang",
        "Son Tra, Da Nang",
        "Thanh Khe, Da Nang",
        "Ngu Hanh Son, Da Nang",
        "Cam Le, Da Nang"
    ];

    public static async Task<CarSeedResult> SeedAsync(
        VivuCarDbContext dbContext,
        CancellationToken cancellationToken = default
    )
    {
        var owners = await dbContext.Users
            .Where(user => user.Role == UserRole.CarOwner)
            .OrderBy(user => user.Id)
            .Take(UserSeed.CarOwnerCount)
            .ToListAsync(cancellationToken);
        if (owners.Count < UserSeed.CarOwnerCount)
        {
            throw new InvalidOperationException(
                $"CarSeed requires {UserSeed.CarOwnerCount} car owners, but only {owners.Count} were found."
            );
        }

        var manifest = await LoadCloudinaryManifestAsync(cancellationToken);
        var cloudCars = manifest.Cars
            .OrderBy(car => car.SourceOrder)
            .Take(CarCount)
            .ToList();
        if (cloudCars.Count != CarCount)
        {
            throw new InvalidOperationException(
                $"Cloudinary manifest must contain at least {CarCount} cars. Found {cloudCars.Count}."
            );
        }

        ValidateCatalogCars(cloudCars);

        var definitions = cloudCars
            .Select((car, index) => BuildDefinition(car, index))
            .ToList();
        var brands = await EnsureBrandsAsync(dbContext, definitions, cancellationToken);
        var types = await EnsureTypesAsync(dbContext, definitions, cancellationToken);
        var models = await EnsureModelsAsync(dbContext, definitions, brands, cancellationToken);
        var plates = definitions.Select(car => car.LicensePlate).ToArray();
        var existingCars = await dbContext.Cars
            .Where(car => plates.Contains(car.LicensePlate))
            .ToDictionaryAsync(car => car.LicensePlate, StringComparer.OrdinalIgnoreCase, cancellationToken);

        var carsAdded = 0;
        for (var index = 0; index < definitions.Count; index++)
        {
            var seed = definitions[index];
            var owner = owners[index % owners.Count];
            if (!existingCars.TryGetValue(seed.LicensePlate, out var car))
            {
                car = new Car { LicensePlate = seed.LicensePlate, CreatedAt = SeedCreatedAt.AddMinutes(index) };
                dbContext.Cars.Add(car);
                existingCars.Add(seed.LicensePlate, car);
                carsAdded++;
            }

            car.OwnerId = owner.Id;
            car.CarBrandId = brands[seed.Brand].Id;
            car.CarModelId = models[ModelKey(seed.Brand, seed.Model)].Id;
            car.CarTypeId = types[seed.Type].Id;
            car.Name = seed.Name;
            car.Year = seed.Year;
            car.Color = seed.Color;
            car.KilometersDriven = seed.KilometersDriven;
            car.Description = seed.Description;
            car.Location = seed.Location;
            car.DailyPrice = seed.DailyPrice;
            car.PricePerHour = decimal.Round(seed.DailyPrice / 10m, 0);
            car.InsuranceFeePerDay = decimal.Round(seed.DailyPrice * 0.07m, 0);
            car.DeliveryFee = 30000m + index % 3 * 10000m;
            car.DepositAmount = seed.DailyPrice >= 1000000m ? 5000000m : 3000000m;
            car.Status = seed.Status;
            car.PreviousStatus = seed.Status is CarStatus.Blocked or CarStatus.Maintenance
                ? CarStatus.Available
                : null;
            car.BlockedReason = seed.BlockedReason;
            car.SeatCount = seed.SeatCount;
            car.TransmissionType = seed.Transmission;
            car.FuelType = seed.Fuel;
        }
        await dbContext.SaveChangesAsync(cancellationToken);

        var seededCars = definitions.Select(definition => existingCars[definition.LicensePlate]).ToList();
        var seededCarIds = seededCars.Select(car => car.Id).ToArray();
        var existingImages = await dbContext.CarImages
            .Where(image => seededCarIds.Contains(image.CarId))
            .ToListAsync(cancellationToken);
        var imagesAdded = 0;

        for (var index = 0; index < cloudCars.Count; index++)
        {
            var car = seededCars[index];
            var carImages = existingImages.Where(image => image.CarId == car.Id).ToList();
            var galleryResult = SynchronizeGallery(dbContext, car, carImages, cloudCars[index]);
            imagesAdded += galleryResult.ImagesAdded;
        }
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CarSeedResult(carsAdded, imagesAdded, seededCars);
    }

    public static async Task<CarCatalogSyncResult> SynchronizeCarsFromCatalogAsync(
        VivuCarDbContext dbContext,
        IReadOnlyDictionary<string, string> imageSlugByLicensePlate,
        CancellationToken cancellationToken = default
    )
    {
        if (imageSlugByLicensePlate.Count == 0)
        {
            return new CarCatalogSyncResult(0, 0, 0);
        }

        var duplicateSlugs = imageSlugByLicensePlate.Values
            .GroupBy(slug => slug, StringComparer.OrdinalIgnoreCase)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key)
            .ToArray();
        if (duplicateSlugs.Length > 0)
        {
            throw new InvalidOperationException(
                $"Each synchronized car must use a distinct catalog entry. Duplicates: {string.Join(", ", duplicateSlugs)}."
            );
        }

        var manifest = await LoadCloudinaryManifestAsync(cancellationToken);
        var orderedCatalog = manifest.Cars.OrderBy(car => car.SourceOrder).ToList();
        var cloudCarsBySlug = orderedCatalog.ToDictionary(
            car => car.Slug,
            StringComparer.OrdinalIgnoreCase
        );
        var missingSlugs = imageSlugByLicensePlate.Values
            .Where(slug => !cloudCarsBySlug.ContainsKey(slug))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
        if (missingSlugs.Length > 0)
        {
            throw new InvalidOperationException(
                $"Cloudinary manifest does not contain: {string.Join(", ", missingSlugs)}."
            );
        }

        var selectedCatalogCars = imageSlugByLicensePlate.Values
            .Select(slug => cloudCarsBySlug[slug])
            .ToList();
        ValidateCatalogCars(selectedCatalogCars);

        var licensePlates = imageSlugByLicensePlate.Keys.ToArray();
        var cars = await dbContext.Cars
            .Include(car => car.Images)
            .Where(car => licensePlates.Contains(car.LicensePlate))
            .ToListAsync(cancellationToken);
        var definitionsBySlug = selectedCatalogCars.ToDictionary(
            cloudCar => cloudCar.Slug,
            cloudCar => BuildDefinition(cloudCar, orderedCatalog.IndexOf(cloudCar)),
            StringComparer.OrdinalIgnoreCase
        );
        var definitions = definitionsBySlug.Values.ToList();
        var brands = await EnsureBrandsAsync(dbContext, definitions, cancellationToken);
        var types = await EnsureTypesAsync(dbContext, definitions, cancellationToken);
        var models = await EnsureModelsAsync(dbContext, definitions, brands, cancellationToken);
        var imagesAdded = 0;
        var imagesRemoved = 0;

        foreach (var car in cars)
        {
            var slug = imageSlugByLicensePlate[car.LicensePlate];
            var cloudCar = cloudCarsBySlug[slug];
            var definition = definitionsBySlug[slug];

            // These license plates belong to demo seed data. Keep owner and operational
            // status, but make every descriptive field match the selected image catalog.
            car.CarBrandId = brands[definition.Brand].Id;
            car.CarModelId = models[ModelKey(definition.Brand, definition.Model)].Id;
            car.CarTypeId = types[definition.Type].Id;
            car.Name = definition.Name;
            car.Year = definition.Year;
            car.Color = definition.Color;
            car.KilometersDriven = definition.KilometersDriven;
            car.Description = definition.Description;
            car.Location = definition.Location;
            car.DailyPrice = definition.DailyPrice;
            car.PricePerHour = decimal.Round(definition.DailyPrice / 10m, 0);
            car.InsuranceFeePerDay = decimal.Round(definition.DailyPrice * 0.07m, 0);
            car.DeliveryFee = 30000m;
            car.DepositAmount = definition.DailyPrice >= 1000000m ? 5000000m : 3000000m;
            car.SeatCount = definition.SeatCount;
            car.TransmissionType = definition.Transmission;
            car.FuelType = definition.Fuel;

            var galleryResult = SynchronizeGallery(dbContext, car, car.Images.ToList(), cloudCar);
            imagesAdded += galleryResult.ImagesAdded;
            imagesRemoved += galleryResult.ImagesRemoved;
        }

        if (cars.Count > 0)
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        return new CarCatalogSyncResult(cars.Count, imagesAdded, imagesRemoved);
    }

    private static (int ImagesAdded, int ImagesRemoved) SynchronizeGallery(
        VivuCarDbContext dbContext,
        Car car,
        List<CarImage> existingImages,
        CloudinaryCar cloudCar
    )
    {
        var desiredImages = cloudCar.Images
            .OrderBy(image => image.DisplayOrder)
            .GroupBy(image => image.Url, StringComparer.OrdinalIgnoreCase)
            .Select(group => group.First())
            .ToList();
        var desiredUrls = desiredImages
            .Select(image => image.Url)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        var imagesRemoved = 0;

        foreach (var duplicate in existingImages
            .GroupBy(image => image.ImageUrl, StringComparer.OrdinalIgnoreCase)
            .SelectMany(group => group.Skip(1))
            .ToList())
        {
            dbContext.CarImages.Remove(duplicate);
            existingImages.Remove(duplicate);
            imagesRemoved++;
        }

        foreach (var staleImage in existingImages
            .Where(image => !desiredUrls.Contains(image.ImageUrl))
            .ToList())
        {
            dbContext.CarImages.Remove(staleImage);
            existingImages.Remove(staleImage);
            imagesRemoved++;
        }

        foreach (var image in existingImages)
        {
            image.IsPrimary = false;
        }

        var imagesAdded = 0;
        for (var index = 0; index < desiredImages.Count; index++)
        {
            var cloudImage = desiredImages[index];
            var image = existingImages.FirstOrDefault(item =>
                string.Equals(item.ImageUrl, cloudImage.Url, StringComparison.OrdinalIgnoreCase)
            );
            if (image is null)
            {
                image = new CarImage { CarId = car.Id, ImageUrl = cloudImage.Url };
                dbContext.CarImages.Add(image);
                existingImages.Add(image);
                imagesAdded++;
            }

            image.DisplayOrder = index + 1;
            image.IsPrimary = index == 0;
        }

        return (imagesAdded, imagesRemoved);
    }

    private static void ValidateCatalogCars(IReadOnlyCollection<CloudinaryCar> cloudCars)
    {
        var invalidCars = cloudCars
            .Where(car => car.Images
                .Select(image => image.Url)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Count() < MinimumImagesPerCar)
            .Select(car => car.Slug)
            .ToArray();
        if (invalidCars.Length > 0)
        {
            throw new InvalidOperationException(
                $"Every catalog car must have at least {MinimumImagesPerCar} distinct images. Invalid: {string.Join(", ", invalidCars)}."
            );
        }
    }
    private static CarDefinition BuildDefinition(CloudinaryCar cloudCar, int index)
    {
        var parts = cloudCar.Slug.Split('-', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length < 3 || !short.TryParse(parts[^1], out var year))
        {
            throw new InvalidOperationException($"Cannot derive car metadata from slug '{cloudCar.Slug}'.");
        }

        var brand = BrandName(parts[0]);
        var model = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(string.Join(' ', parts[1..^1]));
        var type = ResolveType(cloudCar.Slug);
        var dailyPrice = 480000m + index % 10 * 85000m + Math.Max(0, year - 2020) * 25000m;
        var status = index switch
        {
            4 => CarStatus.Maintenance,
            12 => CarStatus.Blocked,
            _ => CarStatus.Available
        };

        return new CarDefinition(
            cloudCar.Slug,
            brand,
            model,
            type,
            $"{brand} {model} {year}",
            LicensePlate(cloudCar.CarId, type),
            year,
            Colors[index % Colors.Length],
            12000 + index * 4700,
            ResolveSeatCount(type, cloudCar.Slug),
            ResolveTransmission(cloudCar.Slug),
            ResolveFuel(cloudCar.Slug),
            dailyPrice,
            status,
            $"Seed vehicle sourced from the Cloudinary catalog for {brand} {model}.",
            status switch
            {
                CarStatus.Maintenance => "Scheduled maintenance",
                CarStatus.Blocked => "Pending document re-verification",
                _ => null
            },
            Locations[index % Locations.Length]
        );
    }

    private static async Task<Dictionary<string, CarBrand>> EnsureBrandsAsync(
        VivuCarDbContext dbContext,
        IReadOnlyList<CarDefinition> definitions,
        CancellationToken cancellationToken
    )
    {
        var brands = await dbContext.CarBrands.ToDictionaryAsync(
            brand => brand.Name,
            StringComparer.OrdinalIgnoreCase,
            cancellationToken
        );
        foreach (var name in definitions.Select(car => car.Brand).Distinct(StringComparer.OrdinalIgnoreCase))
        {
            if (brands.ContainsKey(name)) continue;
            var brand = new CarBrand { Name = name, IsActive = true };
            dbContext.CarBrands.Add(brand);
            brands.Add(name, brand);
        }
        await dbContext.SaveChangesAsync(cancellationToken);
        return brands;
    }

    private static async Task<Dictionary<string, CarType>> EnsureTypesAsync(
        VivuCarDbContext dbContext,
        IReadOnlyList<CarDefinition> definitions,
        CancellationToken cancellationToken
    )
    {
        var types = await dbContext.CarTypes.ToDictionaryAsync(
            type => type.Name,
            StringComparer.OrdinalIgnoreCase,
            cancellationToken
        );
        foreach (var name in definitions.Select(car => car.Type).Distinct(StringComparer.OrdinalIgnoreCase))
        {
            if (types.ContainsKey(name)) continue;
            var type = new CarType { Name = name, IsActive = true };
            dbContext.CarTypes.Add(type);
            types.Add(name, type);
        }
        await dbContext.SaveChangesAsync(cancellationToken);
        return types;
    }

    private static async Task<Dictionary<string, CarModel>> EnsureModelsAsync(
        VivuCarDbContext dbContext,
        IReadOnlyList<CarDefinition> definitions,
        IReadOnlyDictionary<string, CarBrand> brands,
        CancellationToken cancellationToken
    )
    {
        var existing = await dbContext.CarModels.Include(model => model.CarBrand).ToListAsync(cancellationToken);
        var models = existing.ToDictionary(
            model => ModelKey(model.CarBrand.Name, model.Name),
            StringComparer.OrdinalIgnoreCase
        );
        foreach (var seed in definitions)
        {
            var key = ModelKey(seed.Brand, seed.Model);
            if (models.ContainsKey(key)) continue;
            var model = new CarModel { CarBrandId = brands[seed.Brand].Id, Name = seed.Model, IsActive = true };
            dbContext.CarModels.Add(model);
            models.Add(key, model);
        }
        await dbContext.SaveChangesAsync(cancellationToken);
        return models;
    }

    private static async Task<CloudinaryManifest> LoadCloudinaryManifestAsync(
        CancellationToken cancellationToken
    )
    {
        var assembly = typeof(CarSeed).Assembly;
        var resourceName = assembly.GetManifestResourceNames().SingleOrDefault(
            name => name.EndsWith(CloudinaryManifestName, StringComparison.OrdinalIgnoreCase)
        ) ?? throw new InvalidOperationException($"Embedded seed resource '{CloudinaryManifestName}' was not found.");
        await using var stream = assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException($"Embedded seed resource '{resourceName}' could not be opened.");
        return await JsonSerializer.DeserializeAsync<CloudinaryManifest>(
            stream,
            new JsonSerializerOptions(JsonSerializerDefaults.Web),
            cancellationToken
        ) ?? throw new InvalidOperationException("Cloudinary image manifest is invalid.");
    }

    private static string ResolveType(string slug)
    {
        if (slug.Contains("colorado", StringComparison.OrdinalIgnoreCase)) return "Pickup";
        if (ContainsAny(slug, "hatchback", "spark", "swift", "mirage", "veloser")) return "Hatchback";
        if (ContainsAny(slug, "avanza", "xpander", "ertiga", "innova")) return "MPV";
        if (ContainsAny(slug, "captiva", "fortuner", "outlander", "sorento", "lux-sa", "omoda-c5", "santa-fe", "cx-5", "terra")) return "SUV";
        return "Sedan";
    }

    private static int ResolveSeatCount(string type, string slug) =>
        type is "SUV" or "MPV" || ContainsAny(slug, "fortuner", "captiva", "outlander", "sorento") ? 7 : 5;

    private static TransmissionType ResolveTransmission(string slug)
    {
        if (ContainsAny(slug, "spark-2012", "c200-2008")) return TransmissionType.Manual;
        if (ContainsAny(slug, "attrage", "outlander", "mirage")) return TransmissionType.Cvt;
        return TransmissionType.Automatic;
    }

    private static FuelType ResolveFuel(string slug) =>
        ContainsAny(slug, "colorado", "fortuner", "sorento", "terra") ? FuelType.Diesel : FuelType.Gasoline;

    private static string BrandName(string slugBrand) => slugBrand.ToLowerInvariant() switch
    {
        "vinfast" => "VinFast",
        "mercedes" => "Mercedes-Benz",
        "kia" => "Kia",
        "bmw" => "BMW",
        _ => CultureInfo.InvariantCulture.TextInfo.ToTitleCase(slugBrand)
    };

    private static string LicensePlate(int carId, string type) =>
        $"{(type == "Pickup" ? "43C" : "43A")}-{12000 + carId:00000}";

    private static bool ContainsAny(string value, params string[] terms) =>
        terms.Any(term => value.Contains(term, StringComparison.OrdinalIgnoreCase));

    private static string ModelKey(string brand, string model) => $"{brand.Trim()}::{model.Trim()}";

    private sealed record CarDefinition(
        string Slug,
        string Brand,
        string Model,
        string Type,
        string Name,
        string LicensePlate,
        short Year,
        string Color,
        int KilometersDriven,
        int SeatCount,
        TransmissionType Transmission,
        FuelType Fuel,
        decimal DailyPrice,
        CarStatus Status,
        string Description,
        string? BlockedReason,
        string Location
    );

    private sealed class CloudinaryManifest
    {
        public List<CloudinaryCar> Cars { get; set; } = [];
    }

    private sealed class CloudinaryCar
    {
        [JsonPropertyName("source_order")]
        public int SourceOrder { get; set; }

        [JsonPropertyName("car_id")]
        public int CarId { get; set; }

        public string Slug { get; set; } = string.Empty;
        public List<CloudinaryImage> Images { get; set; } = [];
    }

    private sealed class CloudinaryImage
    {
        [JsonPropertyName("display_order")]
        public int DisplayOrder { get; set; }

        [JsonPropertyName("is_primary")]
        public bool IsPrimary { get; set; }

        public string Url { get; set; } = string.Empty;
    }
}