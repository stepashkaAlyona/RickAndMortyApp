using RM.DAL.Models;

namespace RM.DAL.CharactersOperations
{
    public interface ICharDataService
    {
        Task CreateTableIfNotExistAsync();

        Task<List<CharacterDb>> GetAllCharsAsync();

        Task CreateCharactersListAsync(List<CharacterDb> characters);

        Task<CharacterDb> GetCharByIdAsync(int id);

        Task<int> CreateCharacterAsync(CreateCharacterDb character);
    }
}