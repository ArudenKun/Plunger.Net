using Discord.Interactions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Plunger.Data;

namespace Plunger.Modules.General;

public class SlashPing : PlungerInteractionModuleBase
{
    public SlashPing(
        IConfiguration configuration,
        IHostEnvironment hostEnvironment,
        IHttpClientFactory httpClientFactory,
        ILogger<SlashPing> logger,
        PlungerDbContext database) : base(configuration, hostEnvironment, httpClientFactory, logger, database)
    {
    }

    [SlashCommand("ping", "pings the bot")]
    public async Task Ping()
    {
        await DeferAsync();
        var time = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        var msg = await FollowupAsync("Pinging....");
        await msg.ModifyAsync(m => 
        {
            m.Content = $"```Latency: {msg.Timestamp.ToUnixTimeMilliseconds() - Context.Interaction.CreatedAt.ToUnixTimeMilliseconds()}ms ```" +
                $"```API: {Context.Client.Latency}ms```";
        });
    }
}
