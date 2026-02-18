using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace UrlShortener.Controllers;

[ApiController]
[Route("[controller]")]
public class UrlController : ControllerBase, IUrlController
{
    private readonly ILogger<UrlController> _logger;
    private readonly IUrlHandler _urlHandler;

    // todo dependency injection

    public UrlController(
        ILogger<UrlController> logger,
        IUrlHandler urlHandler)
    {
        _logger = logger;
        _urlHandler = urlHandler;
    }

    /// <summary>
    /// Delete a shortened URL
    /// </summary>
    /// <param name="alias"></param>
    /// <returns>Task</returns>
    /// <remarks>
    /// todo needs testing
    /// </remarks>
    [HttpDelete("{alias:string}")]
    public async Task DeleteAsync([Required] string alias)
    {
        if (await _urlHandler.Delete(alias))
        {
            // '204': description: Successfully deleted
            Response.StatusCode = StatusCodes.Status204NoContent;
            return;
        }

        // '404': description: Alias not found
        Response.StatusCode = StatusCodes.Status404NotFound;
        return;
    }

    /// <summary>
    /// Redirect to full URL
    /// </summary>
    /// <param name="alias"></param>
    /// <returns>Task</returns>
    /// <remarks>
    /// todo needs testing
    /// </remarks>
    [HttpGet("{alias:string}")]
    public async Task GetAsync([Required] string alias)
    {
        UrlItem? shortenedUrl = await _urlHandler.GetByAlias(alias);

        if (shortenedUrl == null)
        {
            // '404': description: Alias not found
            Response.StatusCode = StatusCodes.Status404NotFound;
            return;
        }

        // '302': description: Redirect to the original URL
        Response.Redirect(shortenedUrl.FullUrl);
        return;
    }

    /// <summary>
    /// Shorten a URL
    /// </summary>
    /// <param name="fullUrl"></param>
    /// <param name="customUrl"></param>
    /// <returns>Task<UrlResponse></returns>
    /// <remarks>
    /// - A shortened URL should have a randomly generated alias.
    /// - Allow a user to **customise the shortened URL** if they want to 
    /// (e.g. user provides `my-custom-alias` instead of a random string).
    /// - Persist the shortened URLs across restarts
    ///   database
    /// - The API should validate inputs and handle errors gracefully.
    /// todo needs testing
    /// </remarks>
    [HttpPost(Name = "shorten")]
    public async Task<UrlResponse> ShortenAsync(UrlBody body)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(body.FullUrl);

        UrlItem? shortenedUrl = await _urlHandler.Shorten(body.FullUrl, body.CustomAlias);

        if (shortenedUrl == null)
        {
            // '400': description: Invalid input or alias already taken         
            Response.StatusCode = StatusCodes.Status400BadRequest;
            return new UrlResponse();
        }

        // 201: URL successfully shortened
        Response.StatusCode = StatusCodes.Status201Created;
        return new UrlResponse { ShortUrl = shortenedUrl.ShortUrl };
    }

    /// <remarks>
    /// todo needs testing
    /// </remarks>
    /// <summary>
    /// List all shortened URLs
    /// </summary>
    /// <returns>Task<ICollection<UrlItem>></returns>
    [HttpGet(Name = "urls")]
    public async Task<ICollection<UrlItem>> UrlsAsync()
    {
        var output = await _urlHandler.GetAll(); // status 200

        if (output == null) return [];

        return output;
    }
}

// todo - Fork the repository and work in your fork
//        Do not push directly to the main repository
// todo - Be containerised (e.g. Docker)
//      - Dockerfile
// todo - Include instructions for running locally
// done - Use the provided[`openapi.yaml`](./openapi.yaml) as the API contract
// todo - Focus on clean, maintainable code
// todo - readme
//          - How to build and run locally
//          - Example usage (frontend and API)
//          - Any notes or assumptions

// front end
// - should show errors from the API appropriately.
// - Decoupled web frontend using React.

// ## Rules
// - There is no time limit, we want to see something you are proud of
//   We would like to understand roughly how long you spent on it though
// - **Commit often with meaningful messages**
// - AI tools (e.g., GitHub Copilot, ChatGPT) are allowed, but please **do not** copy-paste large chunks of code
//   Use them as assistants, not as a replacement for your own work
//   We will be asking

// ## Deliverables
// - Working software
// - A git commit history that shows your thought process
