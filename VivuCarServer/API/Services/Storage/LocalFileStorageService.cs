namespace API.Services.Storage;

public class LocalFileStorageService(IWebHostEnvironment environment) : IFileStorageService
{
    private const long MaxFileSize = 5 * 1024 * 1024;
    private static readonly Dictionary<string, string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        [".jpg"] = "image/jpeg",
        [".jpeg"] = "image/jpeg",
        [".png"] = "image/png",
        [".webp"] = "image/webp"
    };

    public async Task<StoredFileResult> SaveAsync(
        IFormFile file,
        string folder,
        CancellationToken cancellationToken = default)
    {
        ValidateFile(file);
        var safeFolder = NormalizeFolder(folder);
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        var fileName = $"{Guid.NewGuid():N}{extension}";
        var webRootPath = environment.WebRootPath ?? Path.Combine(environment.ContentRootPath, "wwwroot");
        var absoluteFolder = Path.Combine(webRootPath, safeFolder.Replace('/', Path.DirectorySeparatorChar));
        Directory.CreateDirectory(absoluteFolder);

        var absolutePath = Path.Combine(absoluteFolder, fileName);
        await using var stream = new FileStream(absolutePath, FileMode.CreateNew, FileAccess.Write, FileShare.None);
        await file.CopyToAsync(stream, cancellationToken);

        var relativePath = $"{safeFolder}/{fileName}";
        return new StoredFileResult(fileName, relativePath, $"/{relativePath}");
    }

    public Task DeleteAsync(string filePath, CancellationToken cancellationToken = default)
    {
        var webRootPath = environment.WebRootPath ?? Path.Combine(environment.ContentRootPath, "wwwroot");
        var relativePath = filePath.TrimStart('/', '\\').Replace('/', Path.DirectorySeparatorChar);
        var absolutePath = Path.GetFullPath(Path.Combine(webRootPath, relativePath));
        var rootPath = Path.GetFullPath(webRootPath);

        if (!absolutePath.StartsWith(rootPath, StringComparison.OrdinalIgnoreCase))
        {
            return Task.CompletedTask;
        }

        if (File.Exists(absolutePath))
        {
            File.Delete(absolutePath);
        }

        return Task.CompletedTask;
    }

    private static void ValidateFile(IFormFile file)
    {
        if (file.Length <= 0)
        {
            throw new InvalidFileUploadException("File is empty.");
        }

        if (file.Length > MaxFileSize)
        {
            throw new InvalidFileUploadException("Each image must be 5 MB or smaller.");
        }

        var extension = Path.GetExtension(file.FileName);
        if (!AllowedExtensions.TryGetValue(extension, out var expectedContentType))
        {
            throw new InvalidFileUploadException("Only JPEG, PNG, or WebP images are allowed.");
        }

        if (!string.Equals(file.ContentType, expectedContentType, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidFileUploadException("File MIME type does not match the allowed image type.");
        }

        using var stream = file.OpenReadStream();
        Span<byte> header = stackalloc byte[12];
        var bytesRead = stream.Read(header);

        if (!HasValidSignature(header[..bytesRead], extension))
        {
            throw new InvalidFileUploadException("File signature does not match a valid image.");
        }
    }

    private static bool HasValidSignature(ReadOnlySpan<byte> header, string extension)
    {
        if (extension.Equals(".jpg", StringComparison.OrdinalIgnoreCase) || extension.Equals(".jpeg", StringComparison.OrdinalIgnoreCase))
        {
            return header.Length >= 3 && header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF;
        }

        if (extension.Equals(".png", StringComparison.OrdinalIgnoreCase))
        {
            return header.Length >= 8
                && header[0] == 0x89 && header[1] == 0x50 && header[2] == 0x4E && header[3] == 0x47
                && header[4] == 0x0D && header[5] == 0x0A && header[6] == 0x1A && header[7] == 0x0A;
        }

        if (extension.Equals(".webp", StringComparison.OrdinalIgnoreCase))
        {
            return header.Length >= 12
                && header[0] == 0x52 && header[1] == 0x49 && header[2] == 0x46 && header[3] == 0x46
                && header[8] == 0x57 && header[9] == 0x45 && header[10] == 0x42 && header[11] == 0x50;
        }

        return false;
    }

    private static string NormalizeFolder(string folder)
    {
        var normalized = folder.Trim().Trim('/', '\\').Replace('\\', '/');
        if (string.IsNullOrWhiteSpace(normalized) || normalized.Contains("..", StringComparison.Ordinal))
        {
            throw new InvalidFileUploadException("Invalid upload folder.");
        }

        return normalized;
    }
}

public class InvalidFileUploadException(string message) : Exception(message);
