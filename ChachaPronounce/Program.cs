using ChachaPronounce.Common.Extensions;
using ChachaPronounce.Models;
using ChachaPronounce.Producer;
using ChachaPronounce.Services.RabbitMq;
using Microsoft.AspNetCore.HttpOverrides;

namespace ChachaPronounce
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var configuration = builder.Configuration;
            var env = builder.Environment.EnvironmentName;
            configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: true);
            builder.Services.Configure<AppSettings>(configuration);

            var appSettings = configuration.Get<AppSettings>();

            builder.Services.AddMinioService(new(appSettings.Minio.Endpoint, appSettings.Minio.AccessKey, appSettings.Minio.SecretKey));
            builder.Services.AddLineMessageService(appSettings.LineMessage.ChannelAccessToken);
            builder.Services.AddMemoryCache();
            builder.Services.AddSingleton<IRabbitMqService, RabbitMqService>();
            builder.Services.AddScoped<IMessageProducer, MessageProducer>();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}