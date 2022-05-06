using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Humanizer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Plunger.Data;
using Plunger.Data.Entities;

namespace Plunger.Modules.Admin;

[EnabledInDm(false)]
[Group("lockdown", "lockdown commands")]
public class SlashLockdown : PlungerInteractionModuleBase
{
    public SlashLockdown(
        IConfiguration configuration,
        IHostEnvironment hostEnvironment,
        IHttpClientFactory httpClientFactory,
        ILogger<SlashLockdown> logger,
        PlungerDbContext database) : base(configuration, hostEnvironment, httpClientFactory, logger, database)
    {
    }

    [SlashCommand("lock", "Lockdown the channel")]
    public async Task Lock(
        [Summary(description: "Duration of the lockdown")] TimeSpan duration,
        [Summary(description: "Reason for the lockdown")] string reason = "None")
    {
        await DeferAsync();
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

        var totalDuration = new TimestampTag { Style = TimestampTagStyles.Relative, Time = DateTimeOffset.Now.AddSeconds(1) + duration };
        var message = await FollowupAsync(embed: embed
                    .WithColor(Color.DarkRed)
                    .WithDescription(
                        $"🔒 | This channel is under lockdown | Reason: {reason} | Lockdown will be lifted " +
                        $"{totalDuration}")
                    .Build());
        var lockdown = await Database.Lockdowns!.AddAsync(new LockdownEntity()
        {
            GuildId = guild.Id,
            GuildName = guild.Name,
            ChannelId = channel.Id,
            ChannelName = channel.Name.Transform(To.TitleCase),
            MessageId = message.Id,
            Duration = totalDuration.Time,
            Reason = reason
        });
        await Database.SaveChangesAsync();
        await Task.Delay(duration.Subtract(TimeSpan.FromSeconds(1.0)));
        await channel.AddPermissionOverwriteAsync(
            guild.EveryoneRole,
            channel.GetPermissionOverwrite(guild.EveryoneRole)!.Value.Modify(sendMessages: PermValue.Allow));
        Database.Lockdowns.Remove(lockdown.Entity);
        await Database.SaveChangesAsync();
        await message.ModifyAsync(x =>
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
        var lockdown = Database.Lockdowns!.FirstOrDefault(x => x.GuildId == Context.Guild.Id && x.ChannelId == channel!.Id);

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
        Database.Lockdowns!.Remove(lockdown!);
        await Database.SaveChangesAsync();
        await Context.Interaction.RespondAsync(embed: embed
            .WithColor(Color.Green)
            .WithDescription("🔓 | The lockdown has been lifted")
            .Build());
    }
}
