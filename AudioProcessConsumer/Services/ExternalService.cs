using AudioProcessConsumer.Models;
using ChachaPronounce.Common.Models.Enums;
using Microsoft.Extensions.Options;

namespace AudioProcessConsumer.Services
{
    public class ExternalService : IExternalService
    {
        private readonly HttpClient _httpClient;
        private readonly AppSettings _appSettings;

        public ExternalService(IHttpClientFactory httpClientFactory, IOptions<AppSettings> options)
        {
            _httpClient = httpClientFactory.CreateClient();
            _appSettings = options.Value;
        }

        public async Task<bool> CheckVocabularyExist(string vocabulary)
        {
            var url = GetVocabularyQueryUrl(vocabulary);
            try
            {
                using var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException)
            {
                return false;
            }

            return true;
        }

        public async Task<MemoryStream?> GetAudioStreamAsync(string vocabulary, PronounceType pronounceType)
        {
            try
            {
                var url = _appSettings.GetAudioFetchUrl(vocabulary, pronounceType);
                var stream = await _httpClient.GetStreamAsync(url);
                var ms = new MemoryStream();
                stream.CopyTo(ms);

                return ms;
            }
            catch(Exception)
            {
                return null;
            }            
        }

        private string GetVocabularyQueryUrl(string vocabulary)
        {
            return $"https://api.dictionaryapi.dev/api/v2/entries/en/{vocabulary}";
        }
    }
}
