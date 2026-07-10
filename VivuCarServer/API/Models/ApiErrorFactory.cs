using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace API.Models;

public static class ApiErrorFactory
{
    public static ObjectResult FromServiceException(HttpContext httpContext, AdminCarServiceException exception)
    {
        return new ObjectResult(new ApiErrorResponse(
            exception.Message,
            exception.Errors,
            httpContext.TraceIdentifier
        ))
        {
            StatusCode = exception.StatusCode
        };
    }

    public static ObjectResult Error(HttpContext httpContext, int statusCode, string message, IReadOnlyDictionary<string, string[]>? errors = null)
    {
        return new ObjectResult(new ApiErrorResponse(message, errors, httpContext.TraceIdentifier))
        {
            StatusCode = statusCode
        };
    }
}
