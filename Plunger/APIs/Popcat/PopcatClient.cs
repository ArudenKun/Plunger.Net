using Plunger.APIs.Popcat.Paremeters;
using Plunger.APIs.Popcat.Responses;
using Refit;

namespace Plunger.APIs.Popcat;

public class PopcatClient : IPopcatClient
{
    private const string BaseUrl = "https://api.popcat.xyz";
    private readonly IPopcatClient client;

    // private static readonly RefitSettings Settings = new()
    // {
    //     ContentSerializer = new SystemTextJsonContentSerializer(new JsonSerializerOptions
    //     {
    //         PropertyNamingPolicy = Jso
    //     }),
    // };

    public PopcatClient()
    {
        client = RestService.For<IPopcatClient>(BaseUrl, ApiSettings.Settings);
    }

    public async Task<BinaryResponse> Encode(string input)
    {
        return await client.Encode(input);
    }

    public async Task<BinaryResponse> Decode([AliasAs("binary")] string input)
    {
        return await client.Decode(input);
    }

    public async Task<ChatbotResponse> Chatbot(ChatbotParams parameters)
    {
        return await client.Chatbot(parameters);
    }
}