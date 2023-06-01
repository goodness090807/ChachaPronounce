using ChachaPronounce.Producer.Params;

namespace ChachaPronounce.Producer
{
    public interface IMessageProducer
    {
        void SendVocabularyProcessQueue(ProducerMessage producerMessage);
    }
}
