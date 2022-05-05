using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Plunger.Data;

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
        ILogger<SlashAdmin> logger,
        PlungerDbContext database,
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

    [Group("moderation", "moderation commands")]
    public class AdminModeration : PlungerInteractionModuleBase
    {
        public AdminModeration(
            IConfiguration configuration,
            IHostEnvironment hostEnvironment,
            IHttpClientFactory httpClientFactory,
            ILogger<AdminModeration> logger,
            PlungerDbContext database) : base(configuration, hostEnvironment, httpClientFactory, logger, database)
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
