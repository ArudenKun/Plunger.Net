using System.Text.Json.Serialization;

namespace Plunger.APIs.WaifuPics.Entities;

public class WaifuImage
{
    [JsonPropertyName("url")]
    public string? ImageUrl { get; set; }
}
