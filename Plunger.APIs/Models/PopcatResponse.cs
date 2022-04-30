using Newtonsoft.Json;

namespace Plunger.APIs.Models;

public class PopcatResponse
{
    [JsonProperty("binary")] public string? Binary { get; set; }
    [JsonProperty("text")] public string? Text { get; set; }
}
