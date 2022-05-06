using Discord;
using Discord.Interactions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Plunger.Data;
using Plunger.Data.Entities;
using Plunger.Modules.Admin.ChatFilter;

namespace Plunger.Modules.Admin.ChatFilters;

[EnabledInDm(false)]
[Group("chat-filter", "Chat filter commands")]
[RequireUserPermission(GuildPermission.Administrator | GuildPermission.ManageChannels | GuildPermission.ManageMessages)]
public class SlashChatFilter : PlungerInteractionModuleBase
{

    public SlashChatFilter(
        IConfiguration configuration,
        IHostEnvironment hostEnvironment,
        IHttpClientFactory httpClientFactory,
        ILogger<SlashChatFilter> logger,
        PlungerDbContext database) : base(configuration, hostEnvironment, httpClientFactory, logger, database)
    {
    }

    [SlashCommand("settings", "Settings for the chat filter")]
    public async Task Settings(
        [Summary(description: "Select the logging channel")] ITextChannel channel)
    {

        // var settings = await Database.Guilds!.FindAsync(Context.Guild.Id);
        // if (settings == null)
        // {
        //     await Database.Guilds.AddAsync(new GuildEntity()
        //     {
        //         Id = Context.Guild.Id,
        //         LoggingChannelId = channel.Id
        //     });
        //     await Database.SaveChangesAsync();
        //     ChatFilterCache.FilterLogs.Add(Context.Guild.Id, channel.Id);
        //     await RespondAsync($"<#{channel.Id}>has been set as the logging channel for the chat filter", ephemeral: true);
        //     return;
        // }
        // ChatFilterCache.FilterLogs.Add(Context.Guild.Id, channel.Id);
        // settings.LoggingChannelId = channel.Id;
        // Database.Guilds.Update(settings);
        // await Database.SaveChangesAsync();
        await Database.Guilds!.Upsert(new GuildEntity
        {
            Id = Context.Guild.Id,
            LoggingChannelId = channel.Id
        })
        .WhenMatched(g => new GuildEntity
        {
            LoggingChannelId = channel.Id
        })
        .RunAsync();
        await Database.SaveChangesAsync();
        if (ChatFilterCache.FilterLogs.ContainsValue(channel.Id))
        {
            await RespondAsync($"<#{channel.Id}> has been set as the logging channel for the chat filter", ephemeral: true);
            return;
        }
        ChatFilterCache.FilterLogs.Add(Context.Guild.Id, channel.Id);
        await RespondAsync($"<#{channel.Id}> has been set as the logging channel for the chat filter", ephemeral: true);
    }


    [SlashCommand("configuration", "Chat filter configuration")]
    public async Task Configurations(
        [Summary(description: "Add or remove words to the filter")] ChatFilterOption option,
        [Summary(description: "Words to add or remove to the filter Ex. Fuck,Money,Cheat")] string words)
    {
        var config = await Database.Guilds!.FindAsync(Context.Guild.Id);
        var Words = words.ToLower().Split(",").ToList();
        switch (option)
        {
            case ChatFilterOption.Add:
                if (config == null)
                {
                    await Database.Guilds!.AddAsync(new GuildEntity()
                    {
                        Id = Context.Guild.Id,
                        Words = Words
                    });
                    await Database.SaveChangesAsync();
                    ChatFilterCache.Filter.Add(Context.Guild.Id, Words);
                    await RespondAsync($"Added {Words.Count} new word(s) to the filter");
                    return;
                }

                List<string> newWords = new();
                foreach (var word in Words)
                {
                    if (config.Words!.Contains(word))
                    {
                        await RespondAsync("Words are already blacklisted");
                        return;
                    };
                    newWords.Add(word);
                    config.Words.Add(word);

                    var k = Context.Guild.Id;
                    if (ChatFilterCache.Filter.ContainsKey(Context.Guild.Id))
                    {
                        ChatFilterCache.Filter[k].Add(word);
                    }
                    else
                    {
                        ChatFilterCache.Filter.Add(k, new List<string> { word });
                    }
                }
                Database.Guilds!.Update(config);
                await Database.SaveChangesAsync();
                await RespondAsync($"Added {newWords.Count} new word(s) to the filter");
                break;
            case ChatFilterOption.Remove:
                if (config == null)
                {
                    await RespondAsync("There is no word to remove");
                    return;
                }
                List<string> removedWords = new();
                Words.ForEach(async word =>
                {
                    if (!config.Words.Contains(word))
                    {
                        await RespondAsync("Word(s) does not exist");
                        return;
                    }
                    removedWords.Add(word);
                    config.Words.Remove(word);

                    var k = Context.Guild.Id;
                    if (ChatFilterCache.Filter.ContainsKey(Context.Guild.Id))
                    {
                        ChatFilterCache.Filter[k].Remove(word).ToString();
                    }
                });
                Database.Guilds.Update(config);
                await Database.SaveChangesAsync();
                await RespondAsync($"Removed {removedWords.Count} word(s) to the filter");
                break;
            default:
                await RespondAsync("Error");
                break;
        }
    }
}
