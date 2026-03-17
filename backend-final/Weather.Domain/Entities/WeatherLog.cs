using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Weather.Domain.Entities;

[BsonIgnoreExtraElements]
public class WeatherLog
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = "";

    public string? UserId { get; set; }
    public string City { get; set; } = "";
    public DateTime RequestedAt { get; set; }
    public string? ErrorMessage { get; set; }
}
