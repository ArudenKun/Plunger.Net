using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Plunger.Database.Models;

public class Lockdown : IIdentity
{
    [BsonId]
    public ObjectId ObjectId { get; set; }
    public ulong GuildId { get; set; }
    public ulong ChannelId { get; set; }
    public string? ChannelName { get; set; }
    public double Duration { get; set; }
    public string Reason { get; set; } = "None";
}
