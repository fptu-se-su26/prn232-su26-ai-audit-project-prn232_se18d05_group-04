using BusinessObjects.Enums;
using BusinessObjects.Models;
using Moq;
using Repositories.Interfaces;
using Services.Implementations;
using Services.Interfaces;
using Services.Models.Admin;

namespace VivuCarServer.Tests;

public class AdminModerationServiceTests
{
    private readonly Mock<IAdminModerationRepository> repository = new();
    private readonly Mock<IDriverLicenseOcrService> ocrService = new();
    private AdminModerationService Service() => new(repository.Object, ocrService.Object);

    [Fact]
    public async Task GetContentAsync_CombinesReviewsAndIncidentsNewestFirst()
    {
        var customer = new AppUser { FullName = "Nguyen Minh Anh" };
        repository.Setup(x => x.GetReviewsAsync(It.IsAny<CancellationToken>())).ReturnsAsync([
            new Review
            {
                Id = 1, Rating = 5, Comment = "Xe sạch.", Customer = customer,
                Car = new Car { Name = "VinFast VF 7 Plus" }, CreatedAt = new DateTime(2026, 7, 1)
            }
        ]);
        repository.Setup(x => x.GetIncidentReportsAsync(It.IsAny<CancellationToken>())).ReturnsAsync([
            new IncidentReport
            {
                Id = 2, Title = "Nội thất bẩn", Description = "Cần kiểm tra.", Reporter = customer,
                Status = IncidentStatus.Open, CreatedAt = new DateTime(2026, 7, 2)
            }
        ]);

        var result = await Service().GetContentAsync();

        Assert.Equal(2, result.Count);
        Assert.Equal("incident", result[0].Source);
        Assert.Equal("reported", result[0].Status);
        Assert.Equal("review", result[1].Source);
    }

    [Fact]
    public async Task GetLicensesAsync_MapsPersistedVerificationStatus()
    {
        repository.Setup(x => x.GetDriverDocumentsAsync(It.IsAny<CancellationToken>())).ReturnsAsync([
            new DriverDocument
            {
                Id = 3, UserId = 8, DriverLicenseNumber = "790204013579",
                VerificationStatus = DocumentVerificationStatus.Pending,
                User = new AppUser { FullName = "Tran Hoai Nam", Email = "nam@example.com" }
            }
        ]);

        var result = await Service().GetLicensesAsync();

        var item = Assert.Single(result);
        Assert.Equal("pending", item.Status);
        Assert.Equal("Tran Hoai Nam", item.UserName);
    }

    [Fact]
    public async Task ScanLicenseAsync_ReturnsExtractedFieldsWithoutPersistingThem()
    {
        repository.Setup(x => x.GetDriverDocumentAsync(5, It.IsAny<CancellationToken>())).ReturnsAsync(
            new DriverDocument { Id = 5, DriverLicenseFrontImageUrl = "https://res.cloudinary.com/demo/image/upload/license.jpg" });
        ocrService.Setup(x => x.ScanAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(
            new DriverLicenseOcrResult("raw", "NGUYỄN MINH TUẤN", "480243004966", "21/06/2004", "A2", "Không thời hạn", 86.4f, new DateTime(2026, 7, 22)));

        var result = await Service().ScanLicenseAsync(5);

        Assert.NotNull(result);
        Assert.Equal("480243004966", result.LicenseNumber);
        Assert.Equal("A2", result.LicenseClass);
        repository.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task SetLicenseStatusAsync_ApprovesAndPersistsDocument()
    {
        var document = new DriverDocument { Id = 4, VerificationStatus = DocumentVerificationStatus.Pending };
        repository.Setup(x => x.GetDriverDocumentAsync(4, It.IsAny<CancellationToken>())).ReturnsAsync(document);

        var result = await Service().SetLicenseStatusAsync(4, "approved");

        Assert.True(result);
        Assert.Equal(DocumentVerificationStatus.Approved, document.VerificationStatus);
        Assert.NotNull(document.UpdatedAt);
        repository.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
