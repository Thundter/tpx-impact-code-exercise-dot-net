using Microsoft.AspNetCore.Mvc;

namespace UrlShortener.Controllers;

[ApiController]
[Route("[controller]")]
public class UrlController : ControllerBase, IUrlController
{
    public UrlController()
    {
        
    }

    public Task DeleteAsync(string alias)
    {
        throw new NotImplementedException();
    }

    public Task GetAsync(string alias)
    {
        throw new NotImplementedException();
    }

    public Task<UrlResponse> ShortenAsync(UrlBody body)
    {
        throw new NotImplementedException();
    }

    public Task<ICollection<UrlItem>> UrlsAsync()
    {
        throw new NotImplementedException();
    }
}
