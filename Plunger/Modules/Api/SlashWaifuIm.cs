using Discord.Interactions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Plunger.APIs.Waifu.Im;
using Plunger.APIs.Waifu.Im.Endpoints;
using Plunger.APIs.Waifu.Im.Parameters;
using Plunger.Data;

namespace Plunger.Modules.Api;

[RequireNsfw]
[Group("waifu-im", "waifu.im Api")]
public class SlashWaifuIm : PlungerInteractionModuleBase
{
    private readonly IWaifuImClient _client;
    public SlashWaifuIm(
        IConfiguration configuration,
        IHostEnvironment hostEnvironment,
        IHttpClientFactory httpClientFactory,
        ILogger<PlungerInteractionModuleBase> logger,
        PlungerDbContext database,
        IWaifuImClient client) : base(configuration, hostEnvironment, httpClientFactory, logger, database)
    {
        _client = client;
    }

    [SlashCommand("nsfw", "gets nsfw image/gif")]
    public async Task Nsfw(WaifuImNsfwTags tags, bool gif)
    {
        await DeferAsync();
        await FollowupAsync(await _client.GetRandomNsfwAsync(
            new WaifuRandomParams<WaifuImNsfwTags> { IsNsfw = true, SelectedTags = tags, Gif = gif }));
    }

    [SlashCommand("sfw", "gest a sfw image/gif")]
    public async Task Sfw(WaifuImVersatileTags tags, [Summary(description: "Tries to find a gif if none returns a normal image")]bool gif)
    {
        await DeferAsync();
        await FollowupAsync(await _client.GetRandomSfwAsync(
            new WaifuRandomParams<WaifuImVersatileTags> { IsNsfw = false, SelectedTags = tags, Gif = gif }));
    }
}
