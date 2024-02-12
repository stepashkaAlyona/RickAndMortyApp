namespace RM.BLL.Interfaces
{
    using Models.ViewModel;

    public interface ICharService
    {
        Task<List<CharacterListVM>> GetAllCharsAsync();

        Task<CharacterVM> GetCharByIdAsync(int id);

        Task CreateCharacterAsync(CreateCharacterVM character);
    }
}