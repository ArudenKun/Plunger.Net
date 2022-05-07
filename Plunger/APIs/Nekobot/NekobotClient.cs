using System.Text.Json;
using Plunger.APIs.Nekobot.Endpoints;
using Plunger.APIs.Nekobot.Responses;
using Refit;

namespace Plunger.APIs.Nekobot;

public class NekobotClient : INekobotClient
{
    private const string BaseUrl = "https://nekobot.xyz/api";
    private readonly INekobotClient client;

    public NekobotClient()
    {
        client = RestService.For<INekobotClient>(BaseUrl, ApiSettings.Settings);
    }

    public async Task<string> GetImage([AliasAs("type")] NekobotTypes types)
    {
        return JsonSerializer.Deserialize<NekobotResponse>(await client.GetImage(types))!.Message!;
    }
}