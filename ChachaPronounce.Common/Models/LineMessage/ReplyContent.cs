namespace ChachaPronounce.Common.Models.LineMessage
{
    public class ReplyContent
    {
        public ReplyContent(string replyToken, List<object> massages)
        {
            ReplyToken = replyToken;
            Messages = massages;
        }

        public string ReplyToken { get; set; }
        public List<object> Messages { get; set; }

        public class AudioMessage : Message
        {
            public AudioMessage(string originalContentUrl, int duration = 1000) : base("audio")
            {
                OriginalContentUrl = originalContentUrl;
                Duration = duration;
            }

            public string OriginalContentUrl { get; set; }
            public int Duration { get; set; }
        }

        public class TextMessage : Message
        {
            public TextMessage(string text) : base("text")
            {
                Text = text;
            }

            public string Text { get; set; }
        }

        public class Message
        {
            public Message(string type)
            {
                Type = type;
            }

            public string Type { get; set; }
        }
    }
}
