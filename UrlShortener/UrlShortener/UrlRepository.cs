using Dapper;
using UrlShortener.Interfaces;

namespace UrlShortener;

public class UrlRepository(IDatabaseHelper dbHelper) : IUrlRepository
{
    const string GetByAliasSQL = @"
    SELECT *
      FROM Urls
     WHERE alias = @alias";

    public async Task<UrlDto?> GetByAlias(string alias)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(alias);

        using var connection = dbHelper.GetConnection();
        connection.Open();
        return await connection.QueryFirstOrDefaultAsync<UrlDto>(GetByAliasSQL, new { alias });
    }

    const string GetAllSQL = @"
    SELECT *
      FROM Urls";

    public async Task<IEnumerable<UrlDto>> GetAllAsync()
    {
        using var connection = dbHelper.GetConnection();
        connection.Open();
        return await connection.QueryAsync<UrlDto>(GetAllSQL);
    }

    const string AddSQL = @"
    INSERT INTO Urls
         ( alias
         , fullurl )
    VALUES
         ( @Alias
         , @FullUrl )";

    public async Task<int> AddAsync(UrlDto urlDto)
    {
        using var connection = dbHelper.GetConnection();
        connection.Open();
        return await connection.ExecuteAsync(AddSQL, urlDto);
    }

    const string DeleteSQL = @"
    DELETE FROM Urls
     WHERE alias = @alias";

    public async Task<bool> DeleteAsync(string alias)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(alias);

        using var connection = dbHelper.GetConnection();
        connection.Open();
        var rowsAffected = await connection.ExecuteAsync(DeleteSQL, new { alias });
        return rowsAffected > 0;
    }

    const string ExistsByAliasSQL = @"
    SELECT CASE WHEN EXISTS (SELECT 1 
                               FROM Urls 
                              WHERE alias = @alias) 
           THEN 1 ELSE 0 END";

    public async Task<bool> ExistsByAlias(string alias)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(alias);

        using var connection = dbHelper.GetConnection();
        connection.Open();

        return await connection.QueryFirstOrDefaultAsync<bool>(ExistsByAliasSQL, new { alias });
    }
}