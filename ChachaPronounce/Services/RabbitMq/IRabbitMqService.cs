using RabbitMQ.Client;

namespace ChachaPronounce.Services.RabbitMq
{
    public interface IRabbitMqService
    {
        IConnection CreateConnection();
    }
}
