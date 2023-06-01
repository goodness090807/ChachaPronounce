using ChachaPronounce.Producer.Params;
using ChachaPronounce.Services.RabbitMq;
using System.Text;
using System.Text.Json;

namespace ChachaPronounce.Producer
{
    public class MessageProducer : IMessageProducer
    {
        private readonly IRabbitMqService _rabbitmqservice;
        private readonly string _queueName = "vocabulary-process";

        public MessageProducer(IRabbitMqService rabbitMqService)
        {
            _rabbitmqservice = rabbitMqService;
        }

        public void SendVocabularyProcessQueue(ProducerMessage producerMessage)
        {
            using var connection = _rabbitmqservice.CreateConnection();
            using var channel = connection.CreateModel();

            var message = JsonSerializer.Serialize(producerMessage);
            var byteMessage = Encoding.UTF8.GetBytes(message);

            channel.QueueDeclare(queue: _queueName,
                     durable: false,
                     exclusive: false,
                     autoDelete: false,
                     arguments: null);

            channel.BasicPublish(string.Empty, _queueName, mandatory: false, null, byteMessage);
        }
    }
}
