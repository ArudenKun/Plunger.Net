using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Plunger.Database.Models;

public class SuggestionModel : IIdentity
{
    [BsonId]
    public ObjectId ObjectId { get; set; }
    public ulong? GuildId { get; set; }
    public ulong? MessageId { get; set; }
    public List<Details>? SuggestionDetails { get; set; }

    public class Details
    {
        public ulong? MemberId { get; set; }
        public string? Type { get; set; }
        public string? Suggestion { get; set; }
    }
}