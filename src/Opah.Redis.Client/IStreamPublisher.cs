namespace Opah.Redis.Client;

public interface IStreamPublisher
{
    Task ProducerAsync<T>(string topic, T message);
    Task<(string key, T message)> ConsumerAsync<T>(string topic, string lastId = "0");
}