namespace UrlShortener.Modules.Search.Models;

public record SearchResult(int Id, string OriginalUrl, string ShortCode, DateTime CreatedAt);
