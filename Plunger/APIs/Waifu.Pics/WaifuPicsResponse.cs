using System.Text.Json.Serialization;

namespace Plunger.APIs.Waifu.Pics;

public class WaifuPicsResponse
{
    [JsonPropertyName("url")] public string? ImageUrl { get; set; }
}
