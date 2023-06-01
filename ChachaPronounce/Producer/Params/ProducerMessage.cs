using ChachaPronounce.Common.Models.Enums;

namespace ChachaPronounce.Producer.Params
{
    public class ProducerMessage
    {
        public ProducerMessage(string replyToken, string vocabulary, PronounceType pronounceType)
        {
            ReplyToken = replyToken;
            Vocabulary = vocabulary;
            PronounceType = pronounceType;
        }

        public string ReplyToken { get; set; }
        public string Vocabulary { get; set; }
        public PronounceType PronounceType { get; set; }
    }
}
