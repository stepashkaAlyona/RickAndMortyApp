using AutoMapper;

using Microsoft.Extensions.Logging;

using RM.BLL.Interfaces;
using RM.BLL.Models.ViewModel;
using RM.DAL.CharactersOperations;
using RM.DAL.Models;

namespace RM.BLL.Services
{
    public class CharService : ICharService
    {
        private readonly ICharDataService _charDataService;

        private readonly IThirdPartyWebApiClient _apiClient;

        private readonly ILogger<CharService> _logger;

        private readonly IMapper _mapper;

        public CharService(ICharDataService charDataService, IThirdPartyWebApiClient apiClient, IMapper mapper, ILogger<CharService> logger)
        {
            _charDataService = charDataService;
            _apiClient = apiClient;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<CharacterListVM>> GetAllCharsAsync()
        {
            _logger.LogInformation("1. Get All Characters.");

            var characters = await _charDataService.GetAllCharsAsync();

            if (characters.Count == 0)
            {
                _logger.LogInformation("2.1. No one character found. Start getting new characters from third party API.");

                var apiResponse = await _apiClient.GetCharactersAsync();

                characters = _mapper.Map<List<CharacterDb>>(apiResponse.Results);

                _logger.LogInformation($"2.2. Found {characters.Count} characters. Saving entities to the database.");

                await _charDataService.CreateCharactersListAsync(characters);
            }

            return _mapper.Map<List<CharacterListVM>>(characters);
        }

        public async Task<CharacterVM> GetCharByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("1. Get Character by Id.");

                var character = await _charDataService.GetCharByIdAsync(id);

                _logger.LogInformation("2. Character was found.");

                return _mapper.Map<CharacterVM>(character);
            }
            catch
            {
                _logger.LogInformation("2. Character not found or several entities found.");
                throw;
            }
        }

        public async Task CreateCharacterAsync(CreateCharacterVM character)
        {
            _logger.LogInformation("1. Saving character to the database.");

            var insertedId = await _charDataService.CreateCharacterAsync(_mapper.Map<CreateCharacterDb>(character));

            _logger.LogInformation($"2. New character was saved with Id = {insertedId}.");
        }
    }
}
