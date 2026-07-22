using Microsoft.AspNetCore.Mvc;

namespace WebClient.Controllers;

[ApiController]
[Route("api/proxy")]
public class ApiProxyController(IHttpClientFactory httpClientFactory)
    : ProxyControllerBase(httpClientFactory)
{
    [AcceptVerbs("GET", "POST", "PUT", "PATCH", "DELETE")]
    [Route("{**path}")]
    public Task<IActionResult> Forward(
        string path,
        CancellationToken cancellationToken
    )
    {
        return ForwardAsync(
            $"api/{path}{Request.QueryString}",
            forwardCookies: false,
            cancellationToken
        );
    }
}
