using Discord.Interactions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Plunger.APIs.Reddit;
using Plunger.Data;

namespace Plunger.Modules.Nsfw;

[EnabledInDm(false)]
[RequireNsfw]
[Group("reddit", "reddit subreddit")]
public class SlashReddit : PlungerInteractionModuleBase
{
    private readonly IRedditClient _redditClient;
    public SlashReddit(
        IConfiguration configuration,
        IHostEnvironment hostEnvironment,
        IHttpClientFactory httpClientFactory,
        ILogger<PlungerInteractionModuleBase> logger,
        PlungerDbContext database,
        IRedditClient redditClient) : base(configuration, hostEnvironment, httpClientFactory, logger, database)
    {
        _redditClient = redditClient;
    }

    [SlashCommand("creampie", "gets a gif/image of a creampie")]
    public async Task Creampie()
    {
        await DeferAsync();
        // var client = HttpClientFactory.CreateClient();
        // var result = await client.GetStringAsync($"https://reddit.com/r/creampies/random.json?limit=1");
        // if (!result.StartsWith("["))
        // {
        //     await FollowupAsync("This Subredddit Does Not Exist");
        //     return;
        // }
        // var array = JsonNode.Parse(result);
        // var post = JsonNode.Parse(array![0]!["data"]!["children"]![0]!["data"]!.ToString());

        // await FollowupAsync(post!["url_overridden_by_dest"]!.ToString());
        await FollowupAsync(await _redditClient.Creampie());
    }
}
