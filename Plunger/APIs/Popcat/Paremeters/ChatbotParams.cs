using Refit;

namespace Plunger.APIs.Popcat.Paremeters;

public class ChatbotParams
{
    [AliasAs("msg")]
    public string? Message { get; set; }

    [AliasAs("owner")]
    public string Owner { get; set; } = "Aruden";

    [AliasAs("botname")]
    public string BotName { get; set; } = "Plunger";
}
