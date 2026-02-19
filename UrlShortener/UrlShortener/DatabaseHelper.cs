using Microsoft.Data.Sqlite;
using System.Data;
using UrlShortener.Interfaces;
namespace UrlShortener;

public class DatabaseHelper : IDatabaseHelper
{
    private readonly string _connectionString;

    public DatabaseHelper(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Sqlite");

        ArgumentNullException.ThrowIfNullOrWhiteSpace(connectionString);

        this._connectionString = connectionString;
    }

    // note in a real application this shouldn't be a test database 
    // however for demo purposes sqlite will do
    public IDbConnection GetConnection() => new SqliteConnection (_connectionString);
}