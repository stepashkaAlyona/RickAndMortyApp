using Microsoft.Data.Sqlite;

using RM.DAL.ConnectionManagement;
using RM.DAL.Models;

using System.Data;

namespace RM.DAL.CharactersOperations
{
    public class CharDataService : ICharDataService
    {
        private const string TableName = "Characters";

        private const string FullInsertQuery = @$"INSERT INTO {TableName} (Id, Name, Status, Species, Type, Gender, OriginName, LocationName, Image, Episode, Url, Created)
                                           VALUES (@Id, @Name, @Status, @Species, @Type, @Gender, @OriginName, @LocationName, @Image, @Episode, @Url, @Created);";

        private const string IncrementedInsertQuery = @$"INSERT INTO {TableName} (Name, Status, Species, Type, Gender, OriginName, LocationName, Image, Episode, Url, Created)
                                           VALUES (@Name, @Status, @Species, @Type, @Gender, @OriginName, @LocationName, @Image, @Episode, @Url, @Created);
                                            SELECT last_insert_rowid();";

        private const string GetAllQuery = $"SELECT * FROM {TableName}";

        private const string GetByIdQuery = $"SELECT * FROM {TableName} WHERE Id = @Id";

        private const string CreateTableQuery = $@"
                    CREATE TABLE IF NOT EXISTS {TableName} (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT,
                        Status TEXT,
                        Species TEXT,
                        Type TEXT,
                        Gender TEXT,
                        OriginName TEXT,
                        LocationName TEXT,
                        Image TEXT,
                        Episode TEXT,
                        Url TEXT,
                        Created DATETIME
                    );";

        private readonly IDbConnectionManager _conManager;

        public CharDataService(IDbConnectionManager conManager)
        {
            _conManager = conManager;
        }

        // TODO: retry policy
        public async Task CreateTableIfNotExistAsync()
        {
            var tryCount = 3;
            Exception? lastException = null;

            while (tryCount > 0)
            {
                try
                {
                    await using (var connection = _conManager.GetConnection())
                    {
                        await connection.OpenAsync();

                        await using (var transaction = connection.BeginTransaction(IsolationLevel.Serializable))
                        {
                            try
                            {
                                await using (var command = new SqliteCommand(CreateTableQuery, connection, transaction))
                                {
                                    await command.ExecuteNonQueryAsync();
                                }

                                await transaction.CommitAsync();

                                break;
                            }
                            catch
                            {
                                await transaction.RollbackAsync();
                                throw;
                            }
                        }
                    }
                }
                catch (Exception? e)
                {
                    lastException = e;
                    tryCount--;
                }
            }

            if (lastException != null && tryCount == 0)
            {
                throw lastException;
            }
        }

        public async Task<List<CharacterDb>> GetAllCharsAsync()
        {
            var characters = new List<CharacterDb>();

            await using (var connection = _conManager.GetConnection())
            {
                await connection.OpenAsync();

                await using (var transaction = connection.BeginTransaction())
                {
                    await using (var command = new SqliteCommand(GetAllQuery, connection, transaction))
                    {
                        await using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (reader.HasRows)
                            {
                                while (await reader.ReadAsync())
                                {
                                    characters.Add(ReadCharacterDbRow(reader));
                                }
                            }

                            return characters;
                        }
                    }
                }
            }
        }

        public async Task CreateCharactersListAsync(List<CharacterDb> characters)
        {
            await using (var connection = _conManager.GetConnection())
            {
                await connection.OpenAsync();

                await using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        await using (var command = new SqliteCommand(FullInsertQuery, connection, transaction))
                        {
                            foreach (var character in characters)
                            {
                                command.Parameters.AddWithValue("@Id", character.Id);
                                command.Parameters.AddWithValue("@Name", character.Name);
                                command.Parameters.AddWithValue("@Status", character.Status);
                                command.Parameters.AddWithValue("@Species", character.Species);
                                command.Parameters.AddWithValue("@Type", character.Type);
                                command.Parameters.AddWithValue("@Gender", character.Gender);
                                command.Parameters.AddWithValue("@OriginName", character.OriginName);
                                command.Parameters.AddWithValue("@LocationName", character.LocationName);
                                command.Parameters.AddWithValue("@Image", character.Image);
                                command.Parameters.AddWithValue("@Episode", string.Join(',', character.Episode));
                                command.Parameters.AddWithValue("@Url", character.Url);
                                command.Parameters.AddWithValue("@Created", character.Created);

                                await command.ExecuteNonQueryAsync();
                                command.Parameters.Clear();
                            }
                        }

                        await transaction.CommitAsync();
                    }
                    catch
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                }
            }
        }

        public async Task<CharacterDb> GetCharByIdAsync(int id)
        {
            await using (var connection = _conManager.GetConnection())
            {
                await connection.OpenAsync();

                await using (var transaction = connection.BeginTransaction())
                {
                    await using (var command = new SqliteCommand(GetByIdQuery, connection, transaction))
                    {
                        command.Parameters.AddWithValue("@Id", id);

                        await using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return ReadCharacterDbRow(reader);
                            }

                            throw new Exception($"Character not found by Id = {id}.");
                        }
                    }
                }
            }
        }

        public async Task<int> CreateCharacterAsync(CreateCharacterDb character)
        {
            await using (var connection = _conManager.GetConnection())
            {
                await connection.OpenAsync();

                await using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        int insertedId;

                        await using (var command = new SqliteCommand(IncrementedInsertQuery, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@Name", character.Name);
                            command.Parameters.AddWithValue("@Status", character.Status);
                            command.Parameters.AddWithValue("@Species", character.Species);
                            command.Parameters.AddWithValue("@Type", character.Type);
                            command.Parameters.AddWithValue("@Gender", character.Gender);
                            command.Parameters.AddWithValue("@OriginName", character.OriginName);
                            command.Parameters.AddWithValue("@LocationName", character.LocationName);
                            command.Parameters.AddWithValue("@Image", character.Image);
                            command.Parameters.AddWithValue("@Episode", string.Join(',', character.Episode));
                            command.Parameters.AddWithValue("@Url", character.Url);
                            command.Parameters.AddWithValue("@Created", DateTime.Now);

                            var insertResult = await command.ExecuteScalarAsync()
                                               ?? throw new Exception(
                                                   "Something went wrong during database insert operation.");

                            insertedId = (int)(long)insertResult;
                        }

                        await transaction.CommitAsync();

                        return insertedId;
                    }
                    catch
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                }
            }
        }

        private static CharacterDb ReadCharacterDbRow(SqliteDataReader reader) =>
            new()
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Status = reader.GetString(2),
                Species = reader.GetString(3),
                Type = reader.GetString(4),
                Gender = reader.GetString(5),
                OriginName = reader.GetString(6),
                LocationName = reader.GetString(7),
                Image = reader.GetString(8),
                Episode = reader.GetString(9),
                Url = reader.GetString(10),
                Created = reader.GetDateTime(11)
            };
    }
}