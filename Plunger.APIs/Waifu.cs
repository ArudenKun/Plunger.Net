using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Plunger.APIs.Interfaces;
using Plunger.APIs.Models;
using Plunger.APIs.Models.Enums;
using Refit;

namespace Plunger.APIs;

public class Waifu : IWaifu
{
    private static readonly RefitSettings Settings = new()
    {
        ContentSerializer = new NewtonsoftJsonContentSerializer(new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            }
        })
    };

    private readonly IWaifu _client;

    public Waifu()
    {
        _client = RestService.For<IWaifu>("https://api.waifu.pics", Settings);
    }

    public async Task<WaifuPicsImage> GetSfwImageAsync(SfwCategory category = SfwCategory.Waifu)
    {
        return await _client.GetSfwImageAsync(category);
    }

    public async Task<WaifuPicsImage> GetNsfwImageAsync(NsfwCategory category = NsfwCategory.Waifu)
    {
        return await _client.GetNsfwImageAsync(category);
    }
}