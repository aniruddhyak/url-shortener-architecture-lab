namespace UrlShortener.Modules.Search.Models;

public record UrlResponse(int Id, string OriginalUrl, string ShortCode, DateTime CreatedAt);
