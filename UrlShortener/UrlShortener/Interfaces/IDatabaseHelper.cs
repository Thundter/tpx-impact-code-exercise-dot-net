using System.Data;

namespace UrlShortener.Interfaces;

public interface IDatabaseHelper
{
    IDbConnection GetConnection();

}