using MongoDB.Bson;

namespace Plunger.Database.Models;

public class ChatFilter : IIdentity
{
    public ObjectId ObjectId { get; set; }
    public ulong GuildId { get; set; }
    public ulong LoggingChannelId { get; set; }
    public List<string>? Words { get; set; }
}