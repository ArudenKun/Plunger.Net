using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Plunger.Database;

namespace Plunger.Modules;

[RequireOwner(Group = "Admin")]
[RequireUserPermission(GuildPermission.Administrator, Group = "Admin")]
[RequireUserPermission(GuildPermission.ManageChannels, Group = "Admin")]
public class SlashAdmin : PlungerInteractionModuleBase
{
    private readonly IHost _host;

    public SlashAdmin(
        IConfiguration configuration,
        IHostEnvironment hostEnvironment,
        IHttpClientFactory httpClientFactory,
        ILogger<PlungerInteractionModuleBase> logger,
        PlungerDatabase database,
        IHost host) : base(configuration, hostEnvironment, httpClientFactory, logger, database)
    {
        _host = host;
    }

    [SlashCommand("shutdown", "Turns off the bot")]
    public async Task Shutdown()
    {
        await RespondAsync("```Shutting Down....```");
        await Task.Delay(1000);
        await Context.Interaction.ModifyOriginalResponseAsync(x =>
        {
            x.Content = "```The Bot Has been shutdown```";
        });
        await Task.Delay(2000);
        var msg = await Context.Channel.GetMessagesAsync(1).FlattenAsync();
        await ((SocketTextChannel)Context.Interaction.Channel).DeleteMessagesAsync(msg);
        await _host.StopAsync();
    }
}
