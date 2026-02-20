using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using UrlShortener.Interfaces;

namespace UrlShortener.Controllers;

[ApiController]
[Route("[controller]")]
public class UrlController(IUrlHandler urlHandler) : ControllerBase, IUrlController
{
    /// <summary>
    /// Delete a shortened URL
    /// </summary>
    /// <param name="alias"></param>
    /// <returns>Task</returns>
    /// <remarks>
    /// tested
    /// </remarks>
    [HttpDelete("{alias}")]
    public async Task DeleteAsync([Required] string alias, CancellationToken cancellationToken = default)
    {
        if (await urlHandler.Delete(alias))
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
    /// tested
    /// this works as an independant url, however it doesn't work in swagger
    /// </remarks>
    [HttpGet("{alias}")]
    public async Task GetUrlByAliasAsync([Required] string alias, CancellationToken cancellationToken = default)
    {
        UrlItem? shortenedUrl = await urlHandler.GetByAlias(alias);

        if (shortenedUrl == null)
        {
            // '404': description: Alias not found
            Response.StatusCode = StatusCodes.Status404NotFound;
            return;
        }

        // '302': description: Redirect to the original URL
        Response.StatusCode = 302;
        Response.Headers.Location = shortenedUrl.FullUrl;
        return;
    }

    /// <summary>
    /// Shorten a URL
    /// </summary>
    /// <param name="fullUrl"></param>
    /// <param name="customUrl"></param>
    /// <returns>Task<UrlResponse></returns>
    /// <remarks>
    /// v A shortened URL should have a randomly generated alias.
    /// v Allow a user to **customise the shortened URL** if they want to 
    /// (e.g. user provides `my-custom-alias` instead of a random string).
    /// v Persist the shortened URLs across restarts
    ///   database
    /// v The API should validate inputs and handle errors gracefully.
    /// tested
    /// </remarks>
    [HttpPost(Name = "shorten")]
    public async Task<UrlResponse> ShortenAsync(UrlBody body, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(body.FullUrl);

        UrlItem? shortenedUrl = await urlHandler.Shorten(body.FullUrl, body.CustomAlias);

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
    /// tested
    /// </remarks>
    /// <summary>
    /// List all shortened URLs
    /// </summary>
    /// <returns>Task<ICollection<UrlItem>></returns>
    [HttpGet(Name = "urls")]
    public async Task<ICollection<UrlItem>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var output = await urlHandler.GetAll(); // status 200

        if (output == null) return [];

        return output;
    }

}

// - Fork the repository and work in your fork
//   Do not push directly to the main repository
// - Use the provided[`openapi.yaml`](./openapi.yaml) as the API contract

// done - Be containerised (e.g. Docker)
//      - Dockerfile
// todo - Focus on clean, maintainable code
// todo - Include instructions for running locally
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
