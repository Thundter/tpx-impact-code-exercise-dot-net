using Microsoft.Extensions.Configuration;

namespace UrlShortener.Tests;

[TestClass]
public class HelperTests
{
    private IConfiguration CreateConfig(string? chars, int length)
    {
        var inMemorySettings = new Dictionary<string, string?>
        {
            {"Settings:CreateRandomAlias:Chars", chars},
            {"Settings:CreateRandomAlias:Length", length.ToString()}
        };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();
    }

    [TestMethod]
    public void CreateRandomAlias_ReturnsCorrectLength()
    {
        // Arrange
        int expectedLength = 8;
        var config = CreateConfig("ABCDEF", expectedLength);
        var helper = new Helper(config);

        // Act
        var result = helper.CreateRandomAlias();

        // Assert
        Assert.AreEqual(expectedLength, result.Length);
    }

    [TestMethod]
    public void CreateRandomAlias_OnlyUsesConfiguredChars()
    {
        // Arrange
        string allowedChars = "A";
        var config = CreateConfig(allowedChars, 5);
        var helper = new Helper(config);

        // Act
        var result = helper.CreateRandomAlias();

        // Assert
        Assert.AreEqual("AAAAA", result);
    }

    [TestMethod]
    public void CreateRandomAlias_ThrowsWhenCharsMissing()
    {
        // Arrange
        var config = CreateConfig(null, 5);
        var helper = new Helper(config);

        // Act & Assert
        Assert.Throws<ArgumentException>(helper.CreateRandomAlias);
    }
}
