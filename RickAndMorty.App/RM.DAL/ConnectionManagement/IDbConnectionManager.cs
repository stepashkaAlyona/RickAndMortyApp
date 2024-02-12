using Microsoft.Data.Sqlite;

namespace RM.DAL.ConnectionManagement
{
    public interface IDbConnectionManager
    {
        SqliteConnection GetConnection();
    }
}