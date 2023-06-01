using ChachaPronounce.Models;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace ChachaPronounce.Services.RabbitMq
{
    public class RabbitMqService : IRabbitMqService
    {
        private readonly AppSettings _appSettings;

        public RabbitMqService(IOptions<AppSettings> options)
        {
            _appSettings = options.Value;
        }

        public IConnection CreateConnection()
        {
            var connection = new ConnectionFactory()
            {
                HostName = _appSettings.RabbitMQ.HostName,
                UserName = _appSettings.RabbitMQ.Username,
                Password = _appSettings.RabbitMQ.Password
            };

            return connection.CreateConnection();
        }
    }
}
