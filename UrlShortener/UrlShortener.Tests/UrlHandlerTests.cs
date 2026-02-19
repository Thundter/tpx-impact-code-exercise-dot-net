using Microsoft.Extensions.Configuration;
using Moq;
using UrlShortener.Interfaces;

namespace UrlShortener.Tests;

/// <summary>
/// UrlHandler tests
/// </summary>
[TestClass]
public sealed class UrlHandlerTests
{
    private Mock<IUrlRepository>? _mockRepo;
    private Mock<IHelper>? _mockHelper;
    private UrlHandler? _urlHandler;

    private IEnumerable<UrlDto>? repoItems;

    [TestInitialize]
    public void TestInit()
    {
        _mockRepo = new Mock<IUrlRepository>();
        _mockHelper = new Mock<IHelper>();

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                {"Settings:UrlHandler:AliasToShortUrl", "http://localhost:5284/url/"}
            })
            .Build();

        _urlHandler = new UrlHandler(configuration, _mockRepo.Object, _mockHelper.Object);
    }

    #region " Delete "

    [TestMethod]
    public async Task Delete_WhenAliasExists_ReturnsTrueOnSuccess()
    {
        ArgumentNullException.ThrowIfNull(_mockRepo);
        ArgumentNullException.ThrowIfNull(_urlHandler);

        // Arrange
        var alias = "test-alias";
        _mockRepo.Setup(r => r.ExistsByAlias(alias)).ReturnsAsync(true);
        _mockRepo.Setup(r => r.DeleteAsync(alias)).ReturnsAsync(true);

        // Act
        var result = await _urlHandler.Delete(alias);

        // Assert
        Assert.IsTrue(result);
        _mockRepo.Verify(r => r.DeleteAsync(alias), Times.Once);
    }

    [TestMethod]
    public async Task Delete_WhenAliasDoesNotExist_ReturnsFalseWithoutCallingDelete()
    {
        ArgumentNullException.ThrowIfNull(_mockRepo);
        ArgumentNullException.ThrowIfNull(_urlHandler);

        // Arrange
        var alias = "non-existent";
        _mockRepo.Setup(r => r.ExistsByAlias(alias)).ReturnsAsync(false);

        // Act
        var result = await _urlHandler.Delete(alias);

        // Assert
        Assert.IsFalse(result);
        _mockRepo.Verify(r => r.DeleteAsync(It.IsAny<string>()), Times.Never);
    }

    [TestMethod]
    public async Task Delete_WhenRepositoryThrowsException_ReturnsFalse()
    {
        ArgumentNullException.ThrowIfNull(_mockRepo);
        ArgumentNullException.ThrowIfNull(_urlHandler);

        // Arrange
        var alias = "error-alias";
        _mockRepo.Setup(r => r.ExistsByAlias(alias))
                 .ThrowsAsync(new System.Exception("Database Connection Failed"));

        // Act
        var result = await _urlHandler.Delete(alias);

        // Assert
        Assert.IsFalse(result);
    }

    #endregion " Delete "

    #region " GetAll "

    [TestMethod]
    public async Task GetAll_WhenDataExists_ReturnsTranslatedCollection()
    {
        ArgumentNullException.ThrowIfNull(_mockRepo);
        ArgumentNullException.ThrowIfNull(_urlHandler);

        // Arrange
        repoItems = [
            new() { Alias = "abc", FullUrl = "https://google.com" },
            new() { Alias = "xyz", FullUrl = "https://bing.com" }
        ];

        _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(repoItems);

        // Act
        var result = await _urlHandler.GetAll();

        // Assert
        Assert.IsNotNull(result, "Result should not be null");
        Assert.HasCount(2, result);
        _mockRepo.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [TestMethod]
    public async Task GetAll_WhenRepositoryThrowsException_ReturnsNull()
    {
        ArgumentNullException.ThrowIfNull(_mockRepo);
        ArgumentNullException.ThrowIfNull(_urlHandler);

        // Arrange: Force an exception from the repository
        _mockRepo.Setup(r => r.GetAllAsync())
                 .ThrowsAsync(new System.Exception("Critical DB failure"));

        // Act
        var result = await _urlHandler.GetAll();

        // Assert
        Assert.IsNull(result, "Result should be null");
    }

    #endregion " GetAll "

    #region " GetByAlias "

    [TestMethod]
    public async Task GetByAlias_WhenAliasExists_ReturnsTranslatedUrlItem()
    {
        ArgumentNullException.ThrowIfNull(_mockRepo);
        ArgumentNullException.ThrowIfNull(_urlHandler);

        // Arrange
        var alias = "valid-alias";
        var dto = new UrlDto
        {
            Alias = alias,
            FullUrl = "https://example.com"
        };

        _mockRepo.Setup(r => r.GetByAlias(alias))
                 .ReturnsAsync(dto);

        // Act
        var result = await _urlHandler.GetByAlias(alias);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(alias, result.Alias);
        Assert.AreEqual("https://example.com", result.FullUrl);
        _mockRepo.Verify(r => r.GetByAlias(alias), Times.Once);
    }

    [TestMethod]
    public async Task GetByAlias_WhenAliasDoesNotExist_ReturnsNull()
    {
        ArgumentNullException.ThrowIfNull(_mockRepo);
        ArgumentNullException.ThrowIfNull(_urlHandler);

        // Arrange
        var alias = "ghost-alias";
        // Mocking the repository to return null (simulating record not found)
        _mockRepo.Setup(r => r.GetByAlias(alias))
                 .ReturnsAsync((UrlDto?)null);

        // Act
        var result = await _urlHandler.GetByAlias(alias);

        // Assert
        Assert.IsNull(result, "Should return null if the data layer returns null.");
    }

    [TestMethod]
    public async Task GetByAlias_WhenRepositoryThrows_ReturnsNull()
    {
        ArgumentNullException.ThrowIfNull(_mockRepo);
        ArgumentNullException.ThrowIfNull(_urlHandler);

        // Arrange
        var alias = "crash-alias";
        _mockRepo.Setup(r => r.GetByAlias(alias))
                 .ThrowsAsync(new System.Exception("Connection timeout"));

        // Act
        var result = await _urlHandler.GetByAlias(alias);

        // Assert
        Assert.IsNull(result, "The try-catch block should handle exceptions by returning null.");
    }

    [TestMethod]
    public async Task GetByAlias_WhenAliasIsEmpty_StillCallsRepository()
    {
        ArgumentNullException.ThrowIfNull(_mockRepo);
        ArgumentNullException.ThrowIfNull(_urlHandler);

        // Arrange
        var alias = string.Empty;
        _mockRepo.Setup(r => r.GetByAlias(alias))
                 .ReturnsAsync((UrlDto?)null);

        // Act
        await _urlHandler.GetByAlias(alias);

        // Assert
        _mockRepo.Verify(r => r.GetByAlias(""), Times.Once);
    }

    #endregion " GetByAlias "

    #region " Shorten "

    [TestMethod]
    public async Task Shorten_WhenFullUrlIsEmpty_ReturnsNull()
    {
        ArgumentNullException.ThrowIfNull(_mockRepo);
        ArgumentNullException.ThrowIfNull(_urlHandler);

        // Act
        var result = await _urlHandler.Shorten("", "my-alias");

        // Assert
        Assert.IsNull(result);
        _mockRepo.Verify(r => r.AddAsync(It.IsAny<UrlDto>()), Times.Never);
    }

    [TestMethod]
    public async Task Shorten_WhenCustomAliasExists_ReturnsNull()
    {
        ArgumentNullException.ThrowIfNull(_mockRepo);
        ArgumentNullException.ThrowIfNull(_urlHandler);

        // Arrange
        var alias = "taken";
        _mockRepo.Setup(r => r.ExistsByAlias(alias)).ReturnsAsync(true);

        // Act
        var result = await _urlHandler.Shorten("https://google.com", alias);

        // Assert
        Assert.IsNull(result);
        _mockRepo.Verify(r => r.AddAsync(It.IsAny<UrlDto>()), Times.Never);
    }

    [TestMethod]
    public async Task Shorten_WhenNoAliasProvided_GeneratesRandomAlias()
    {
        ArgumentNullException.ThrowIfNull(_mockRepo);
        ArgumentNullException.ThrowIfNull(_urlHandler);
        ArgumentNullException.ThrowIfNull(_mockHelper);

        // Arrange
        var generated = "random123";
        _mockHelper.Setup(h => h.CreateRandomAlias()).Returns(generated);
        _mockRepo.Setup(r => r.AddAsync(It.IsAny<UrlDto>())).ReturnsAsync(1);

        // Act
        var result = await _urlHandler.Shorten("https://google.com", string.Empty);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(generated, result.Alias);
        _mockHelper.Verify(h => h.CreateRandomAlias(), Times.Once);
    }

    [TestMethod]
    public async Task Shorten_WhenDatabaseSaveFails_ReturnsNull()
    {
        ArgumentNullException.ThrowIfNull(_mockRepo);
        ArgumentNullException.ThrowIfNull(_urlHandler);

        // Arrange
        _mockRepo.Setup(r => r.AddAsync(It.IsAny<UrlDto>())).ReturnsAsync(0);

        // Act
        var result = await _urlHandler.Shorten("https://google.com", "new-alias");

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task Shorten_WhenExceptionOccurs_ReturnsNull()
    {
        ArgumentNullException.ThrowIfNull(_mockRepo);
        ArgumentNullException.ThrowIfNull(_urlHandler);

        // Arrange
        _mockRepo.Setup(r => r.ExistsByAlias(It.IsAny<string>()))
                 .ThrowsAsync(new System.Exception("DB Dead"));

        // Act
        var result = await _urlHandler.Shorten("https://google.com", "alias");

        // Assert
        Assert.IsNull(result);
    }

    #endregion " Shorten "

    #region " Translates "

    [TestMethod]
    public async Task GetAll_WhenDataExists_CorrectlyTranslatesDtosToItems()
    {
        ArgumentNullException.ThrowIfNull(_mockRepo);
        ArgumentNullException.ThrowIfNull(_urlHandler);

        // Arrange
        repoItems = [
            new() { Alias = "abc", FullUrl = "https://banana.com" },
            new() { Alias = "xyz", FullUrl = "https://apple.com" }
        ];

        Console.WriteLine(repoItems.First());
        Console.WriteLine(repoItems.Last());

        // Cast to IEnumerable to satisfy the Moq extension method resolution
        _mockRepo.Setup(r => r.GetAllAsync())
                 .ReturnsAsync(repoItems);

        // Act
        var result = await _urlHandler.GetAll();

        // Assert
        Assert.IsNotNull(result);
        Assert.HasCount(2, result);

        // Verify the translation logic (mapping Alias to Alias, etc.)
        var firstItem = result.First();
        Assert.AreEqual("abc", firstItem.Alias);
        Assert.AreEqual("https://banana.com", firstItem.FullUrl);
        var lastItem = result.Last();
        Assert.AreEqual("xyz", lastItem.Alias);
        Assert.AreEqual("https://apple.com", lastItem.FullUrl);
    }

    #endregion " Translates "

}
