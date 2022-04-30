using Discord.Interactions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Plunger.APIs;
using Plunger.APIs.Interfaces;
using Plunger.APIs.Models.Enums;
using Plunger.Database;

namespace Plunger.Modules;

public class SlashSfw : PlungerInteractionModuleBase
{
    private readonly IWaifu _waifu;

    public SlashSfw(
        IConfiguration configuration,
        IHostEnvironment hostEnvironment,
        IHttpClientFactory httpClientFactory,
        ILogger<PlungerInteractionModuleBase> logger,
        PlungerDatabase database,
        IWaifu waifu) : base(configuration, hostEnvironment, httpClientFactory, logger, database)
    {
        _waifu = waifu;
    }

    [SlashCommand("waifu-pics-sfw", "Sends a picture of a sfw waifu")]
    public async Task WaifuPicsSfw(SfwCategory category)
    {
        await RespondAsync(_waifu.GetSfwImageAsync(category).Result.ImageUrl);
    }
}
