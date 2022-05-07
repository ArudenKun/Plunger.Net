using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Plunger.Data;
using Plunger.Data.Entities;

namespace Plunger.Modules.Admin;

[EnabledInDm(false)]
[RequireOwner(Group = "Admin")]
[RequireUserPermission(GuildPermission.Administrator, Group = "Admin")]
[Group("chatbot", "chatbot command")]
public class SlashChatBot : PlungerInteractionModuleBase
{
    public SlashChatBot(
        IConfiguration configuration,
        IHostEnvironment hostEnvironment,
        IHttpClientFactory httpClientFactory,
        ILogger<PlungerInteractionModuleBase> logger,
        PlungerDbContext database) : base(configuration, hostEnvironment, httpClientFactory, logger, database)
    {
    }

    [SlashCommand("configure", "configure the chatbot")]
    public async Task Configure(SocketTextChannel channel)
    {
        await DeferAsync();
        await Database.Guilds!.Upsert(new GuildEntity
        {
            Id = Context.Guild.Id,
            GuildName = Context.Guild.Name,
            ChatBotChannelId = channel.Id
        })
        .WhenMatched(x => new GuildEntity
        {
            ChatBotChannelId = channel.Id
        })
        .RunAsync();
        await FollowupAsync($"<#{channel.Id}> has been set as the logging channel for the chat filter", ephemeral: true);
    }
}