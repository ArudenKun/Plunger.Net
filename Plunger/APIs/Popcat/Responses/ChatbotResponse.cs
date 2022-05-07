using System.Text.Json.Serialization;

namespace Plunger.APIs.Popcat.Responses;

public class ChatbotResponse
{
    [JsonPropertyName("response")]
    public string? Response { get; set; }
}
