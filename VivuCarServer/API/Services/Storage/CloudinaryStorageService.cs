using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace API.Services.Storage;

public class CloudinaryStorageService : IFileStorageService
{
    private readonly Cloudinary _cloudinary;
    private readonly string _folderName;

    public CloudinaryStorageService(IConfiguration configuration)
    {
        var cloudinaryConfiguration = configuration.GetSection("Cloudinary");
        var cloudName = cloudinaryConfiguration["CloudName"];
        var apiKey = cloudinaryConfiguration["ApiKey"];
        var apiSecret = cloudinaryConfiguration["ApiSecret"];
        _folderName = cloudinaryConfiguration["FolderName"] ?? "Vivucar";

        if (string.IsNullOrEmpty(cloudName) || string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(apiSecret))
        {
            throw new ArgumentException("Cloudinary credentials are not properly configured in the environment.");
        }

        var account = new Account(cloudName, apiKey, apiSecret);
        _cloudinary = new Cloudinary(account);
        _cloudinary.Api.Secure = true;
    }

    public async Task<StoredFileResult> SaveAsync(IFormFile file, string folder, CancellationToken cancellationToken = default)
    {
        ValidateFile(file);

        await using var stream = file.OpenReadStream();
        
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(file.FileName, stream),
            Folder = $"{_folderName}/{folder}".Trim('/'),
            PublicId = Guid.NewGuid().ToString("N"),
            Overwrite = true
        };

        var uploadResult = await _cloudinary.UploadAsync(uploadParams, cancellationToken);

        if (uploadResult.Error != null)
        {
            throw new Exception($"Cloudinary upload failed: {uploadResult.Error.Message}");
        }

        return new StoredFileResult(
            FileName: file.FileName,
            RelativePath: uploadResult.PublicId,
            PublicUrl: uploadResult.SecureUrl.ToString()
        );
    }

    public async Task DeleteAsync(string filePath, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(filePath)) return;

        // If the filePath passed is a full URL (like we store it), extract PublicId
        var publicId = ExtractPublicIdFromUrl(filePath);
        if (string.IsNullOrEmpty(publicId)) return;

        var deletionParams = new DeletionParams(publicId)
        {
            ResourceType = ResourceType.Image
        };

        await _cloudinary.DestroyAsync(deletionParams);
    }

    private static void ValidateFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            throw new InvalidFileUploadException("File is empty.");
        }

        const long maxSizeBytes = 5 * 1024 * 1024; // 5 MB
        if (file.Length > maxSizeBytes)
        {
            throw new InvalidFileUploadException("Each image must be 5 MB or smaller.");
        }

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (!allowedExtensions.Contains(extension))
        {
            throw new InvalidFileUploadException("Only JPEG, PNG, or WebP images are allowed.");
        }
    }

    private static string? ExtractPublicIdFromUrl(string url)
    {
        // Example URL: https://res.cloudinary.com/demo/image/upload/v1234567890/folder/sample.jpg
        // Public ID is "folder/sample"
        try
        {
            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
            {
                return url; // fallback, in case it's already a publicId
            }

            var segments = uri.AbsolutePath.Split('/');
            
            // Find "upload" in segments
            var uploadIndex = Array.IndexOf(segments, "upload");
            if (uploadIndex == -1 || uploadIndex + 2 >= segments.Length)
            {
                return url;
            }

            // Skip version segment (e.g. v1234567890) and join the rest
            var startIndex = uploadIndex + 2; 
            var publicIdWithExtension = string.Join("/", segments.Skip(startIndex));
            
            // Remove extension
            var lastDotIndex = publicIdWithExtension.LastIndexOf('.');
            if (lastDotIndex > -1)
            {
                return publicIdWithExtension[..lastDotIndex];
            }

            return publicIdWithExtension;
        }
        catch
        {
            return null;
        }
    }
}
