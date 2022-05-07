using System.Text.Json.Serialization;

namespace Plunger.APIs.Popcat.Responses;

public class BinaryResponse
{
    [JsonPropertyName("text")]
    public string? Text { get; set; }
    
    [JsonPropertyName("binary")]
    public string? Binary { get; set; }
}
