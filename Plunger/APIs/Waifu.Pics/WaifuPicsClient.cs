using System.Text.Json;
using Plunger.APIs.Waifu.Pics.Enums;
using Refit;

namespace Plunger.APIs.Waifu.Pics;

public class WaifuPicsClient : IWaifuPicsClient
{
    private const string BaseUrl = "https://api.waifu.pics";
    private readonly IWaifuPicsClient client;

    public WaifuPicsClient()
    {
        client = RestService.For<IWaifuPicsClient>(BaseUrl, ApiSettings.Settings);
    }

    public async Task<string> GetSfwImageAsync(SfwCategory category)
    {
        return JsonSerializer.Deserialize<WaifuPicsResponse>(await client.GetSfwImageAsync(category))!.ImageUrl!;
    }
    
    public async Task<string> GetNsfwImageAsync(NsfwCategory category)
    {
        return JsonSerializer.Deserialize<WaifuPicsResponse>(await client.GetNsfwImageAsync(category))!.ImageUrl!;
    }
}
