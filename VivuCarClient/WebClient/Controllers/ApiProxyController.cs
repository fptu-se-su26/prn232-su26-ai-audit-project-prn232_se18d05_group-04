using Microsoft.AspNetCore.Mvc;

namespace WebClient.Controllers;

[ApiController]
[Route("api/proxy")]
public class ApiProxyController : ProxyControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;

    public ApiProxyController(IHttpClientFactory httpClientFactory)
        : base(httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }
    [HttpPost("uploads")]
    public async Task<IActionResult> ProxyUpload(
        [FromForm] IFormFile file,
        [FromQuery] string? folder,
        CancellationToken cancellationToken
    )
    {
        Console.WriteLine($"[Proxy] ProxyUpload hit! File: {file?.FileName}, Length: {file?.Length}, Folder: {folder}");
        if (file == null || file.Length == 0)
        {
            Console.WriteLine("[Proxy] ProxyUpload: file is null or empty!");
            return BadRequest("No file provided.");
        }

        var client = _httpClientFactory.CreateClient("VivuCarApi");
        using var content = new MultipartFormDataContent();
        
        using var fileStream = file.OpenReadStream();
        using var streamContent = new StreamContent(fileStream);
        streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
        
        content.Add(streamContent, "file", file.FileName);

        var requestMessage = new HttpRequestMessage(HttpMethod.Post, $"api/uploads?folder={folder}")
        {
            Content = content
        };

        if (Request.Headers.TryGetValue("Authorization", out var authHeader))
        {
            requestMessage.Headers.TryAddWithoutValidation("Authorization", authHeader.ToArray());
        }

        Console.WriteLine("[Proxy] Sending multipart request to backend...");
        using var response = await client.SendAsync(requestMessage, cancellationToken);
        Console.WriteLine($"[Proxy] Backend response status: {response.StatusCode}");
        
        var responseContent = await response.Content.ReadAsByteArrayAsync(cancellationToken);
        Response.StatusCode = (int)response.StatusCode;
        
        if (responseContent.Length == 0)
        {
            return new EmptyResult();
        }

        return File(
            responseContent,
            response.Content.Headers.ContentType?.ToString() ?? "application/json"
        );
    }

    [AcceptVerbs("GET", "POST", "PUT", "PATCH", "DELETE")]
    [Route("{**path}")]
    public Task<IActionResult> Forward(
        string path,
        CancellationToken cancellationToken
    )
    {
        Console.WriteLine($"[Proxy] Wildcard Forward hit for path: {path}");
        return ForwardAsync(
            $"api/{path}{Request.QueryString}",
            forwardCookies: false,
            cancellationToken
        );
    }
}
