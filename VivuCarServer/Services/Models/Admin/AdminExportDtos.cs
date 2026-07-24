using Repositories.Models;

namespace Services.Models.Admin;

public sealed class AdminExportJobCreateRequest
{
    public string ExportType { get; init; } = string.Empty;
    public AdminReportFilter Filter { get; init; } = new();
}

public sealed class AdminExportJobResponse
{
    public int Id { get; init; }
    public required string ExportType { get; init; }
    public required string Status { get; init; }
    public string? FileUrl { get; init; }
    public string? ErrorMessage { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? CompletedAt { get; init; }
}

public sealed record ExportFileContent(byte[] Bytes, string ContentType, string FileName);

