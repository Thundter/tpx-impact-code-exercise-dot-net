using Dapper;
using Moq;
using System.Data;
using UrlShortener.Interfaces;

namespace UrlShortener.Tests;

[TestClass]
public class UrlRepositoryTests
{
    private IDbConnection _connection;
    private Mock<IDatabaseHelper> _dbHelperMock;

    [TestInitialize]
    public void Setup()
    {
        // Use a real in-memory connection
        _connection = new Microsoft.Data.Sqlite.SqliteConnection("Data Source=:memory:") as IDbConnection;
        _connection.Open();

        // Create the table schema for the test
        _connection.Execute(@"
        CREATE TABLE IF NOT EXISTS urls (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            alias TEXT NOT NULL UNIQUE,
            fullurl TEXT NOT NULL
        );");

        _dbHelperMock = new Mock<IDatabaseHelper>();
        _dbHelperMock.Setup(m => m.GetConnection()).Returns(_connection);
    }

    [TestCleanup]
    public void Teardown() => _connection.Dispose();

    #region " GetByAlias "

    [TestMethod]
    public async Task GetByAlias_WhenExists_ReturnsRecord()
    {
        // Arrange
        _connection.Execute("INSERT INTO Urls (alias, fullurl) VALUES ('test', 'https://google.com')");
        var repo = new UrlRepository(_dbHelperMock.Object);

        // Act
        var result = await repo.GetByAlias("test");

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("https://google.com", result.FullUrl);
    }

    [TestMethod]
    public async Task GetByAlias_WhenAliasDoesNotExist_ReturnsNull()
    {
        // Arrange
        // DB is empty
        var repo = new UrlRepository(_dbHelperMock.Object);

        // Act
        var result = await repo.GetByAlias("non-existent-alias");

        // Assert
        Assert.IsNull(result, "Should return null when no matching alias is found.");
    }

    [TestMethod]
    public async Task GetByAlias_IsCaseSensitive()
    {
        // Arrange
        await _connection.ExecuteAsync("INSERT INTO Urls (alias, fullurl) VALUES ('MyAlias', 'https://test.com')");
        var repo = new UrlRepository(_dbHelperMock.Object);

        // Act
        var result = await repo.GetByAlias("myalias"); // Lowercase lookup

        // Assert
        // In SQLite this is usually case-sensitive by default
        Assert.IsNull(result, "Search should be case-sensitive.");
    }

    [TestMethod]
    public async Task GetByAlias_MapsAllFieldsCorrectly()
    {
        // Arrange
        // Use a DTO with multiple properties
        var expected = new UrlDto { Alias = "test", FullUrl = "https://a.com" };

        await _connection.ExecuteAsync(
            "INSERT INTO Urls (alias, fullurl) VALUES (@Alias, @FullUrl)",
            expected);


        var repo = new UrlRepository(_dbHelperMock.Object);

        // Act
        var result = await repo.GetByAlias("test");

        // Assert
        Assert.AreEqual(expected.FullUrl, result?.FullUrl);
    }

    #endregion " GetByAlias "

    #region "  "

    // todo 

    #endregion "  "


}
