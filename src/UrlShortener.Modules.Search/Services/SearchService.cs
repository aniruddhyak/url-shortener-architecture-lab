using UrlShortener.Modules.Search.Models;
using UrlShortener.Modules.Search.Repositories;

namespace UrlShortener.Modules.Search.Services;

public class SearchService(ISearchRepository repository) : ISearchService
{
    public Task<IEnumerable<SearchResult>> SearchAsync(string query, CancellationToken cancellationToken = default) =>
        repository.SearchAsync(query, cancellationToken);
}
