using ChachaPronounce.Common.Models.Enums;

namespace AudioProcessConsumer.Models
{
    public class ProducerMessage
    {
        public string ReplyToken { get; set; } = string.Empty;
        public string Vocabulary { get; set; } = string.Empty;
        public PronounceType PronounceType { get; set; } = PronounceType.American;
    }
}
