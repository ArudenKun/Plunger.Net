using System.Text.Json.Serialization;

namespace Plunger.APIs.Hastebin;

public class HastebinResponse
{
    [JsonPropertyName("key")]
    public string? Key { get; set; }
}
