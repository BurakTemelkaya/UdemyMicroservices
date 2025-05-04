using StackExchange.Redis;

namespace FreeCourse.Services.Basket.Services;

public class RedisService
{
    private readonly string _host;
    private readonly int _port;
    private ConnectionMultiplexer _connectionMultiplexer;

    public RedisService(string host, int port)
    {
        _host = host;
        _port = port;
    }

    public void Connect() => _connectionMultiplexer = ConnectionMultiplexer.Connect($"{_host}:{_port}");

    public IDatabase GetDb(int db = 1) => _connectionMultiplexer.GetDatabase(db);

    public async Task<Dictionary<string, string>> GetAllKeyValuesAsync(int db = 1)
    {
        var result = new Dictionary<string, string>();

        var database = _connectionMultiplexer.GetDatabase(db);

        var endpoints = _connectionMultiplexer.GetEndPoints();
        var server = _connectionMultiplexer.GetServer(endpoints.First());

        foreach (var key in server.Keys(database: db))
        {
            var value = await database.StringGetAsync(key);
            result.Add(key, value);
        }

        return result;
    }

}