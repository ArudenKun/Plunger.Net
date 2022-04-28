using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Plunger.Attributes;
using Plunger.Database;

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

    [SlashCommand("timepsan", "testing")]
    public async Task TestAsync([Summary("time", "input time")] TimeSpan timeSpan)
    {
        
        await RespondAsync($"{timeSpan}");
    }
}
