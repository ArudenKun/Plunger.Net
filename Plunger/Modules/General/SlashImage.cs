using Discord.Interactions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Plunger.APIs.Waifu.Pics;
using Plunger.APIs.Waifu.Pics.Enums;
using Plunger.Data;

namespace Plunger.Modules.General;

[EnabledInDm(false)]
[Group("image", "gets an image")]
public class SlashImage : PlungerInteractionModuleBase
{
    private readonly IWaifuPicsClient _waifuPicsClient;
    public SlashImage(
        IConfiguration configuration,
        IHostEnvironment hostEnvironment,
        IHttpClientFactory httpClientFactory,
        ILogger<SlashImage> logger,
        PlungerDbContext database,
        IWaifuPicsClient waifuClient) : base(configuration, hostEnvironment, httpClientFactory, logger, database)
    {
        _waifuPicsClient = waifuClient;
    }

    [SlashCommand("nsfw", "Not safe for work image")]
    public async Task Nsfw(NsfwCategory category = NsfwCategory.Waifu)
    {
        await DeferAsync();
        var image = await _waifuPicsClient.GetNsfwImageAsync(category);
        await FollowupAsync(image);
    }

    [SlashCommand("sfw", "Safe for work image")]
    public async Task Sfw(SfwCategory category = SfwCategory.Waifu)
    {
        await DeferAsync();
        var image = await _waifuPicsClient.GetSfwImageAsync(category);
        await FollowupAsync(image);
    }
}