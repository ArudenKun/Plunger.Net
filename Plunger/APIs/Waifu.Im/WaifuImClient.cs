using System.Text.Json;
using Plunger.APIs.Waifu.Im.Endpoints;
using Plunger.APIs.Waifu.Im.Parameters;
using Plunger.APIs.Waifu.Im.Responses;
using Refit;

namespace Plunger.APIs.Waifu.Im;

public class WaifuImClient : IWaifuImClient
{
    private const string BaseUrl = "https://api.waifu.im";
    private readonly IWaifuImClient client;

    public WaifuImClient()
    {
        client = RestService.For<IWaifuImClient>(BaseUrl, ApiSettings.Settings);
    }

    public async Task<string> GetRandomNsfwAsync(WaifuRandomParams<WaifuImNsfwTags> parameters)
    {
        return JsonSerializer.Deserialize<WaifuImResponse>(await client.GetRandomNsfwAsync(parameters))!.Images!.FirstOrDefault()!.Url! ??
            "No image found matching the criteria given";
    }

    public async Task<string> GetRandomSfwAsync(WaifuRandomParams<WaifuImVersatileTags> parameters)
    {
        if (parameters.SelectedTags != WaifuImVersatileTags.Waifu)
        {
            parameters.Gif = false;
        }
        return JsonSerializer.Deserialize<WaifuImResponse>(await client.GetRandomSfwAsync(parameters))!.Images!.FirstOrDefault()!.Url! ??
            "No image found matching the criteria given";
    }
}
