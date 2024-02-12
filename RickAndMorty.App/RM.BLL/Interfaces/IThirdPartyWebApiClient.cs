using RM.BLL.Models.ThirdPartyApi;

namespace RM.BLL.Interfaces
{
    public interface IThirdPartyWebApiClient
    {
        Task<ApiCharacterResponse> GetCharactersAsync();
    }
}