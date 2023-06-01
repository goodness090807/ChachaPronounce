using ChachaPronounce.Common.Models.Enums;
using Minio;

namespace ChachaPronounce.Common.Services.Storage
{
    public class MinioService : IStorageService
    {
        private readonly IMinioClient _minioClient;
        private readonly string _bucketName = "chacha-pronounce";
        private readonly string _objectPrefix = "audio";
        private readonly Dictionary<PronounceType, string> _accent = new()
        {
            [PronounceType.American] = "us",
            [PronounceType.British] = "uk"
        };

        public MinioService(IMinioClient minioClient)
        {
            _minioClient = minioClient;
        }

        public async Task<string> GetFileUrlAsync(PronounceType pronounceType, string vocabulary)
        {
            var filePath = GetFilePath(pronounceType, vocabulary);
            var reqParams = new Dictionary<string, string> { { "response-content-type", "audio/x-m4a" } };

            if (!await CheckFileExistAsync(filePath))
            {
                return "";
            }

            var args = new PresignedGetObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(filePath)
                .WithExpiry(1000)
                .WithHeaders(reqParams);

            return await _minioClient.PresignedGetObjectAsync(args).ConfigureAwait(false);
        }

        public async Task<Stream?> GetFileStreamAsync(PronounceType pronounceType, string vocabulary)
        {
            var filePath = GetFilePath(pronounceType, vocabulary);

            if (!await CheckFileExistAsync(filePath))
            {
                return null;
            }

            var memoryStream = new MemoryStream();

            var args = new GetObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(filePath)
                .WithCallbackStream((stream) =>
                {
                    stream.CopyTo(memoryStream);
                });

            await _minioClient.GetObjectAsync(args);
            memoryStream.Seek(0, SeekOrigin.Begin);

            return memoryStream;
        }

        public async Task<bool> CheckFileExistAsync(PronounceType pronounceType, string vocabulary)
        {
            return await CheckFileExistAsync(GetFilePath(pronounceType, vocabulary));
        }

        public async Task<bool> UpdloadFileAsync(PronounceType pronounceType, string vocabulary, string fileName)
        {
            try
            {
                using (FileStream fileStream = File.OpenRead(fileName))
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        var putObjectArgs = new PutObjectArgs()
                        .WithBucket(_bucketName)
                        .WithObject(GetFilePath(pronounceType, vocabulary))
                        .WithStreamData(fileStream)
                        .WithObjectSize(fileStream.Length)
                        .WithContentType("audio/x-m4a");

                        await _minioClient.PutObjectAsync(putObjectArgs).ConfigureAwait(false);
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
            
            return true;
        }

        private string GetFilePath(PronounceType pronounceType, string vocabulary)
        {
            return $"{_objectPrefix}/{_accent[pronounceType]}/{vocabulary}.m4a";
        }

        private async Task<bool> CheckFileExistAsync(string filePath)
        {
            try
            {
                StatObjectArgs statObjectArgs = new StatObjectArgs()
                                       .WithBucket(_bucketName)
                                       .WithObject(filePath);

                await _minioClient.StatObjectAsync(statObjectArgs).ConfigureAwait(false);
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}
