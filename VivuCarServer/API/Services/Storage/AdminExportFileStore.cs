using Services.Interfaces;
namespace API.Services.Storage;
public class AdminExportFileStore(IWebHostEnvironment environment):IAdminExportFileStore
{
    private string Root=>Path.GetFullPath(Path.Combine(environment.ContentRootPath,"App_Data","exports","admin"));
    public async Task<string> SaveAsync(string fileName,byte[] content,CancellationToken ct=default){Directory.CreateDirectory(Root);var safe=Path.GetFileName(fileName);var path=Path.Combine(Root,safe);await File.WriteAllBytesAsync(path,content,ct);return safe;}
    public async Task<byte[]?> ReadAsync(string relativePath,CancellationToken ct=default){var path=Path.GetFullPath(Path.Combine(Root,Path.GetFileName(relativePath)));if(!path.StartsWith(Root,StringComparison.OrdinalIgnoreCase)||!File.Exists(path))return null;return await File.ReadAllBytesAsync(path,ct);}
}
