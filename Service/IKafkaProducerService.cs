using System.Threading.Tasks;

namespace Service
{
    public interface IKafkaProducerService
    {
        Task ProduceMessageAsync<T>(string topic, T message);
    }
}
