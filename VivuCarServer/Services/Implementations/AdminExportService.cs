using Repositories.Models;
using System.Globalization;
using System.Text;
using System.Text.Json;
using BusinessObjects.Models;
using Repositories.Interfaces;
using Services.Interfaces;
using Services.Models.Admin;

namespace Services.Implementations;

public class AdminExportService(IAdminExportRepository repository,IAdminExportFileStore store):IAdminExportService
{
    private static readonly HashSet<string> Types=["payments","users","cars","revenue"];
    private static readonly HashSet<string> Formats=["csv","pdf"];
    public async Task<AdminReportPreviewResponse> PreviewAsync(AdminReportFilter filter,CancellationToken ct=default){Validate(filter);return await repository.GetPreviewAsync(filter,100,ct);}
    public async Task<AdminExportJobResponse> CreateJobAsync(int adminId,AdminExportJobCreateRequest request,CancellationToken ct=default)
    {
        Validate(request.Filter); var parts=request.ExportType.ToLowerInvariant().Split('_'); if(parts.Length!=2||!Types.Contains(parts[0])||!Formats.Contains(parts[1]))throw new AdminExportValidationException("export_type must be <payments|users|cars|revenue>_<csv|pdf>.");
        if(parts[0]!=request.Filter.Type.ToLowerInvariant())throw new AdminExportValidationException("export_type and filter.type must match.");
        var job=new ExportJob{RequestedBy=adminId,ExportType=request.ExportType.ToLowerInvariant(),ParamsJson=JsonSerializer.Serialize(request.Filter),Status="pending",CreatedAt=DateTime.UtcNow}; await repository.AddJobAsync(job,ct); await repository.SaveChangesAsync(ct);
        try{job.Status="processing";await repository.SaveChangesAsync(ct);var preview=await repository.GetPreviewAsync(request.Filter,5000,ct);var fileName=$"vivucar-{parts[0]}-{DateTime.UtcNow:yyyyMMdd-HHmmss}.{parts[1]}";var bytes=parts[1]=="csv"?Csv(preview):SimplePdf.Create($"VivuCar {parts[0]} report",preview);job.FileUrl=await store.SaveAsync(fileName,bytes,ct);job.Status="done";job.CompletedAt=DateTime.UtcNow;await repository.SaveChangesAsync(ct);}
        catch(Exception ex){job.Status="failed";job.ErrorMessage=ex.Message[..Math.Min(ex.Message.Length,2000)];job.CompletedAt=DateTime.UtcNow;await repository.SaveChangesAsync(ct);}
        return Map(job);
    }
    public async Task<IReadOnlyList<AdminExportJobResponse>> GetJobsAsync(int adminId,CancellationToken ct=default)=>(await repository.GetJobsAsync(adminId,50,ct)).Select(Map).ToList();
    public async Task<AdminExportJobResponse?> GetJobAsync(int adminId,int id,CancellationToken ct=default){var job=await repository.GetJobAsync(id,adminId,ct);return job is null?null:Map(job);}
    public async Task<ExportFileContent?> DownloadAsync(int adminId,int id,CancellationToken ct=default){var job=await repository.GetJobAsync(id,adminId,ct);if(job is null||job.Status!="done"||string.IsNullOrWhiteSpace(job.FileUrl))return null;var bytes=await store.ReadAsync(job.FileUrl,ct);if(bytes is null)return null;var pdf=job.ExportType.EndsWith("_pdf");return new(bytes,pdf?"application/pdf":"text/csv; charset=utf-8",Path.GetFileName(job.FileUrl));}
    private static void Validate(AdminReportFilter f){var type=f.Type.ToLowerInvariant();if(!Types.Contains(type))throw new AdminExportValidationException("Unsupported report type.");if(f.From>f.To)throw new AdminExportValidationException("from must be on or before to.");if(f.To.DayNumber-f.From.DayNumber+1>366)throw new AdminExportValidationException("Date range cannot exceed 366 days.");if(!string.IsNullOrEmpty(f.PaymentStatus)&&f.PaymentStatus is not("success" or "pending" or "failed" or "refunded"))throw new AdminExportValidationException("Invalid payment status.");if(!string.IsNullOrEmpty(f.BookingStatus)&&f.BookingStatus is not("pending" or "approved" or "rejected" or "completed" or "cancelled"))throw new AdminExportValidationException("Invalid booking status.");}
    private static byte[] Csv(AdminReportPreviewResponse p){var b=new StringBuilder();b.AppendLine(string.Join(',',p.Headers.Select(E)));foreach(var row in p.Rows)b.AppendLine(string.Join(',',row.Select(E)));return new UTF8Encoding(true).GetBytes(b.ToString());static string E(string v)=>$"\"{v.Replace("\"","\"\"")}\"";}
    private static AdminExportJobResponse Map(ExportJob j)=>new(){Id=j.Id,ExportType=j.ExportType,Status=j.Status,FileUrl=j.FileUrl,ErrorMessage=j.ErrorMessage,CreatedAt=j.CreatedAt,CompletedAt=j.CompletedAt};
}
public sealed class AdminExportValidationException(string message):Exception(message);

internal static class SimplePdf
{
    public static byte[] Create(string title,AdminReportPreviewResponse data)
    {
        var lines=new List<string>{title,$"Generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm} UTC",string.Join(" | ",data.Headers)};lines.AddRange(data.Rows.Select(r=>string.Join(" | ",r)));var pages=lines.Chunk(42).ToList();var objects=new List<string>();objects.Add("<< /Type /Catalog /Pages 2 0 R >>");var kids=string.Join(' ',Enumerable.Range(0,pages.Count).Select(i=>$"{4+i*2} 0 R"));objects.Add($"<< /Type /Pages /Kids [{kids}] /Count {pages.Count} >>");objects.Add("<< /Type /Font /Subtype /Type1 /BaseFont /Courier >>");
        for(var i=0;i<pages.Count;i++){var pageId=4+i*2; var contentId=pageId+1;objects.Add($"<< /Type /Page /Parent 2 0 R /MediaBox [0 0 842 595] /Resources << /Font << /F1 3 0 R >> >> /Contents {contentId} 0 R >>");var stream=new StringBuilder("BT /F1 8 Tf 28 565 Td 11 TL ");foreach(var line in pages[i])stream.Append('(').Append(Escape(Ascii(line,130))).Append(") Tj T* ");stream.Append("ET");var s=stream.ToString();objects.Add($"<< /Length {Encoding.ASCII.GetByteCount(s)} >>\nstream\n{s}\nendstream");}
        var output=new MemoryStream();void W(string s){var bytes=Encoding.ASCII.GetBytes(s);output.Write(bytes);}W("%PDF-1.4\n");var offsets=new List<long>{0};for(var i=0;i<objects.Count;i++){offsets.Add(output.Position);W($"{i+1} 0 obj\n{objects[i]}\nendobj\n");}var xref=output.Position;W($"xref\n0 {objects.Count+1}\n0000000000 65535 f \n");for(var i=1;i<offsets.Count;i++)W($"{offsets[i]:D10} 00000 n \n");W($"trailer << /Size {objects.Count+1} /Root 1 0 R >>\nstartxref\n{xref}\n%%EOF");return output.ToArray();
    }
    private static string Ascii(string value,int max){var normalized=value.Normalize(NormalizationForm.FormD);var chars=normalized.Where(c=>CharUnicodeInfo.GetUnicodeCategory(c)!=UnicodeCategory.NonSpacingMark&&c<=127).ToArray();var result=new string(chars);return result[..Math.Min(result.Length,max)];}
    private static string Escape(string value)=>value.Replace("\\","\\\\").Replace("(","\\(").Replace(")","\\)");
}


