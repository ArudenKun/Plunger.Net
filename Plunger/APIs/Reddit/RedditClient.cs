using System.Text.Json.Nodes;
using Refit;

namespace Plunger.APIs.Reddit;

public class RedditClient : IRedditClient
{
    private const string BaseUrl = "https://www.reddit.com";
    
    private readonly IRedditClient client;

    public RedditClient()
    {
        client = RestService.For<IRedditClient>(BaseUrl, ApiSettings.Settings);
    }

    public async Task<string> Creampie()
    {
        var array = JsonNode.Parse(await client.Creampie());
        var json = JsonNode.Parse(array![0]!["data"]!["children"]![0]!["data"]!.ToString());
        return json!["url_overridden_by_dest"]!.ToString();
    }
}