using Discord;
using Discord.Addons.Hosting;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Plunger.Commons;
using Plunger.Data;
using Plunger.Modules.Admin.ChatFilters;

namespace Plunger.Services;

public class EventListenerService : PlungerService
{
    public EventListenerService(
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
        Client.MessageReceived += ChatFilterListener;
        return Task.CompletedTask;
    }

    private Task ChatFilterListener(SocketMessage message)
    {
        var channel = message.Channel as SocketGuildChannel;
        var messageContent = message.Content.ToLower().Split(" ").ToList();

        if (message.Author.IsBot) return Task.CompletedTask;
        var Filter = ChatFilterCache.Filter[channel!.Guild.Id];
        if (Filter is null) return Task.CompletedTask;

        List<string> wordsUsed = new();
        bool shouldDelete = false;

        messageContent.ForEach(word =>
        {
            if (Filter.Contains(word))
            {
                wordsUsed.Add(word);
                shouldDelete = true;
            }

            if (shouldDelete)
            {
                message.DeleteAsync();
            }
        });

        if (wordsUsed is not null)
        {
            var id = ChatFilterCache.FilterLogs[channel.Guild.Id];
            string channelId = id.ToString();
            if (channelId is null) return Task.CompletedTask;
            var currentChannel = channel.Guild.GetChannel(id) as SocketTextChannel;

            var Embed = new EmbedBuilder()
                .WithColor(Colors.Random)
                .WithAuthor($"{message.Author.Username}#{message.Author.Discriminator}"
                    , message.Author.GetAvatarUrl() ?? message.Author.GetDefaultAvatarUrl())
                .WithDescription($"Used {wordsUsed.Count} filtered word in <#{channel.Id}>")
                .AddField(x =>
                {
                    x.Name = "**Filtered Word Used**";
                    wordsUsed.ForEach((w) =>
                    {
                        x.Value += $"{w}\n";
                    });
                }).Build();
            currentChannel!.SendMessageAsync(embed: Embed);
        }
        return Task.CompletedTask;
    }
}
