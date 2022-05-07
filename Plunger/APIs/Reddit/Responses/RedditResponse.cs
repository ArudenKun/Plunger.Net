using System.Text.Json.Serialization;

namespace Plunger.APIs.Reddit.Responses;

public class RedditResponse
{
    [JsonPropertyName("url_overridden_by_dest")]
    public string? ImageOrGifUrl { get; set; }
}
