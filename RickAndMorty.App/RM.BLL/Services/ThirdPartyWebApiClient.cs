using Microsoft.Extensions.Options;

using Newtonsoft.Json;

using RM.BLL.Interfaces;
using RM.BLL.Models.ThirdPartyApi;
using RM.Common;

using Serilog;

using System.Net;
using System.Net.Http.Headers;

namespace RM.BLL.Services
{
   public class ThirdPartyWebApiClient : IThirdPartyWebApiClient, IDisposable
    {
        private const int HttpTimeout = 600;

        private readonly string _apiUrl;

        private readonly ILogger _logger;

        private HttpClient httpClient;

        public ThirdPartyWebApiClient(ILogger logger, IOptions<SettingsModel> settings)
        {
            _apiUrl = settings.Value.ThirdPartyApiUrl;
            _logger = logger;

            Initialize();
        }

        public void Dispose()
        {
            httpClient.Dispose();
        }

        public async Task<ApiCharacterResponse> GetCharactersAsync()
        {
            var result = await httpClient.GetAsync($"{_apiUrl}/character/?page=1");
            CheckResponse(result);

            return JsonConvert.DeserializeObject<ApiCharacterResponse>(await result.Content.ReadAsStringAsync())!;
        }

        private void Initialize()
        {
            if (string.IsNullOrEmpty(_apiUrl))
            {
                var message = "Base address is null or empty.";

                _logger.Error(message);

                throw new Exception(message);
            }

            httpClient = new HttpClient { BaseAddress = new Uri(_apiUrl) };
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.Timeout = TimeSpan.FromSeconds(HttpTimeout);

            _logger.Information($"Http client initialized with base address: '{_apiUrl}'");
        }

        private void CheckResponse(HttpResponseMessage response, bool includeModelState = false)
        {
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return;
            }

            var error = JsonConvert.DeserializeAnonymousType(
                response.Content.ReadAsStringAsync().Result,
                new { Message = string.Empty, ModelState = new Dictionary<string, string[]>() });

            Exception exception;

            if (includeModelState && error?.ModelState != null && error.ModelState.Any())
            {
                exception = new Exception(string.Join("\r\n", error.ModelState.Select(x => string.Join("\r\n", x.Value))));
            }
            else
            {
                exception = new Exception(
                    $"Request to '{response.RequestMessage?.RequestUri}' returns code: '{response.StatusCode}'." + "\n"
                    + error?.Message);
            }

            exception.Data.Add("ResponseError", error?.Message);
            _logger.Error(exception.Message);

            throw exception;
        }
    }
}
