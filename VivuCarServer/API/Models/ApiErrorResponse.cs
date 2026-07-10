namespace API.Models;

public record ApiErrorResponse(
    string Message,
    IReadOnlyDictionary<string, string[]>? Errors,
    string TraceId
);
