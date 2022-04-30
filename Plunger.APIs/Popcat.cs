using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Plunger.APIs.Interfaces;
using Plunger.APIs.Models;
using Refit;

namespace Plunger.APIs;

public class Popcat : IPopcat
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

    private readonly IPopcat _client;

    public Popcat()
    {
        _client = RestService.For<IPopcat>("https://api.popcat.xyz/", Settings);
    }

    public async Task<PopcatResponse> BinaryEncode(string text)
    {
        return await _client.BinaryEncode(text);
    }

    public async Task<PopcatResponse> BinaryDecode(string binary)
    {
        return await _client.BinaryDecode(binary);
    }
}
