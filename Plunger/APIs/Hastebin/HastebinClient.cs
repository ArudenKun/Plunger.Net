using System.Text.Json;
using Refit;

namespace Plunger.APIs.Hastebin;

public class HastebinClient : IHastebinClient
{
    private const string BaseUrl = "https://www.toptal.com/developers/hastebin";
    private readonly IHastebinClient client;

    public HastebinClient()
    {
        client = RestService.For<IHastebinClient>(BaseUrl, ApiSettings.Settings);
    }

    public async Task<string> UploadAsync([Body] string code)
    {
        var key = JsonSerializer.Deserialize<HastebinResponse>(await client.UploadAsync(code))!.Key;
        return $"{BaseUrl}/{key}";
    }
}