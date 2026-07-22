using System.Reflection;
using System.Text.Json;
using BusinessObjects.Data.Seed;

namespace VivuCarServer.Tests;

public class CarSeedDataQualityTests
{
    [Fact]
    public void CloudinaryCatalog_HasAtLeastThreeDistinctImagesPerCar()
    {
        var assembly = typeof(CarSeed).Assembly;
        var resourceName = assembly.GetManifestResourceNames().Single(name =>
            name.EndsWith("cloudinary-car-images.json", StringComparison.OrdinalIgnoreCase)
        );
        using var stream = assembly.GetManifestResourceStream(resourceName);
        using var document = JsonDocument.Parse(stream!);
        var root = document.RootElement;
        var cars = root.GetProperty("cars").EnumerateArray().ToList();

        Assert.Equal(root.GetProperty("car_count").GetInt32(), cars.Count);
        Assert.Equal(
            root.GetProperty("image_count").GetInt32(),
            cars.Sum(car => car.GetProperty("images").GetArrayLength())
        );

        foreach (var car in cars)
        {
            var distinctUrls = car.GetProperty("images")
                .EnumerateArray()
                .Select(image => image.GetProperty("url").GetString())
                .Where(url => !string.IsNullOrWhiteSpace(url))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Count();

            Assert.True(
                distinctUrls >= CarSeed.MinimumImagesPerCar,
                $"Catalog car '{car.GetProperty("slug").GetString()}' has only {distinctUrls} distinct images."
            );
        }
    }
}
