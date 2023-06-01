namespace ChachaPronounce.Params
{
    public class WebhookEventParam
    {
        public string Destination { get; set; } = string.Empty;
        public List<Event> Events { get; set; } = new List<Event>();

        public class Event
        {
            public string? Type { get; set; }
            public TextMessageContent Message { get; set; } = new TextMessageContent();
            public long Timestamp { get; set; }
            public Source? Source { get; set; }
            public string ReplyToken { get; set; } = string.Empty;
            public string? Mode { get; set; }
            public string? WebhookEventId { get; set; }
            public DeliveryContext? DeliveryContext { get; set; }
        }
        public class TextMessageContent
        {
            public string Type { get; set; } = string.Empty;
            public string Id { get; set; } = string.Empty;
            public string Text { get; set; } = string.Empty;
        }

        public class Source
        {
            public string? Type { get; set; }
            public string? UserId { get; set; }
        }

        public class DeliveryContext
        {
            public bool IsRedelivery { get; set; }
        }
    }
}
