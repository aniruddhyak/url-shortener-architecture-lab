using Microsoft.AspNetCore.Mvc;
using UrlShortener.Modules.Search.Models;
using UrlShortener.Modules.Search.Services;

namespace UrlShortener.Modules.Search.Controllers;

[ApiController]
public class SearchController(ISearchService searchService) : ControllerBase
{
    [HttpGet("search")]
    public async Task<ActionResult<IReadOnlyList<UrlResponse>>> Search([FromQuery] string? q, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(q))
        {
            return BadRequest("The q query parameter is required.");
        }

        var entries = await searchService.SearchAsync(q.Trim(), cancellationToken);
        return Ok(entries.Select(entry => new UrlResponse(
            entry.Id, entry.OriginalUrl, entry.ShortCode, entry.CreatedAt)));
    }
}
