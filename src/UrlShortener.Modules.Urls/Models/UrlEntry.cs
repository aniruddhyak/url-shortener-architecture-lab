namespace UrlShortener.Modules.Urls.Models;

public class UrlEntry
{
    public int Id { get; set; }

    public required string OriginalUrl { get; set; }

    public required string ShortCode { get; set; }

    public DateTime CreatedAt { get; set; }
}
