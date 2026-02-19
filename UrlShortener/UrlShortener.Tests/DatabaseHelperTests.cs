using Microsoft.Extensions.Configuration;

namespace UrlShortener.Tests;

[TestClass]
public class DatabaseHelperTests
{
    [TestMethod]
    public void Constructor_ValidConnectionString_ShouldSucceed()
    {
        // Arrange
        // Create a configuration with the required "Sqlite" key
        var inMemorySettings = new Dictionary<string, string?> {
            {"ConnectionStrings:Sqlite", "Data Source=:memory:"}
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        // Act
        var helper = new DatabaseHelper(configuration);
        using var connection = helper.GetConnection();

        // Assert
        // Verify connection string was mapped correctly
        Assert.IsNotNull(connection, "Connection should not be null.");
        Assert.AreEqual("Data Source=:memory:", connection.ConnectionString);
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow("   ")]
    public void Constructor_InvalidConnectionString_ShouldThrowArgumentException(string? invalidConnString)
    {
        // Arrange
        var inMemorySettings = new Dictionary<string, string?> {
            {"ConnectionStrings:Sqlite", invalidConnString}
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
        {
            _ = new DatabaseHelper(configuration);
        });
    }

}
