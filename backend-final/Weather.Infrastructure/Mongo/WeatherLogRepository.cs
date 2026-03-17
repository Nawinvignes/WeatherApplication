using MongoDB.Driver;
using Microsoft.Extensions.Options;
using Weather.Domain.Entities;

namespace Weather.Infrastructure.Mongo;

public class WeatherLogRepository
{
    private readonly IMongoCollection<WeatherLog> _collection;

    public WeatherLogRepository(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        var database = client.GetDatabase(settings.Value.DatabaseName);
        _collection = database.GetCollection<WeatherLog>("WeatherLogs");
    }

    public async Task InsertLogAsync(WeatherLog log)
    {
        await _collection.InsertOneAsync(log);
    }

    // Get all logs for a specific user sorted by latest first
    public async Task<List<WeatherLog>> GetLogsByUserAsync(string userId)
    {
        return await _collection
            .Find(x => x.UserId == userId)
            .SortByDescending(x => x.RequestedAt)
            .ToListAsync();
    }

    // Get all logs sorted by latest first (admin use)
    public async Task<List<WeatherLog>> GetAllLogsAsync()
    {
        return await _collection
            .Find(_ => true)
            .SortByDescending(x => x.RequestedAt)
            .ToListAsync();
    }
}
