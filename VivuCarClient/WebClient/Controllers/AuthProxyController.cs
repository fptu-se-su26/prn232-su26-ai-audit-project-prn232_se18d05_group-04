using Microsoft.AspNetCore.Mvc;

namespace WebClient.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthProxyController(IHttpClientFactory httpClientFactory)
    : ProxyControllerBase(httpClientFactory)
{
    [AcceptVerbs("GET", "POST")]
    [Route("{**path}")]
    public Task<IActionResult> ForwardAuth(
        string path,
        CancellationToken cancellationToken
    )
    {
        return ForwardAsync(
            $"api/auth/{path}{Request.QueryString}",
            forwardCookies: true,
            cancellationToken
        );
    }
}
