using Dapper;
using Moq;
using System.Data;
using UrlShortener.Interfaces;

namespace UrlShortener.Tests;

[TestClass]
public class UrlRepositoryTests
{
    private IDbConnection? _connection;
    private Mock<IDatabaseHelper>? _dbHelperMock;
    private IUrlRepository? _repo;

    [TestInitialize]
    public void Setup()
    {
        // Use a real in-memory connection
        _connection = new Microsoft.Data.Sqlite.SqliteConnection("Data Source=:memory:");
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

        _repo = new UrlRepository(_dbHelperMock.Object);
    }

    [TestCleanup]
    public void Teardown() => _connection?.Dispose();

    #region " GetByAlias "

    [TestMethod]
    public async Task GetByAlias_WhenExists_ReturnsRecord()
    {
        ArgumentNullException.ThrowIfNull(_connection);
        ArgumentNullException.ThrowIfNull(_repo);

        // Arrange
        _connection.Execute("INSERT INTO Urls (alias, fullurl) VALUES ('test', 'https://google.com')");

        // Act
        var result = await _repo.GetByAlias("test");

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("https://google.com", result.FullUrl);
    }

    [TestMethod]
    public async Task GetByAlias_WhenAliasDoesNotExist_ReturnsNull()
    {
        ArgumentNullException.ThrowIfNull(_connection);
        ArgumentNullException.ThrowIfNull(_repo);

        // Arrange

        // Act
        var result = await _repo.GetByAlias("non-existent-alias");

        // Assert
        Assert.IsNull(result, "Should return null when no matching alias is found.");
    }

    [TestMethod]
    public async Task GetByAlias_IsCaseSensitive()
    {
        ArgumentNullException.ThrowIfNull(_connection);
        ArgumentNullException.ThrowIfNull(_dbHelperMock);

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
        ArgumentNullException.ThrowIfNull(_connection);
        ArgumentNullException.ThrowIfNull(_repo);

        // Arrange
        // Use a DTO with multiple properties
        var expected = new UrlDto { Alias = "test", FullUrl = "https://a.com" };

        await _connection.ExecuteAsync(
            "INSERT INTO Urls (alias, fullurl) VALUES (@Alias, @FullUrl)",
            expected);

        // Act
        var result = await _repo.GetByAlias("test");

        // Assert
        Assert.AreEqual(expected.FullUrl, result?.FullUrl);
    }

    #endregion " GetByAlias "

    #region " GetAllAsync "

    [TestMethod]
    public async Task GetAllAsync_ReturnsExpectedData()
    {
        ArgumentNullException.ThrowIfNull(_connection);
        ArgumentNullException.ThrowIfNull(_repo);

        // Arrange
        _connection.Execute("INSERT INTO Urls (alias, fullurl) VALUES ('testa', 'https://a.com')");
        _connection.Execute("INSERT INTO Urls (alias, fullurl) VALUES ('testb', 'https://b.com')");
        _connection.Execute("INSERT INTO Urls (alias, fullurl) VALUES ('testc', 'https://c.com')");

        // Act
        var result = await _repo.GetAllAsync();

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(3, result.Count());
        Assert.AreEqual("testa", result.First().Alias);
        Assert.AreEqual("https://c.com", result.Last().FullUrl);
    }

    #endregion " GetAllAsync "

    #region " AddAsync "

    [TestMethod]
    public async Task AddAsync_ReturnsAffectedRows()
    {
        // Arrange
        ArgumentNullException.ThrowIfNull(_connection);
        ArgumentNullException.ThrowIfNull(_repo);

        var urlDto = new UrlDto { Alias = "testa", FullUrl = "https://a.com" };

        // Act
        var result = await _repo.AddAsync(urlDto);

        // Assert
        Assert.AreEqual(1, result);
    }

    #endregion " AddAsync "

    #region " DeleteAsync "

    [TestMethod]
    [DataRow(1, true)]  // 1 row affected -> returns true
    [DataRow(0, false)] // 0 rows affected -> returns false
    public async Task DeleteAsync_ReturnsCorrectBoolBasedOnRows(int rows, bool expected)
    {
        ArgumentNullException.ThrowIfNull(_connection);
        ArgumentNullException.ThrowIfNull(_repo);

        // Arrange
        if (rows > 0)
        {
            _connection.Execute("INSERT INTO Urls (alias, fullurl) VALUES ('testa', 'https://a.com')");
        }

        // Act
        var result = await _repo.DeleteAsync("testa");

        // Assert
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow(" ")]
    public async Task DeleteAsync_ThrowsWhenAliasInvalid(string? invalidAlias)
    {
        ArgumentNullException.ThrowIfNull(_repo);

        // Act
        // workaround due to await
        try
        {
            await _repo.DeleteAsync(invalidAlias!);
            Assert.Fail("The expected exception was not thrown.");
        }
        catch (Exception ex)
        {
            // Assert
            // Exception was caught as expected
            // "" & " " throws ArgumentException, null throw ArgumentNullException 😩
            Assert.IsInstanceOfType(ex, typeof(ArgumentException));
        }
    }

    #endregion " DeleteAsync "

}
