using Discord;
using Discord.Addons.Hosting;
using Discord.Commands;
using Discord.Interactions;
using Discord.Rest;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Plunger.Data;
using Plunger.Modules.Admin.ChatFilters;

namespace Plunger.Services;

public class EventsService : PlungerService
{
    public EventsService(
        DiscordSocketClient client,
        ILogger<DiscordClientService> logger,
        IConfiguration configuration,
        IHostEnvironment environment,
        IServiceProvider serviceProvider,
        CommandService commandService,
        InteractionService interactionService,
        PlungerDbContext database) : base(client, logger, configuration, environment, serviceProvider, commandService, interactionService, database)
    {
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Client.Ready += LockdownCheckEvent;
        Client.Ready += ChatFilterEvent;
        return Task.CompletedTask;
    }

    private Task ChatFilterEvent()
    {
        foreach (var guild in Client.Guilds)
        {
            var document = Database.Guilds!.Where(x => x.Id == guild.Id).ToList();

            if (document == null) return Task.CompletedTask;

            document.ForEach(x =>
            {
                var k = guild.Id;
                x.Words.ForEach(w =>
                {
                    if (ChatFilterCache.Filter.ContainsKey(k))
                    {
                        ChatFilterCache.Filter[k].Add(w);
                    }
                    else
                    {
                        ChatFilterCache.Filter.Add(k, new List<string> { w });
                    }
                });
                ChatFilterCache.FilterLogs.Add(k, x.LoggingChannelId);

            });

        }
        return Task.CompletedTask;
    }

    private Task LockdownCheckEvent()
    {
        foreach (var guild in Client.Guilds)
        {
            var lockdown = Database.Lockdowns!.Where(x => x.GuildId == guild.Id).ToList();
            if (lockdown == null) return Task.CompletedTask;
            lockdown.ForEach(async d =>
            {
                if (guild.Channels.FirstOrDefault(x => x.Id == d.ChannelId) is not SocketTextChannel channel) return;
                if (await channel.GetMessageAsync(d.MessageId) is not IUserMessage message) return;
                var TimeNow = DateTimeOffset.Now;
                if (d.Duration < TimeNow)
                {
                    await channel.AddPermissionOverwriteAsync(
                        channel.Guild.EveryoneRole,
                        channel.GetPermissionOverwrite(guild.EveryoneRole)!.Value.Modify(sendMessages: PermValue.Allow));
                    Database.Remove(d);
                    await Database.SaveChangesAsync();
                    await message!.ModifyAsync(x =>
                    {
                        x.Embed = new EmbedBuilder()
                            .WithColor(Color.Green)
                            .WithDescription("🔓 | The lockdown has been lifted")
                            .Build();
                    });
                    return;
                }

                var Expire = d.Duration - DateTimeOffset.Now;
                await Task.Delay(Expire);
                await channel.AddPermissionOverwriteAsync(
                        channel.Guild.EveryoneRole,
                        channel.GetPermissionOverwrite(guild.EveryoneRole)!.Value.Modify(sendMessages: PermValue.Allow));
                Database.Remove(d);
                await Database.SaveChangesAsync();
                await message!.ModifyAsync(x =>
                {
                    x.Embed = new EmbedBuilder()
                        .WithColor(Color.Green)
                        .WithDescription("🔓 | The lockdown has been lifted")
                        .Build();
                });
            });
        }
        return Task.CompletedTask;
    }
}