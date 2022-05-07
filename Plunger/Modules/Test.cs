using Discord.Interactions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Plunger.APIs.Hastebin;
using Plunger.APIs.Nekobot;
using Plunger.APIs.Nekobot.Endpoints;
using Plunger.APIs.Popcat;
using Plunger.APIs.Popcat.Paremeters;
using Plunger.APIs.Reddit;
using Plunger.Data;

namespace Plunger.Modules;

public class Test : PlungerInteractionModuleBase
{
    private readonly IPopcatClient _popcatClient;
    private readonly INekobotClient _nekobotClient;
    private readonly IRedditClient _redditClient;
    public Test(
        IConfiguration configuration,
        IHostEnvironment hostEnvironment,
        IHttpClientFactory httpClientFactory,
        ILogger<PlungerInteractionModuleBase> logger,
        PlungerDbContext database,
        IPopcatClient popcatClient,
        INekobotClient nekobotClient,
        IRedditClient redditClient) : base(configuration, hostEnvironment, httpClientFactory, logger, database)
    {
        _popcatClient = popcatClient;
        _nekobotClient = nekobotClient;
        _redditClient = redditClient;
    }

    [SlashCommand("timepsan", "testing")]
    public async Task TestAsync([Summary("time", "input time")] TimeSpan timeSpan)
    {

        await RespondAsync($"{timeSpan}");
    }

    [SlashCommand("code", "coding shit")]
    public async Task Code(string code)
    {
        await DeferAsync();
        HastebinClient client = new();
        var url = await client.UploadAsync(code);
        await FollowupAsync(url);
    }

    [SlashCommand("bot", "coding shit")]
    public async Task Bot(string message)
    {
        await DeferAsync();
        var bot = await _popcatClient.Chatbot(new ChatbotParams { Message = message });
        await FollowupAsync(bot.Response);
    }

    [SlashCommand("nekobot", "coding shit")]
    public async Task Nekobot(NekobotTypes types)
    {
        await DeferAsync();
        await FollowupAsync(await _nekobotClient.GetImage(types));
    }
}