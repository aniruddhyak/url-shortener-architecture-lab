using Microsoft.Extensions.DependencyInjection;
using UrlShortener.Modules.Search.Repositories;
using UrlShortener.Modules.Search.Services;

namespace UrlShortener.Modules.Search;

public static class SearchModuleRegistration
{
    public static IServiceCollection AddSearchModule(this IServiceCollection services)
    {
        services.AddScoped<ISearchService, SearchService>();
        return services;
    }
}
