using Newtonsoft.Json;

namespace Plunger.APIs.Models;

public class WaifuPicsImage
{
    [JsonProperty("url")] public string? ImageUrl { get; set; }
}
