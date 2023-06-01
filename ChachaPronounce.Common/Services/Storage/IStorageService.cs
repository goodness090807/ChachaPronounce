using ChachaPronounce.Common.Models.Enums;

namespace ChachaPronounce.Common.Services.Storage
{
    public interface IStorageService
    {
        Task<string> GetFileUrlAsync(PronounceType pronounceType, string vocabulary);
        Task<Stream?> GetFileStreamAsync(PronounceType pronounceType, string vocabulary);
        Task<bool> CheckFileExistAsync(PronounceType pronounceType, string vocabulary);
        Task<bool> UpdloadFileAsync(PronounceType pronounceType, string vocabulary, string fileName);
    }
}
