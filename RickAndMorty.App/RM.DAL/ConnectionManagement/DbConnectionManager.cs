using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;

using RM.Common;

namespace RM.DAL.ConnectionManagement
{
    public class DbConnectionManager : IDbConnectionManager
    {
        private readonly string _connectionString;

        public DbConnectionManager(IOptions<SettingsModel> settings)
        {
            _connectionString = settings.Value.DbConnStr;
            SQLitePCL.Batteries.Init();
        }

        public SqliteConnection GetConnection() => new(_connectionString);
    }
}