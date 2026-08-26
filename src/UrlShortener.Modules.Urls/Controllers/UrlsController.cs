using Microsoft.AspNetCore.Mvc;
using UrlShortener.Modules.Urls.Models;
using UrlShortener.Modules.Urls.Services;

namespace UrlShortener.Modules.Urls.Controllers;

[ApiController]
[Route("urls")]
public class UrlsController(IUrlService urlService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<object>> Create(CreateUrlRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Url) ||
            !Uri.TryCreate(request.Url, UriKind.Absolute, out var uri) ||
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        {
            return BadRequest("Url must be a valid absolute HTTP or HTTPS URL.");
        }

        var entry = await urlService.CreateAsync(request.Url, cancellationToken);
        return Ok(new { shortCode = entry.ShortCode });
    }

    [HttpGet("{shortCode}")]
    public async Task<ActionResult<string>> Get(string shortCode, CancellationToken cancellationToken)
    {
        var entry = await urlService.GetByShortCodeAsync(shortCode, cancellationToken);
        return entry is null ? NotFound() : Ok(entry.OriginalUrl);
    }
}
