using Microsoft.Extensions.DependencyInjection;
using UrlShortener.Modules.Search.Repositories;
using UrlShortener.Modules.Urls.Repositories;
using UrlShortener.Modules.Urls.Services;

namespace UrlShortener.Modules.Urls;

public static class UrlModuleRegistration
{
    public static IServiceCollection AddUrlsModule(this IServiceCollection services)
    {
        services.AddScoped<IUrlRepository, UrlRepository>();
        services.AddScoped<ISearchRepository, UrlRepository>();
        services.AddScoped<IUrlService, UrlService>();
        return services;
    }
}
