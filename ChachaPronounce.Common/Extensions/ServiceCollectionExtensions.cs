using ChachaPronounce.Common.Models.Minio;
using ChachaPronounce.Common.Services.LineMessage;
using ChachaPronounce.Common.Services.Storage;
using Microsoft.Extensions.DependencyInjection;
using Minio;

namespace ChachaPronounce.Common.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 註冊LineMessage服務
        /// </summary>
        public static IServiceCollection AddLineMessageService(this IServiceCollection services, string channelAccessToken)
        {
            services.AddHttpClient("LineMessageAPI", httpClient =>
            {
                httpClient.BaseAddress = new Uri("https://api.line.me/");
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {channelAccessToken}");
            });
            services.AddSingleton<ILineMessageService, LineMessageService>();

            return services;
        }

        /// <summary>
        /// 註冊Minio服務
        /// </summary>
        public static IServiceCollection AddMinioService(this IServiceCollection services, MinioOptions options)
        {
            services.AddSingleton<IMinioClient>(provider =>
            {
                return new MinioClient()
                    .WithEndpoint(options.Endpoint)
                    .WithCredentials(options.AccessKey, options.SecretKey)
                    .Build();
            });
            services.AddSingleton<IStorageService, MinioService>();

            return services;
        }
    }
}
