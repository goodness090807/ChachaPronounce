using ChachaPronounce.Common.Models.LineMessage;
using System.Text.Json;
using System.Text;
using System.Net.Mime;

namespace ChachaPronounce.Common.Services.LineMessage
{
    public class LineMessageService : ILineMessageService
    {
        private readonly HttpClient _httpClient;

        public LineMessageService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("LineMessageAPI");
        }

        public async Task<bool> SendMessage(string replyToken, List<object> messages)
        {
            var replyJson = JsonSerializer.Serialize(new
                 ReplyContent(replyToken, messages)
               , new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            var stringContent = new StringContent(replyJson, Encoding.UTF8, MediaTypeNames.Application.Json);

            try
            {
                using var response = await _httpClient.PostAsync(LineUrl.Reply, stringContent);

                response.EnsureSuccessStatusCode();
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}
