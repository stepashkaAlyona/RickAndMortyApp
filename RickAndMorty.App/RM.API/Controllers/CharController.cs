using Microsoft.AspNetCore.Mvc;

using RM.API.Models.HttpModels;
using RM.BLL.Interfaces;
using RM.BLL.Models.ViewModel;

namespace RM.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CharController : ControllerBase
    {
        private readonly ICharService _charService;

        private readonly ILogger<CharController> _logger;

        public CharController(ICharService charService, ILogger<CharController> logger)
        {
            _charService = charService;
            _logger = logger;
        }

        [HttpGet("{id}")]
        [ResponseCache(Duration = 600)]
        public async Task<ResponseResult> GetChar(int id)
        {
            try
            {
                var foundedChar = await _charService.GetCharByIdAsync(id);
                return ResponseResultCreator.Success(foundedChar);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return ResponseResultCreator.Error(e.Message);
            }
        }

        [HttpGet]
        [ResponseCache(Duration = 5)]
        public async Task<ResponseResult> GetChars()
        {
            try
            {
                var chars = await _charService.GetAllCharsAsync();
                return ResponseResultCreator.Success(chars);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return ResponseResultCreator.Error(e.Message);
            }
        }

        [HttpPost]
        public async Task<ResponseResult> AddChar(CreateCharacterVM character)
        {
            try
            {
                await _charService.CreateCharacterAsync(character);
                return ResponseResultCreator.Success();
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return ResponseResultCreator.Error(e.Message);
            }
        }
    }
}