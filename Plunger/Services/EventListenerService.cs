using System.Linq.Expressions;
using Discord;
using Discord.Addons.Hosting;
using Discord.Addons.Hosting.Util;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Plunger.APIs.Popcat;
using Plunger.APIs.Popcat.Paremeters;
using Plunger.Commons;
using Plunger.Data;
using Plunger.Modules.Admin.ChatFilters;

namespace Plunger.Services;

public class EventListenerService : PlungerService
{
    private readonly IPopcatClient _popcatClient;
    public EventListenerService(
        DiscordSocketClient client,
        ILogger<EventListenerService> logger,
        IConfiguration configuration,
        IHostEnvironment environment,
        IServiceProvider serviceProvider,
        CommandService commandService,
        InteractionService interactionService,
        PlungerDbContext database,
        IPopcatClient popcatClient) : base(client, logger, configuration, environment, serviceProvider, commandService, interactionService, database)
    {
        _popcatClient = popcatClient;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Client.MessageReceived += MessageReceivedListeners;
        // Client.MessageReceived += ChatFilterListener;
        // Client.MessageReceived += ChatBotListener;
        return Task.CompletedTask;
    }

    private async Task MessageReceivedListeners(SocketMessage arg)
    {
        await ChatFilterListener(arg);
        await ChatBotListener(arg);
    }

    private static async Task ChatFilterListener(SocketMessage message)
    {
        var originalChannel = message.Channel as SocketGuildChannel;
        var messageContent = message.Content.ToLower();

        if (message.Author.IsBot) return;
        var Filter = ChatFilterCache.Filter[originalChannel!.Guild.Id];
        if (Filter is null) return;

        bool anyMatch = Filter.Any(f => f.Contains(messageContent));
        if (anyMatch)
        {
            await message.DeleteAsync();
            SocketGuildChannel channel = originalChannel!.Guild.GetTextChannel(id: ChatFilterCache.FilterLogs[originalChannel.Guild.Id]);
            var Embed = new EmbedBuilder()
                .WithColor(Colors.Random)
                .WithAuthor($"{message.Author.Username}#{message.Author.Discriminator}",
                    message.Author.GetAvatarUrl() ?? message.Author.GetDefaultAvatarUrl())
                .WithDescription($"Used a filtered word in <#{originalChannel.Id}>")
                .Build();

            await ((SocketTextChannel)channel).SendMessageAsync(embed: Embed);
        }
        else
        {
            return;
        }
        // List<string> wordsUsed = new();
        // bool shouldDelete = false;
        // messageContent.ForEach(word =>
        // {
        //     if (Filter.Contains(word))
        //     {
        //         wordsUsed.Add(word);
        //         shouldDelete = true;
        //     }
        // });
        // if (shouldDelete)
        // {
        //     await message.DeleteAsync();
        // }
        // else return;

        // if (wordsUsed is not null)
        // {
        //     var id = ChatFilterCache.FilterLogs[originalChannel.Guild.Id];
        //     string channelId = id.ToString();
        //     if (channelId is null) return;
        //     if (originalChannel.Guild.GetChannel(id) is not SocketTextChannel channel) return;

        //     var Embed = new EmbedBuilder()
        //         .WithColor(Colors.Random)
        //         .WithAuthor($"{message.Author.Username}#{message.Author.Discriminator}",
        //             message.Author.GetAvatarUrl() ?? message.Author.GetDefaultAvatarUrl())
        //         .WithDescription($"Used {wordsUsed.Count} filtered word in <#{originalChannel.Id}>")
        //         .AddField(x =>
        //         {
        //             x.Name = "**Filtered Word Used**";
        //             wordsUsed.ForEach((w) =>
        //             {
        //                 x.Value += $"{w}\n";
        //             });
        //         }).Build();


        //     await channel!.SendMessageAsync(embed: Embed);
        // }
    }

    private async Task ChatBotListener(SocketMessage message)
    {
        var channel = message.Channel as SocketTextChannel;
        var author = message.Author;
        var data = await Database.Guilds!.FirstOrDefaultAsync(x => x.Id == channel!.Guild.Id);
        if (data is null) return;
        // if (Data.ChatBotChannelId == 0)
        // {
        //      Channel!.EnterTypingState();
        //      await Channel!.SendMessageAsync("Please set up your chatbot on what channel he should listen");
        //      return;
        // }

        // Logger.LogInformation($"{Channel!.Id} {Data.ChatBotChannelId}");
        if (message.Author.IsBot) return;
        if (channel!.Id != data!.ChatBotChannelId) return;

        var chatbot = await _popcatClient.Chatbot(new ChatbotParams { Message = message.Content });
        await channel!.SendMessageAsync(chatbot.Response);
    }
}