using Discord.Interactions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Plunger.APIs;
using Plunger.APIs.Interfaces;
using Plunger.APIs.Models.Enums;
using Plunger.Database;

namespace Plunger.Modules;

public class SlashNsfw : PlungerInteractionModuleBase
{
    private readonly  IWaifu _waifu;

    public SlashNsfw(
        IConfiguration configuration,
        IHostEnvironment hostEnvironment,
        IHttpClientFactory httpClientFactory,
        ILogger<PlungerInteractionModuleBase> logger,
        PlungerDatabase database,
        IWaifu waifu) : base(configuration, hostEnvironment, httpClientFactory, logger, database)
    {
        _waifu = waifu;
    }

    [SlashCommand("waifu-pics-nsfw", "Sends a picture of a nsfw waifu")]
    public async Task WaifuPicsNsfw(NsfwCategory category)
    {
        await RespondAsync(_waifu.GetNsfwImageAsync(category).Result.ImageUrl);
    }
}