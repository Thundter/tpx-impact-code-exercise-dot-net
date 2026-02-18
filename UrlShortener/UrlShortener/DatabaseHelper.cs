using Microsoft.Data.Sqlite;
namespace UrlShortener;

public class DatabaseHelper
{
    private readonly string _connectionString = $"Data Source=database.db;Version=3;";

    public DatabaseHelper(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Sqlite");

        ArgumentNullException.ThrowIfNullOrWhiteSpace(connectionString);

        this._connectionString = connectionString;
    }

    public SqliteConnection GetConnection() => new(_connectionString);
}