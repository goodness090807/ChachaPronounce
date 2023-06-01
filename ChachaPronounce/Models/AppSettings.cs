using ChachaPronounce.Common.Models.Enums;

namespace ChachaPronounce.Models
{
    public class AppSettings
    {
        public LineMessageDto LineMessage { get; set; } = new ();
        public MinioDto Minio { get; set; } = new ();
        public RabbitMQDto RabbitMQ { get; set; } = new ();
        public string BaseAudioUrl { get; set; } = string.Empty;

        public string GetAudioUrl(PronounceType pronounceType, string vocabulary) => $"{BaseAudioUrl}/{(int)pronounceType}/{vocabulary}";

        public class LineMessageDto
        {
            public string BaseUrl { get; set; } = string.Empty;
            public string ChannelAccessToken { get; set; } = string.Empty;
        }

        public class MinioDto
        {
            public string Endpoint { get; set; } = string.Empty;
            public string AccessKey { get; set; } = string.Empty;
            public string SecretKey { get; set; } = string.Empty;
        }

        public class RabbitMQDto
        {
            public string HostName { get; set; } = string.Empty;
            public string Username { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }
    }
}
