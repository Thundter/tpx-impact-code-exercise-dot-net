using UrlShortener.Controllers;

namespace UrlShortener;

public interface IUrlRepository
{
    Task<bool> DeleteAsync(string alias);
    Task<bool> ExistsByAlias(string alias);
    Task<IEnumerable<UrlDto>> GetAllAsync();
    Task<UrlDto?> GetByAlias(string alias);
    Task<int> AddAsync(UrlDto urlDto);
}
