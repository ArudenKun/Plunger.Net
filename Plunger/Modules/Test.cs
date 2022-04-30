using Discord.Interactions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Plunger.APIs.Interfaces;
using Plunger.Database;

namespace Plunger.Modules;

public class Test : PlungerInteractionModuleBase
{
    private readonly IPopcat _popcat;
    public Test(
        IConfiguration configuration,
        IHostEnvironment hostEnvironment,
        IHttpClientFactory httpClientFactory,
        ILogger<PlungerInteractionModuleBase> logger,
        PlungerDatabase database,
        IPopcat popcat) : base(configuration, hostEnvironment, httpClientFactory, logger, database)
    {
        _popcat = popcat;
    }

    [SlashCommand("timepsan", "testing")]
    public async Task TestAsync([Summary("time", "input time")] TimeSpan timeSpan)
    {
        
        await RespondAsync($"{timeSpan}");
    }
}
