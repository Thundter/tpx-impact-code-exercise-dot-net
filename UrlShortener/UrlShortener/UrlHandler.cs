using UrlShortener.Controllers;
using UrlShortener.Interfaces;

namespace UrlShortener;

public class UrlHandler(IConfiguration configuration, IUrlRepository dataLayer, IHelper helper) : IUrlHandler
{
    public async Task<bool> Delete(string alias)
    {
        try
        {
            if (await dataLayer.ExistsByAlias(alias))
            {
                return await dataLayer.DeleteAsync(alias);
            }

            return false;
        }
        catch
        {
            return false;
        }
    }

    public async Task<ICollection<UrlItem>?> GetAll()
    {
        try
        {
            return Translate(await dataLayer.GetAllAsync());
        }
        catch
        {
            return null;
        }
    }

    public async Task<UrlItem?> GetByAlias(string alias)
    {
        try
        {
            var item = await dataLayer.GetByAlias(alias);

            if (item == null) return null;

            return Translate(item);
        }
        catch
        {
            return null;
        }
    }

    public async Task<UrlItem?> Shorten(string fullUrl, string customAlias)
    {
        if (string.IsNullOrWhiteSpace(fullUrl)) return null;

        try
        {
            if (string.IsNullOrWhiteSpace(customAlias))
            {
                customAlias = helper.CreateRandomAlias();
            }
            else
            {
                // if exists is error, return null
                if (await dataLayer.ExistsByAlias(customAlias))
                {
                    return null;
                }
            }

            if (await dataLayer.AddAsync(new() { Alias = customAlias, FullUrl = fullUrl }) > 0)
            {
                return new UrlItem
                {
                    Alias = customAlias,
                    FullUrl = fullUrl,
                    ShortUrl = AliasToShortUrl(customAlias)
                };
            }
            else
            {
                return null;
            }
        }
        catch
        {
            return null;
        }
    }

    private string AliasToShortUrl(string alias)
    {
        var shortUrl = configuration.GetValue<string>("Settings:UrlHandler:AliasToShortUrl");

        ArgumentNullException.ThrowIfNullOrWhiteSpace(shortUrl);

        return string.Concat(shortUrl, alias);
    }

    #region " mappers "
    private ICollection<UrlItem> Translate(IEnumerable<UrlDto> urlDtoItems) => [.. urlDtoItems.Select(Translate)];

    private UrlItem Translate(UrlDto urlDto)
         => new() { Alias = urlDto.Alias, FullUrl = urlDto.FullUrl, ShortUrl = AliasToShortUrl(urlDto.Alias) };

    #endregion " mappers "
}
