using Discord.Interactions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Plunger.APIs.WaifuPics;
using Plunger.APIs.WaifuPics.Entities;
using Plunger.Data;

namespace Plunger.Modules.General;

[EnabledInDm(false)]
[Group("image", "gets an image")]
public class SlashImage : PlungerInteractionModuleBase
{
    private readonly WaifuClient _waifuClient;
    public SlashImage(
        IConfiguration configuration,
        IHostEnvironment hostEnvironment,
        IHttpClientFactory httpClientFactory,
        ILogger<SlashImage> logger,
        PlungerDbContext database,
        WaifuClient waifuClient) : base(configuration, hostEnvironment, httpClientFactory, logger, database)
    {
        _waifuClient = waifuClient;
    }

    [SlashCommand("nsfw", "Not safe for work image")]
    public async Task Nsfw(NsfwCategory category)
    {
        await DeferAsync();
        await FollowupAsync(await _waifuClient.GetRandomNsfwAsync(category));
    }

    [SlashCommand("sfw", "Safe for work image")]
    public async Task Sfw(SfwCategory category)
    {
        await DeferAsync();
        await FollowupAsync(await _waifuClient.GetRandomSfwAsync(category));
    }
}
