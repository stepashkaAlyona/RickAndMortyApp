using System.Net;
using System.Net.Http.Json;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Logging;

using Moq;

using Newtonsoft.Json;

using RM.API;
using RM.API.Controllers;
using RM.API.Models.HttpModels;
using RM.BLL.Interfaces;
using RM.BLL.Models.ViewModel;

namespace RM.Tests
{
    public class CharControllerTest
    {
        private const string ValidUrlString =
            "https://cdn.vox-cdn.com/rickroll_4k.jpg";
        
        private const string InvalidUrlString =
            "htt1://cdn.vox-cdn.com/thumbor/WR9hE8wvdM4hfHysXitls9_bCZI=/0x0:1192x795/1400x1400/filters:"
            + "focal(596x398:597x399)/cdn.vox-cdn.com/uploads/chorus_asset/file/22312759/rickroll_4k.jpg"
            + "cdn.vox-cdn.com/thumbor/WR9hE8wvdM4hfHysXitls9_bCZI=/0x0:1192x795/1400x1400/filters:focal(596x398:597x399)/cdn.vox-cdn.com/uploads/chorus_asset/file/22312759/rickroll_4k.jpg"
            + "cdn.vox-cdn.com/thumbor/WR9hE8wvdM4hfHysXitls9_bCZI=/0x0:1192x795/1400x1400/filters:focal(596x398:597x399)/cdn.vox-cdn.com/uploads/chorus_asset/file/22312759/rickroll_4k.jpg";

        private const string StringValue = "Alyona";

        private const string ApiCharUrl = "/api/Char";

        private readonly CharController _controller;

        private readonly Mock<ICharService> _charService;

        public CharControllerTest()
        {
            Mock<ILogger<CharController>> logger = new();

            _charService = new Mock<ICharService>();
            _controller = new CharController(_charService.Object, logger.Object);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(50)]
        public async void GetChars_ValidCharList_SuccessResponse(int charsCount)
        {
            //arrange
            var charList = GetCharListVmList(charsCount);
            _charService.Setup(x => x.GetAllCharsAsync()).Returns(Task.FromResult(charList));

            //act
            var response = (await _controller.GetChars()) as ResponseResult<List<CharacterListVM>>;
            var resultData = response?.Data;

            //assert
            Assert.NotNull(resultData);
            Assert.Equal(charList.Count(), resultData.Count());
            Assert.True(response is { IsSuccess: true, ErrorMessage: null });
        }

        [Fact]
        public async void GetChars_RequestValidCharList_SuccessResponse()
        {
            //arrange
            await using var application = new WebApplicationFactory<Program>();
            var client = application.CreateClient();

            //act
            var response = await client.GetAsync(ApiCharUrl);
            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ResponseResult<List<CharacterListVM>>>(responseContent);

            //assert
            Assert.NotNull(result);
            Assert.NotEmpty(result.Data);
            Assert.True(result is { IsSuccess: true, ErrorMessage: null });
        }

        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(50)]
        public async void GetChar_PassExistedId_SuccessResponse(int charsCount)
        {
            //arrange
            var charId = charsCount - 1;
            var foundedChar = GetCharVm(charId);
            _charService.Setup(x => x.GetCharByIdAsync(charId)).Returns(Task.FromResult(foundedChar));

            //act
            var response = (await _controller.GetChar(charId)) as ResponseResult<CharacterVM>;
            var resultData = response?.Data;

            //assert
            Assert.NotNull(resultData);
            Assert.Equal(charId, resultData.Id);
            Assert.True(response is { IsSuccess: true, ErrorMessage: null });
        }

        [Fact]
        public async void AddChar_PassValidModel_SuccessResponse()
        {
            //arrange
            var validCreateChar = GetValidCreateCharVm();
            _charService.Setup(x => x.CreateCharacterAsync(validCreateChar)).Returns(Task.CompletedTask);

            //act
            var response = await _controller.AddChar(validCreateChar);

            //assert
            Assert.NotNull(response);
            Assert.True(response is { IsSuccess: true, ErrorMessage: null });
        }

        [Fact]
        public async void AddChar_RequestPassValidModel_SuccessResponse()
        {
            //arrange
            await using var application = new WebApplicationFactory<Program>();
            var client = application.CreateClient();

            var validCreateChar = GetValidCreateCharVm();

            // Act 
            var response = await client.PostAsync(ApiCharUrl, JsonContent.Create(validCreateChar));
            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ResponseResult>(responseContent);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(result);
            Assert.True(result is { IsSuccess: true, ErrorMessage: null });
        }

        [Fact]
        public async void AddChar_RequestPassInvalidModel_ErrorResponse()
        {
            // Arrange
            await using var application = new WebApplicationFactory<Program>();
            var client = application.CreateClient();

            var invalidCreateChar = GetInvalidCreateCharVm();

            // Act 
            var response = await client.PostAsync(ApiCharUrl, JsonContent.Create(invalidCreateChar));

            // Assert
            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        private List<CharacterListVM> GetCharListVmList(int count)
        {
            var chars = new List<CharacterListVM>();

            for (var i = 1; i <= count; i++)
            {
                chars.Add(new CharacterListVM { Id = i });
            }

            return chars;
        }

        private CharacterVM GetCharVm(int id)
        {
            return new CharacterVM { Id = id };
        }

        private CreateCharacterVM GetValidCreateCharVm()
        {
            return new CreateCharacterVM
                       {
                           Name = StringValue, 
                           Image = ValidUrlString, 
                           Url = ValidUrlString,
                           Episode = new List<string>(),
                           Gender = StringValue,
                           LocationName = StringValue,
                           OriginName = StringValue,
                           Species = StringValue,
                           Status = StringValue,
                           Type = StringValue
            };
        }

        private CreateCharacterVM GetInvalidCreateCharVm()
        {
            return new CreateCharacterVM { Image = InvalidUrlString, Url = InvalidUrlString };
        }
    }
}