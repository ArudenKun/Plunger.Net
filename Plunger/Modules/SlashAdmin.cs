using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Humanizer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Plunger.Database;
using Plunger.Database.Models;

namespace Plunger.Modules;

[Group("admin", "admin commands")]
[RequireUserPermission(GuildPermission.Administrator, Group = "Admin")]
[RequireUserPermission(GuildPermission.ManageMessages, Group = "Admin")]
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

    [Group("lockdown", "lockdown commands")]
    public class SlashLockdown : PlungerInteractionModuleBase
    {
        public SlashLockdown(
            IConfiguration configuration,
            IHostEnvironment hostEnvironment,
            IHttpClientFactory httpClientFactory,
            ILogger<PlungerInteractionModuleBase> logger,
            PlungerDatabase database) : base(configuration, hostEnvironment, httpClientFactory, logger, database)
        {
        }

        [SlashCommand("lock", "Lockdown the channel")]
        public async Task Lock(
            [Summary(description: "Duration of the lockdown")] TimeSpan duration,
            [Summary(description: "Reason for the lockdown")] string reason = "None")
        {
            var totalDuration = DateTimeOffset.Now.ToUnixTimeMilliseconds() + duration.TotalMilliseconds;
            var guild = Context.Guild;
            var channel = Context.Interaction.Channel as SocketTextChannel;
            var embed = new EmbedBuilder();

            if (channel!.PermissionOverwrites.FirstOrDefault().Permissions.SendMessages == PermValue.Deny)
            {
                await RespondAsync(embed: embed
                    .WithColor(Color.DarkRed)
                    .WithDescription("⛔ | This channel is already locked")
                    .Build());
                return;
            }

            await channel.AddPermissionOverwriteAsync(
                guild.EveryoneRole,
                channel.GetPermissionOverwrite(guild.EveryoneRole)!.Value.Modify(sendMessages: PermValue.Deny));

            await Database.InsertDocumentAsync(LockdownCollection, new LockdownModel()
            {
                GuildId = guild.Id,
                ChannelId = channel.Id,
                ChannelName = channel.Name.Transform(To.TitleCase),
                Duration = totalDuration,
                Reason = reason
            });
            await RespondAsync(embed: embed
                        .WithColor(Color.DarkRed)
                        .WithDescription(
                            $"🔒 | This channel is under lockdown | Reason: {reason} | Lockdown will be lifted " +
                            $"<t:{Convert.ToInt64(totalDuration) / 1000}:R>")
                        .Build());

            await Task.Delay(duration);
            await channel.AddPermissionOverwriteAsync(
                guild.EveryoneRole,
                channel.GetPermissionOverwrite(guild.EveryoneRole)!.Value.Modify(sendMessages: PermValue.Allow));

            await Database.DeleteDocumentAsync(
                LockdownCollection,
                await Database.FindDocumentAsync<LockdownModel>(LockdownCollection, _ => _.GuildId == guild.Id && _.ChannelId == channel.Id));
            await Context.Interaction.ModifyOriginalResponseAsync(x =>
            {
                x.Embed = embed
                .WithColor(Color.Green)
                .WithDescription("🔓 | The lockdown has been lifted")
                .Build();
            });
        }

        [SlashCommand("unlock", "Unlock the channel from the lockdown")]
        public async Task Unlock()
        {
            var guild = Context.Guild;
            var channel = Context.Interaction.Channel as SocketTextChannel;
            var embed = new EmbedBuilder();
            var lockdown = await Database.FindDocumentAsync<LockdownModel>(LockdownCollection, _ => _.GuildId == guild.Id && _.ChannelId == channel!.Id);

            if (channel!.PermissionOverwrites.FirstOrDefault().Permissions.SendMessages == PermValue.Allow || lockdown is null)
            {
                await RespondAsync(embed: embed
                    .WithColor(Color.DarkRed)
                    .WithDescription("⛔ | This channel is not locked")
                    .Build());
                return;
            }
            
            await channel.AddPermissionOverwriteAsync(
                guild.EveryoneRole,
                channel.GetPermissionOverwrite(guild.EveryoneRole)!.Value.Modify(sendMessages: PermValue.Allow));
            await Database.DeleteDocumentAsync(LockdownCollection, lockdown);
            await Context.Interaction.RespondAsync(embed: embed
                .WithColor(Color.Green)
                .WithDescription("🔓 | The lockdown has been lifted")
                .Build());
        }
    }

    [Group("moderation", "moderation commands")]
    public class AdminModeration : PlungerInteractionModuleBase
    {
        public AdminModeration(
            IConfiguration configuration,
            IHostEnvironment hostEnvironment,
            IHttpClientFactory httpClientFactory,
            ILogger<PlungerInteractionModuleBase> logger,
            PlungerDatabase database) : base(configuration, hostEnvironment, httpClientFactory, logger, database)
        {
        }

        [SlashCommand("clear", "Deletes messages in the channel (Default: 1, Max: 100)")]
        public async Task Clear(
            [Summary(description: "The amount of messages you want to delete")] int Amount = 1,
            [Summary(description: "The user messages that you want to delete")] IUser? Target = null)
        {
            var messages = await Context.Channel.GetMessagesAsync(Amount + 1).FlattenAsync();
            var Embed = new EmbedBuilder()
                    .WithColor(Color.DarkRed);

            if (Amount > 100)
            {
                await RespondAsync("```⛔ The maximum amount of messages you can delete at a time is 100```");
                return;
            }

            if (Target == null)
            {
                await ((SocketTextChannel)Context.Interaction.Channel).DeleteMessagesAsync(messages);
                _ = RespondAsync(embed: Embed
                    .WithDescription($"🧹 {messages.Count() - 1} messages has been deleted")
                    .WithCurrentTimestamp()
                    .Build());
                await Task.Delay(2000);
                _ = DeleteOriginalResponseAsync();
                return;
            }
            else
            {
                int i = 0;
                List<IMessage> Filter = new();
                foreach (var message in messages)
                {
                    if (message.Author.Id == Target.Id && Amount > i)
                    {
                        Filter.Add(message);
                        i++;
                    }
                }
                await ((SocketTextChannel)Context.Interaction.Channel).DeleteMessagesAsync(Filter);
                _ = RespondAsync(embed: Embed
                    .WithDescription($"🧹 {messages.Count() - 1} messages has been deleted from {Target}")
                    .WithCurrentTimestamp()
                    .Build());
                await Task.Delay(2000);
                _ = DeleteOriginalResponseAsync();
                return;
            }
        }
    }
}
