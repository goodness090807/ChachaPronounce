namespace ChachaPronounce.Common.Services.LineMessage
{
    public interface ILineMessageService
    {
        Task<bool> SendMessage(string replyToken, List<object> messages);
    }
}
