using AudioProcessConsumer.Models;
using AudioProcessConsumer.Services;
using ChachaPronounce.Common.Models.LineMessage;
using ChachaPronounce.Common.Services.LineMessage;
using ChachaPronounce.Common.Services.Storage;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace AudioProcessConsumer
{
    public class MessageConsumerService : IHostedService
    {
        private readonly Task _completedTask = Task.CompletedTask;
        private readonly CancellationTokenSource _cancellationTokenSource;

        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly string _queueName = "vocabulary-process";

        private readonly ILogger<MessageConsumerService> _logger;
        private readonly AppSettings _appSettings;
        private readonly IMemoryCache _memorycache;

        private readonly IExternalService _externalservice;
        private readonly ILineMessageService _linemessageservice;
        private readonly IStorageService _storageservice;

        public MessageConsumerService(ILogger<MessageConsumerService> logger, IOptions<AppSettings> options, IMemoryCache memoryCache
            , IExternalService externalService, ILineMessageService lineMessageService, IStorageService storageService)
        {
            _logger = logger;
            _memorycache = memoryCache;
            _appSettings = options.Value;

            _externalservice = externalService;
            _linemessageservice = lineMessageService;
            _storageservice = storageService;
            var factory = new ConnectionFactory()
            {
                HostName = _appSettings.RabbitMQ.HostName,
                UserName = _appSettings.RabbitMQ.UserName,
                Password = _appSettings.RabbitMQ.Password,
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: _queueName,
                              durable: false,
                              exclusive: false,
                              autoDelete: false,
                              arguments: null);

            _cancellationTokenSource = new CancellationTokenSource();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("服務已啟動，開始時間：{time}", DateTimeOffset.UtcNow.AddHours(8));

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += ProcessMessage;

            _channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);

            return _completedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _channel.Close();
            _connection.Close();
            _cancellationTokenSource.Cancel();
            _logger.LogInformation("服務已停止，停止時間：{time}", DateTimeOffset.UtcNow.AddHours(8));

            return _completedTask;
        }

        private async void ProcessMessage(object? model, BasicDeliverEventArgs args)
        {
            var body = args.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            var producerMessage = JsonConvert.DeserializeObject<ProducerMessage>(message) ?? new();

            if (_memorycache.TryGetValue($"{producerMessage.Vocabulary}.{producerMessage.PronounceType}", out bool vocabularyExist))
            {
                if (vocabularyExist)
                {
                    var url = _appSettings.GetAudioUrl(producerMessage.PronounceType, producerMessage.Vocabulary);
                    await _linemessageservice.SendMessage(producerMessage.ReplyToken, new() { new ReplyContent.AudioMessage(url) });
                }
                else
                {
                    await _linemessageservice.SendMessage(producerMessage.ReplyToken, new() { new ReplyContent.TextMessage("找不到單字") });
                }

                return;
            }

            if (!await _externalservice.CheckVocabularyExist(producerMessage.Vocabulary))
            {
                _memorycache.Set($"{producerMessage.Vocabulary}.{producerMessage.PronounceType}", false, TimeSpan.FromMinutes(10));
                await _linemessageservice.SendMessage(producerMessage.ReplyToken, new() { new ReplyContent.TextMessage("找不到單字") });
                return;
            }

            try
            {
                var vocabularyStream = await _externalservice.GetAudioStreamAsync(producerMessage.Vocabulary, producerMessage.PronounceType);
                if (vocabularyStream == null)
                {
                    await _linemessageservice.SendMessage(producerMessage.ReplyToken, new() { new ReplyContent.TextMessage("外部服務有問題，請稍後再試") });
                    return;
                }

                var fileName = vocabularyStream.SaveFileByStream(producerMessage.Vocabulary, "mp3");
                var convertedFileName = FileService.ConvertAudio(fileName, "m4a");

                if (!await _storageservice.UpdloadFileAsync(producerMessage.PronounceType, producerMessage.Vocabulary, convertedFileName))
                {
                    throw new Exception();
                }

                var url = _appSettings.GetAudioUrl(producerMessage.PronounceType, producerMessage.Vocabulary);
                await _linemessageservice.SendMessage(producerMessage.ReplyToken, new() { new ReplyContent.AudioMessage(url) });

                File.Delete(fileName);
                File.Delete(convertedFileName);
                _memorycache.Set($"{producerMessage.Vocabulary}.{producerMessage.PronounceType}", true, TimeSpan.FromMinutes(10));
            }
            catch (Exception)
            {
                await _linemessageservice.SendMessage(producerMessage.ReplyToken, new() { new ReplyContent.TextMessage("發生錯誤") });
                return;
            }
        }
    }
}
