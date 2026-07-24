using Microsoft.AspNetCore.Mvc;

namespace WebClient.Controllers;

public abstract class ProxyControllerBase(
    IHttpClientFactory httpClientFactory
) : ControllerBase
{
    protected async Task<IActionResult> ForwardAsync(
        string upstreamPath,
        bool forwardCookies,
        CancellationToken cancellationToken
    )
    {
        var client = httpClientFactory.CreateClient("VivuCarApi");
        using var request = new HttpRequestMessage(
            new HttpMethod(Request.Method),
            upstreamPath
        );

        if (Request.ContentLength > 0 || Request.Headers.TransferEncoding.Count > 0 || Request.ContentType is not null)
        {
            var memoryStream = new MemoryStream();
            await Request.Body.CopyToAsync(memoryStream, cancellationToken);
            memoryStream.Position = 0;
            request.Content = new StreamContent(memoryStream);

            if (Request.ContentType is not null)
            {
                request.Content.Headers.TryAddWithoutValidation(
                    "Content-Type",
                    Request.ContentType
                );
            }
        }

        CopyRequestHeader(request, "Authorization");
        CopyRequestHeader(request, "Accept");

        if (forwardCookies)
        {
            CopyRequestHeader(request, "Cookie");
        }

        using var response = await client.SendAsync(
            request,
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken
        );

        if (forwardCookies && response.Headers.TryGetValues("Set-Cookie", out var cookies))
        {
            foreach (var cookie in cookies)
            {
                Response.Headers.Append("Set-Cookie", cookie);
            }
        }

        Response.StatusCode = (int)response.StatusCode;
        var content = await response.Content.ReadAsByteArrayAsync(cancellationToken);

        if (content.Length == 0)
        {
            return new EmptyResult();
        }

        return File(
            content,
            response.Content.Headers.ContentType?.ToString() ?? "application/json"
        );
    }

    private void CopyRequestHeader(HttpRequestMessage request, string headerName)
    {
        if (Request.Headers.TryGetValue(headerName, out var values))
        {
            request.Headers.TryAddWithoutValidation(
                headerName,
                values.ToArray()
            );
        }
    }
}
