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

        // Always read and forward the request body.
        // fetch() + FormData often omits Content-Length, so never gate on ContentLength.
        Request.EnableBuffering();
        if (Request.Body.CanSeek)
        {
            Request.Body.Position = 0;
        }

        await using var memoryStream = new MemoryStream();
        await Request.Body.CopyToAsync(memoryStream, cancellationToken);
        var bodyBytes = memoryStream.ToArray();

        if (bodyBytes.Length > 0)
        {
            request.Content = new ByteArrayContent(bodyBytes);

            if (Request.ContentType is not null)
            {
                request.Content.Headers.TryAddWithoutValidation(
                    "Content-Type",
                    Request.ContentType
                );
            }

            if (Request.ContentLength is > 0)
            {
                request.Content.Headers.ContentLength = Request.ContentLength;
            }
            else
            {
                request.Content.Headers.ContentLength = bodyBytes.Length;
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

        if (response.Headers.TryGetValues("Location", out var locations))
        {
            Response.Headers.Append("Location", locations.FirstOrDefault());
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
