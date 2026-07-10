namespace API.Services.Storage;

public record StoredFileResult(string FileName, string RelativePath, string PublicUrl);

public interface IFileStorageService
{
    Task<StoredFileResult> SaveAsync(
        IFormFile file,
        string folder,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        string filePath,
        CancellationToken cancellationToken = default);
}
