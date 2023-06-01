using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using AudioProcessConsumer.Models;
using ChachaPronounce.Common.Extensions;
using Microsoft.Extensions.Configuration;
using AudioProcessConsumer.Services;

namespace AudioProcessConsumer
{
    public class Program
    {
        static void Main(string[] args)
        {
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    services.AddOptions();
                    services.Configure<AppSettings>(context.Configuration);
                    var appSettings = context.Configuration.Get<AppSettings>();
                    if (appSettings == null)
                    {
                        throw new NotImplementedException("環境變數未設定");
                    }

                    services.AddMemoryCache();
                    services.AddLineMessageService(appSettings.LineMessage.ChannelAccessToken);
                    services.AddMinioService(new(appSettings.Minio.Endpoint, appSettings.Minio.AccessKey, appSettings.Minio.SecretKey));
                    services.AddSingleton<IExternalService, ExternalService>();
                    services.AddHostedService<MessageConsumerService>();
                }).Build().Run();
        }
    }
}