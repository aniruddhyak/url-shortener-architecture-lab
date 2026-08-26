using System.Security.Cryptography;
using UrlShortener.Modules.Urls.Models;
using UrlShortener.Modules.Urls.Repositories;

namespace UrlShortener.Modules.Urls.Services;

public class UrlService(IUrlRepository repository) : IUrlService
{
    private const string Alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    private const int ShortCodeLength = 6;

    public async Task<UrlEntry> CreateAsync(string originalUrl, CancellationToken cancellationToken = default)
    {
        string shortCode;
        do
        {
            shortCode = GenerateShortCode();
        } while (await repository.ShortCodeExistsAsync(shortCode, cancellationToken));

        return await repository.AddAsync(new UrlEntry
        {
            OriginalUrl = originalUrl,
            ShortCode = shortCode,
            CreatedAt = DateTime.UtcNow
        }, cancellationToken);
    }

    public Task<UrlEntry?> GetByShortCodeAsync(string shortCode, CancellationToken cancellationToken = default) =>
        repository.GetByShortCodeAsync(shortCode, cancellationToken);

    private static string GenerateShortCode()
    {
        Span<byte> randomBytes = stackalloc byte[ShortCodeLength];
        RandomNumberGenerator.Fill(randomBytes);
        Span<char> code = stackalloc char[ShortCodeLength];

        for (var index = 0; index < code.Length; index++)
        {
            code[index] = Alphabet[randomBytes[index] % Alphabet.Length];
        }

        return new string(code);
    }
}
