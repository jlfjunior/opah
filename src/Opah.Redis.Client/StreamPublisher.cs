using System.Text.Json;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Opah.Redis.Client;

public class StreamPublisher : IStreamPublisher
{
    readonly IDatabase _database;

    public StreamPublisher(IOptions<RedisClientOptions> options)
    {
        var redisClientOptions = options.Value;
        var connection = ConnectionMultiplexer.Connect($"{redisClientOptions.Host}:{redisClientOptions.Port}");
        _database = connection.GetDatabase();
    }
    
    public async Task ProducerAsync<T>(string topic, T message)
    {
        var entries = typeof(T).GetProperties()
            .Select(prop => new NameValueEntry(prop.Name, prop.GetValue(message)?.ToString() ?? ""))
            .ToArray();
        
        await _database.StreamAddAsync(topic, entries);
    }

    public async Task<(string key, T message)> ConsumerAsync<T>(string topic, string lastId = "0")
    {
        var streams = await _database.StreamReadAsync(topic, lastId, 1);

        if (streams.Length > 0)
        {
            var entry = streams[0];
            lastId = entry.Id;
            var data = entry.Values.ToDictionary(x => (string)x.Name, x => (string)x.Value!);
            try
            {
                var json = JsonSerializer.Serialize(data);
                var obj = JsonSerializer.Deserialize<T>(json);


                return (lastId, obj);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        
        return default;
    }
}