using System.Text;

namespace UrlShortener;

public class Helper(IConfiguration configuration) : IHelper
{
    public string CreateRandomAlias()
    {
        var chars = configuration.GetValue<string>("Settings:CreateRandomAlias:Chars");
        var length = configuration.GetValue<int>("Settings:CreateRandomAlias:Length");

        ArgumentNullException.ThrowIfNullOrWhiteSpace(chars);

        var random = new Random();
        var result = new StringBuilder(length);

        for (int i = 0; i < length; i++)
        {
            result.Append(chars[random.Next(chars.Length)]);
        }

        return result.ToString();
    }
}
