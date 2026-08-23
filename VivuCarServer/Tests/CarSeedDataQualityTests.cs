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

    [Fact]
    public void CloudinaryCatalog_HasNormalizedMetadata_AndCorrectVinFastVf7Facts()
    {
        using var document = LoadCatalog();
        var cars = document.RootElement.GetProperty("cars").EnumerateArray().ToList();

        foreach (var car in cars)
        {
            Assert.False(string.IsNullOrWhiteSpace(car.GetProperty("brand").GetString()));
            Assert.False(string.IsNullOrWhiteSpace(car.GetProperty("model").GetString()));
            Assert.False(string.IsNullOrWhiteSpace(car.GetProperty("car_type").GetString()));
            Assert.False(string.IsNullOrWhiteSpace(car.GetProperty("address").GetString()));
            Assert.True(car.GetProperty("seats").GetInt32() > 0);
            Assert.True(car.GetProperty("price_per_day").GetDecimal() > 0);
            Assert.False(string.IsNullOrWhiteSpace(car.GetProperty("color").GetString()));
            Assert.False(string.IsNullOrWhiteSpace(car.GetProperty("description").GetString()));

            var normalizedTextFields = new[]
            {
                car.GetProperty("name").GetString(),
                car.GetProperty("brand").GetString(),
                car.GetProperty("model").GetString(),
                car.GetProperty("address").GetString(),
                car.GetProperty("color").GetString(),
                car.GetProperty("description").GetString()
            };
            Assert.All(normalizedTextFields, value =>
            {
                Assert.DoesNotContain("\uFFFD", value);
                Assert.DoesNotContain("?", value);
            });
        }

        var vf7 = cars.Single(car =>
            car.GetProperty("slug").GetString() == "vinfast-vf7-plus-2025"
        );
        Assert.Equal("VinFast", vf7.GetProperty("brand").GetString());
        Assert.Equal("VF 7 Plus", vf7.GetProperty("model").GetString());
        Assert.Equal("SUV", vf7.GetProperty("car_type").GetString());
        Assert.Equal(5, vf7.GetProperty("seats").GetInt32());
        Assert.Equal("electric", vf7.GetProperty("fuel_type").GetString());
        Assert.Equal(0, vf7.GetProperty("kilometers_driven").GetInt32());
        Assert.Equal("Tr\u1eafng", vf7.GetProperty("color").GetString());

        Assert.Equal(
            13,
            cars.Count(car => car.GetProperty("fuel_type").GetString() == "electric")
        );
        Assert.Equal(
            42,
            cars.Count(car => car.GetProperty("fuel_type").GetString() == "gasoline")
        );
    }

    private static JsonDocument LoadCatalog()
    {
        var assembly = typeof(CarSeed).Assembly;
        var resourceName = assembly.GetManifestResourceNames().Single(name =>
            name.EndsWith("cloudinary-car-images.json", StringComparison.OrdinalIgnoreCase)
        );
        using var stream = assembly.GetManifestResourceStream(resourceName);
        return JsonDocument.Parse(stream!);
    }

}
