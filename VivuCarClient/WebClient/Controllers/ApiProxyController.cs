using Microsoft.AspNetCore.Mvc;

namespace WebClient.Controllers;

[ApiController]
[Route("api/proxy")]
public class ApiProxyController(IHttpClientFactory httpClientFactory) : ControllerBase
{
    [HttpGet("{**path}")]
    public async Task<IActionResult> Get(string path, CancellationToken cancellationToken)
    {
        var client = httpClientFactory.CreateClient("VivuCarApi");
        using var response = await client.GetAsync(path + Request.QueryString, cancellationToken);
        var content = await response.Content.ReadAsByteArrayAsync(cancellationToken);
        Response.StatusCode = (int)response.StatusCode;

        return File(
            content,
            response.Content.Headers.ContentType?.ToString() ?? "application/json");
    }
}
