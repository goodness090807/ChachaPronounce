namespace ChachaPronounce.Common.Models.Minio
{
    public class MinioOptions
    {
        public MinioOptions(string endpoint, string accessKey, string secretKey)
        {
            Endpoint = endpoint;
            AccessKey = accessKey;
            SecretKey = secretKey;
        }

        public string Endpoint { get; set; }

        public string AccessKey { get; set; }

        public string SecretKey { get; set; }
    }
}
