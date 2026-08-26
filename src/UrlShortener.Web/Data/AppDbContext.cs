using Microsoft.EntityFrameworkCore;
using UrlShortener.Modules.Urls.Models;

namespace UrlShortener.Web.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<UrlEntry> UrlEntries => Set<UrlEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UrlEntry>(entity =>
        {
            entity.HasKey(url => url.Id);
            entity.Property(url => url.OriginalUrl).IsRequired();
            entity.Property(url => url.ShortCode).IsRequired();
            entity.HasIndex(url => url.ShortCode).IsUnique();
            entity.Property(url => url.CreatedAt).IsRequired();
        });
    }
}
