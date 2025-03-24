namespace Opah.Redis.Client;

public class RedisClientOptions
{
    public const string Section = "RedisClient";
    
    public string Host { get; set; }
    public int Port { get; set; }
}