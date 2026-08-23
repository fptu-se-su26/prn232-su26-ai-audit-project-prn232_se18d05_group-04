using Services.Models.Admin;

namespace Services.Interfaces;

public interface IDriverLicenseOcrService
{
    Task<DriverLicenseOcrResult> ScanAsync(string imageUrl, CancellationToken cancellationToken = default);
}

public sealed class DriverLicenseOcrException(string message, Exception? innerException = null)
    : Exception(message, innerException);
