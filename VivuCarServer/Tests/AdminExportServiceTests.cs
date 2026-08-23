using BusinessObjects.Models;
using Moq;
using Repositories.Interfaces;
using Repositories.Models;
using Services.Implementations;
using Services.Interfaces;
using Services.Models.Admin;

namespace VivuCarServer.Tests;

public class AdminExportServiceTests
{
    private readonly Mock<IAdminExportRepository> repo=new(); private readonly Mock<IAdminExportFileStore> store=new();
    private AdminExportService Service()=>new(repo.Object,store.Object);
    private static AdminReportFilter Filter()=>new(){Type="payments",From=new DateOnly(2026,7,1),To=new DateOnly(2026,7,31)};
    [Fact] public async Task CreateJobAsync_CreatesRealCsvAndMarksDone(){ExportJob? captured=null;repo.Setup(x=>x.AddJobAsync(It.IsAny<ExportJob>(),It.IsAny<CancellationToken>())).Callback<ExportJob,CancellationToken>((j,_)=>{j.Id=9;captured=j;}).Returns(Task.CompletedTask);repo.Setup(x=>x.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);repo.Setup(x=>x.GetPreviewAsync(It.IsAny<AdminReportFilter>(),5000,It.IsAny<CancellationToken>())).ReturnsAsync(new AdminReportPreviewResponse{Headers=["Giao dịch","Số tiền"],Rows=[["TX-1","1250000"]],TotalRows=1});store.Setup(x=>x.SaveAsync(It.IsAny<string>(),It.IsAny<byte[]>(),It.IsAny<CancellationToken>())).ReturnsAsync("exports/admin/report.csv");var result=await Service().CreateJobAsync(3,new(){ExportType="payments_csv",Filter=Filter()});Assert.Equal("done",result.Status);Assert.Equal(9,result.Id);Assert.NotNull(captured?.CompletedAt);store.Verify(x=>x.SaveAsync(It.Is<string>(name=>name.EndsWith(".csv")),It.Is<byte[]>(b=>b.Length>10&&b[0]==0xEF&&b[1]==0xBB&&b[2]==0xBF&&System.Text.Encoding.UTF8.GetString(b,3,b.Length-3).Contains("Giao dịch")),It.IsAny<CancellationToken>()),Times.Once);}
    [Fact] public async Task CreateJobAsync_RejectsMismatchedType(){await Assert.ThrowsAsync<AdminExportValidationException>(()=>Service().CreateJobAsync(3,new(){ExportType="cars_pdf",Filter=Filter()}));}
    [Fact] public async Task DownloadAsync_ReturnsOnlyCompletedOwnedJob(){repo.Setup(x=>x.GetJobAsync(7,3,It.IsAny<CancellationToken>())).ReturnsAsync(new ExportJob{Id=7,RequestedBy=3,ExportType="payments_pdf",Status="done",FileUrl="exports/admin/a.pdf"});store.Setup(x=>x.ReadAsync("exports/admin/a.pdf",It.IsAny<CancellationToken>())).ReturnsAsync([37,80,68,70]);var file=await Service().DownloadAsync(3,7);Assert.NotNull(file);Assert.Equal("application/pdf",file.ContentType);}
}

