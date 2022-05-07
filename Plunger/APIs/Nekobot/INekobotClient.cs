using Plunger.APIs.Nekobot.Endpoints;
using Refit;

namespace Plunger.APIs.Nekobot;

public interface INekobotClient
{
    [Get("/image")]
    Task<string> GetImage([AliasAs("type")]NekobotTypes types);
}
