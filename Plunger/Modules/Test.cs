using Discord.Interactions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Plunger.Database;
using Plunger.Database.Models;

namespace Plunger.Modules;

public class Test : PlungerInteractionModuleBase
{
    public Test(
        IConfiguration configuration,
        IHostEnvironment hostEnvironment,
        IHttpClientFactory httpClientFactory,
        ILogger<PlungerInteractionModuleBase> logger,
        PlungerDatabase database) : base(configuration, hostEnvironment, httpClientFactory, logger, database)
    {
    }

    [SlashCommand("test", "testing")]
    public async Task TestAsync([Summary("time", "input time")] TimeSpan timeSpan)
    {
        await RespondAsync($"{timeSpan}");
    }
}
