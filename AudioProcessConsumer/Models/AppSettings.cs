using ChachaPronounce.Common.Models.Enums;

namespace AudioProcessConsumer.Models
{
    public class AppSettings
    {
        public RabbitMQDto RabbitMQ { get; set; } = new();
        public LineMessageDto LineMessage { get; set; } = new();
        public MinioDto Minio { get; set; } = new();
        public string BaseAudioUrl { get; set; } = string.Empty;
        public string ExternalFetchUrl { get; set; } = string.Empty;

        public string GetAudioUrl(PronounceType pronounceType, string vocabulary) => $"{BaseAudioUrl}/{(int)pronounceType}/{vocabulary}";
        public string GetAudioFetchUrl(string vocabulary, PronounceType pronounceType) => $"{ExternalFetchUrl}?audio={vocabulary}&type={(int)pronounceType}";

        public class RabbitMQDto
        {
            public string HostName { get; set; } = string.Empty;
            public string UserName { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }

        public class LineMessageDto
        {
            public string ChannelAccessToken { get; set; } = string.Empty;
        }

        public class MinioDto
        {
            public string Endpoint { get; set; } = string.Empty;
            public string AccessKey { get; set; } = string.Empty;
            public string SecretKey { get; set; } = string.Empty;
        }
    }
}
