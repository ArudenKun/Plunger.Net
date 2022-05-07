using System.Text.Json.Serialization;

namespace Plunger.APIs.Nekobot.Responses;

public class NekobotResponse
{
    [JsonPropertyName("message")]
    public string? Message { get; set; }
}
