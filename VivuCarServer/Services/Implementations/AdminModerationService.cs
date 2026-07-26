using BusinessObjects.Enums;
using Repositories.Interfaces;
using Services.Interfaces;
using Services.Models.Admin;

namespace Services.Implementations;

public class AdminModerationService(
    IAdminModerationRepository repository,
    IDriverLicenseOcrService ocrService) : IAdminModerationService
{
    public async Task<IReadOnlyList<AdminModerationContentResponse>> GetContentAsync(CancellationToken cancellationToken = default)
    {
        var reviews = await repository.GetReviewsAsync(cancellationToken);
        var incidents = await repository.GetIncidentReportsAsync(cancellationToken);

        return reviews.Select(review => new AdminModerationContentResponse(
                review.Id,
                "review",
                "Đánh giá",
                $"{review.Rating}/5 · {review.Car.Name}",
                review.Comment ?? "Không có nội dung.",
                review.Customer.FullName,
                "published",
                review.CreatedAt))
            .Concat(incidents.Select(report => new AdminModerationContentResponse(
                report.Id,
                "incident",
                "Báo cáo sự cố",
                report.Title,
                report.Description,
                report.Reporter.FullName,
                MapIncidentStatus(report.Status),
                report.CreatedAt)))
            .OrderByDescending(item => item.CreatedAt)
            .ToList();
    }

    public async Task<IReadOnlyList<AdminLicenseModerationResponse>> GetLicensesAsync(CancellationToken cancellationToken = default)
    {
        var documents = await repository.GetDriverDocumentsAsync(cancellationToken);
        return documents.Select(MapLicense).ToList();
    }

    public async Task<AdminLicenseOcrResponse?> ScanLicenseAsync(int id, CancellationToken cancellationToken = default)
    {
        var document = await repository.GetDriverDocumentAsync(id, cancellationToken);
        if (document is null) return null;
        if (string.IsNullOrWhiteSpace(document.DriverLicenseFrontImageUrl))
            throw new DriverLicenseOcrException("Hồ sơ chưa có ảnh GPLX mặt trước.");

        var result = await ocrService.ScanAsync(document.DriverLicenseFrontImageUrl, cancellationToken);
        return new AdminLicenseOcrResponse(
            document.Id,
            result.FullName,
            result.LicenseNumber,
            result.DateOfBirth,
            result.LicenseClass,
            result.ExpiryDate,
            result.Confidence,
            result.ProcessedAt,
            result.RawText);
    }

    public async Task<bool> SetLicenseStatusAsync(int id, string status, CancellationToken cancellationToken = default)
    {
        var document = await repository.GetDriverDocumentAsync(id, cancellationToken);
        if (document is null) return false;

        var isApproved = status.Trim().ToLowerInvariant() == "approved";
        document.VerificationStatus = isApproved ? DocumentVerificationStatus.Approved : DocumentVerificationStatus.Rejected;
        document.UpdatedAt = DateTime.UtcNow;

        if (isApproved)
        {
            var pendingBookings = await repository.GetPendingGPLXBookingsByUserIdAsync(document.UserId, cancellationToken);
            foreach (var booking in pendingBookings)
            {
                booking.Status = BookingStatus.WaitingDeposit;
                booking.UpdatedAt = DateTime.UtcNow;
                booking.StatusHistories.Add(new BusinessObjects.Models.BookingStatusHistory
                {
                    OldStatus = BookingStatus.PendingGPLX,
                    NewStatus = BookingStatus.WaitingDeposit,
                    ChangedByUserId = null,
                    Note = "Hệ thống tự động duyệt do GPLX đã được xác thực.",
                    CreatedAt = DateTime.UtcNow
                });
            }
        }

        await repository.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static AdminLicenseModerationResponse MapLicense(BusinessObjects.Models.DriverDocument document)
        => new(document.Id, document.UserId, document.User.FullName, document.User.Email,
            document.DriverLicenseNumber, document.DriverLicenseFrontImageUrl,
            document.DriverLicenseBackImageUrl, document.VerificationStatus.ToString().ToLowerInvariant(),
            document.CreatedAt, document.UpdatedAt);

    private static string MapIncidentStatus(IncidentStatus status) => status switch
    {
        IncidentStatus.Open => "reported",
        IncidentStatus.InReview => "in_review",
        IncidentStatus.Resolved => "resolved",
        IncidentStatus.Rejected => "rejected",
        IncidentStatus.Closed => "closed",
        _ => "reported"
    };
}
