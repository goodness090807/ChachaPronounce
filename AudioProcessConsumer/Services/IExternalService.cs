using ChachaPronounce.Common.Models.Enums;

namespace AudioProcessConsumer.Services
{
    public interface IExternalService
    {
        Task<bool> CheckVocabularyExist(string vocabulary);
        Task<MemoryStream?> GetAudioStreamAsync(string vocabulary, PronounceType pronounceType);
    }
}
