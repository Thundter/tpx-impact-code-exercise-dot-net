using UrlShortener.Controllers;

namespace UrlShortener.Interfaces;

public interface IUrlHandler
{
    Task<bool> Delete(string alias);
    Task<ICollection<UrlItem>?> GetAll();
    Task<UrlItem?> GetByAlias(string alias);
    Task<UrlItem?> Shorten(string fullUrl, string customAlias);
}
