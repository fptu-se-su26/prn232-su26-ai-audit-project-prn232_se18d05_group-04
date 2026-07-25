using Microsoft.Extensions.Caching.Memory;
using Services.Implementations;
using Services.Interfaces;
using Services.Models.Admin;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using Tesseract;

namespace API.Services.Ocr;

public sealed class TesseractDriverLicenseOcrService(
    HttpClient httpClient,
    IWebHostEnvironment environment,
    IMemoryCache cache,
    ILogger<TesseractDriverLicenseOcrService> logger) : IDriverLicenseOcrService
{
    private const int MaxImageBytes = 8 * 1024 * 1024;

    public async Task<DriverLicenseOcrResult> ScanAsync(string imageUrl, CancellationToken cancellationToken = default)
    {
        var uri = ValidateImageUrl(imageUrl);
        var cacheKey = $"driver-license-ocr:v2:{uri}";
        if (cache.TryGetValue<DriverLicenseOcrResult>(cacheKey, out var cached) && cached is not null)
            return cached;

        var imageBytes = await DownloadImageAsync(uri, cancellationToken);
        DriverLicenseOcrResult result;
        try
        {
            result = await Task.Run(() => Recognize(imageBytes), cancellationToken);
        }
        catch (Exception exception) when (exception is not DriverLicenseOcrException)
        {
            logger.LogError(exception, "Driver-license OCR failed for document image host {Host}.", uri.Host);
            throw new DriverLicenseOcrException("Không thể nhận diện ảnh GPLX. Hãy kiểm tra độ rõ của ảnh và thử lại.", exception);
        }

        cache.Set(cacheKey, result, TimeSpan.FromMinutes(30));
        return result;
    }

    private DriverLicenseOcrResult Recognize(byte[] imageBytes)
    {
        var dataPath = Path.Combine(environment.ContentRootPath, "tessdata");
        if (!Directory.Exists(dataPath))
            throw new DriverLicenseOcrException("Thiếu model OCR tiếng Việt/Anh trên máy chủ.");

        using var engine = new TesseractEngine(dataPath, "vie+eng", EngineMode.LstmOnly);
        engine.SetVariable("user_defined_dpi", "300");

        var scans = CreateScanImages(imageBytes)
            .Select((bytes, index) => ReadPage(engine, bytes, index == 0 ? PageSegMode.Auto : PageSegMode.SparseText))
            .Where(scan => !string.IsNullOrWhiteSpace(scan.Text))
            .ToList();
        if (scans.Count == 0)
            throw new DriverLicenseOcrException("Không tìm thấy văn bản trong ảnh GPLX.");

        var combinedText = string.Join('\n', scans.Select(scan => scan.Text));
        var confidence = scans.Average(scan => scan.Confidence);
        return DriverLicenseOcrParser.Parse(combinedText, confidence, DateTime.UtcNow);
    }

    private static (string Text, float Confidence) ReadPage(TesseractEngine engine, byte[] bytes, PageSegMode mode)
    {
        using var image = Pix.LoadFromMemory(bytes);
        using var page = engine.Process(image, mode);
        return (page.GetText(), page.GetMeanConfidence());
    }

    private static IEnumerable<byte[]> CreateScanImages(byte[] imageBytes)
    {
        using var source = Image.Load(imageBytes);
        source.Mutate(context => context.AutoOrient());

        yield return Enhance(source);

        var details = new Rectangle(
            (int)(source.Width * 0.34),
            (int)(source.Height * 0.24),
            (int)(source.Width * 0.64),
            (int)(source.Height * 0.52));
        using var detailsImage = source.Clone(context => context.Crop(details));
        yield return Enhance(detailsImage);

        var lower = new Rectangle(
            0,
            (int)(source.Height * 0.58),
            (int)(source.Width * 0.84),
            (int)(source.Height * 0.40));
        using var lowerImage = source.Clone(context => context.Crop(lower));
        yield return Enhance(lowerImage);
    }

    private static byte[] Enhance(Image image)
    {
        using var enhanced = image.Clone(context => context
            .Resize(image.Width * 3, image.Height * 3, KnownResamplers.Lanczos3)
            .Grayscale()
            .Contrast(1.35f)
            .GaussianSharpen(1.1f));
        using var stream = new MemoryStream();
        enhanced.SaveAsPng(stream);
        return stream.ToArray();
    }

    private async Task<byte[]> DownloadImageAsync(Uri uri, CancellationToken cancellationToken)
    {
        using var response = await httpClient.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        if (!response.IsSuccessStatusCode)
            throw new DriverLicenseOcrException("Không thể tải ảnh GPLX từ Cloudinary.");
        if (response.Content.Headers.ContentLength is > MaxImageBytes)
            throw new DriverLicenseOcrException("Ảnh GPLX vượt quá giới hạn 8 MB.");
        if (response.Content.Headers.ContentType?.MediaType?.StartsWith("image/", StringComparison.OrdinalIgnoreCase) != true)
            throw new DriverLicenseOcrException("Tệp GPLX không phải định dạng ảnh hợp lệ.");

        var bytes = await response.Content.ReadAsByteArrayAsync(cancellationToken);
        if (bytes.Length is 0 or > MaxImageBytes)
            throw new DriverLicenseOcrException("Ảnh GPLX rỗng hoặc vượt quá giới hạn 8 MB.");
        return bytes;
    }

    private static Uri ValidateImageUrl(string imageUrl)
    {
        if (!Uri.TryCreate(imageUrl, UriKind.Absolute, out var uri) || uri.Scheme != Uri.UriSchemeHttps)
            throw new DriverLicenseOcrException("OCR chỉ hỗ trợ URL ảnh Cloudinary HTTPS.");
        if (!uri.Host.Equals("res.cloudinary.com", StringComparison.OrdinalIgnoreCase))
            throw new DriverLicenseOcrException("Nguồn ảnh OCR không được phép. Chỉ hỗ trợ res.cloudinary.com.");
        return uri;
    }
}