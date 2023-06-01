using ChachaPronounce.Common.Models.Enums;

namespace ChachaPronounce.Extensions
{
    public class MessageExtension
    {
        private static readonly Dictionary<string, PronounceType> _pronounce = new()
        {
            ["s"] = PronounceType.American,
            ["k"] = PronounceType.British
        };

        public static (string vocabulary, PronounceType? pronounceType) GetUrlParamByMessage(string message)
        {
            var trimMessage = message.Trim().ToLower();

            var splitMessage = trimMessage.Split(' ');

            if (splitMessage.Length > 1)
            {
                return (splitMessage[0], _pronounce.ContainsKey(splitMessage[1]) ? _pronounce[splitMessage[1]] : null);
            }

            return (splitMessage[0], PronounceType.American);
        }
    }
}
