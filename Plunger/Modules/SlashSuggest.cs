using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Plunger.Database;
using Plunger.Database.Models;
using static Plunger.Database.Models.SuggestionModel;
using static Plunger.Enum.Enums;

namespace Plunger.Modules;

public class SlashSuggest : PlungerInteractionModuleBase
{
    public SlashSuggest(
        IConfiguration configuration,
        IHostEnvironment hostEnvironment,
        IHttpClientFactory httpClientFactory,
        ILogger<PlungerInteractionModuleBase> logger,
        PlungerDatabase database) : base(configuration, hostEnvironment, httpClientFactory, logger, database)
    {
    }

    [SlashCommand("suggest", "Creates a suggestion")]
    public async Task Suggest(SuggestionTypes type, string suggestion)
    {
        var Embed = new EmbedBuilder()
            .WithColor(Color.DarkerGrey)
            .WithAuthor($"{Context.Interaction.User.Username}#{Context.Interaction.User.Discriminator}",
            Context.Interaction.User.GetAvatarUrl() ?? Context.Interaction.User.GetDefaultAvatarUrl())
            .AddField("Suggestion", suggestion, false)
            .AddField("Type", type, true)
            .AddField("Status", "Pending", true)
            .WithCurrentTimestamp()
            .Build();

        var Buttons = new ComponentBuilder()
            .WithButton("✔ Accept", "suggest-accept", ButtonStyle.Success)
            .WithButton("❌ Decline", "suggest-decline", ButtonStyle.Danger)
            .WithButton("📑 Finalize", "suggest-final", ButtonStyle.Secondary, disabled: true)
            .Build();

        await DeferAsync();
        // Context.Guild.Id, Context.Interaction.Id, Context.Interaction.User.Id
        await Database.InsertDocumentAsync(SuggestionCollection, new SuggestionModel()
        {
            GuildId = Context.Guild.Id,
            MessageId = Context.Interaction.Id,
            SuggestionDetails = new List<Details>
                {
                    new Details
                    {
                        MemberId = Context.Interaction.User.Id,
                        Type = type.ToString(),
                        Suggestion = suggestion
                    }
                }
        });
        await Context.Interaction.ModifyOriginalResponseAsync(_ =>
        {
            _.Embed = Embed;
            _.Components = Buttons;
        });
    }

    [ComponentInteraction("suggest-accept")]
    [RequireUserPermission(GuildPermission.Administrator)]
    public async Task SuggestAccept()
    {
        var Interaction = Context.Interaction as SocketMessageComponent;

        // if (!await Database.SuggestionExistAsync(Context.Guild.Id, Interaction!.Message.Interaction.Id))
        // {
        //     await RespondAsync("Suggestion Does Not Exist In The Database", ephemeral: true);
        //     return;
        // }

        var suggestion = await Database.FindDocumentAsync<SuggestionModel>(SuggestionCollection,
            _ => _.GuildId == Context.Guild.Id && _.MessageId == Interaction!.Message.Interaction.Id);

        if (suggestion is null)
        {
            await RespondAsync("Suggestion Does Not Exist In The Database", ephemeral: true);
            return;
        }

        await Interaction!.UpdateAsync(x =>
        {
            var suggestion = Interaction.Message.Embeds.FirstOrDefault()!.Fields[0].Value;
            var type = Interaction.Message.Embeds.FirstOrDefault()!.Fields[1].Value;
            var newButtons = new ComponentBuilder()
            .WithButton("✔ Accept", "suggest-accept", ButtonStyle.Success, disabled: true)
            .WithButton("❌ Decline", "suggest-decline", ButtonStyle.Danger, disabled: false)
            .WithButton("📑 Finalize", "suggest-final", ButtonStyle.Secondary, disabled: false)
            .Build();

            var newEmbed = new EmbedBuilder()
                .WithColor(Color.Green)
                .WithAuthor($"{Context.Interaction.User.Username}#{Context.Interaction.User.Discriminator}",
                Context.Interaction.User.GetAvatarUrl() ?? Context.Interaction.User.GetDefaultAvatarUrl())
                .AddField("Suggestion", suggestion, false)
                .AddField("Type", type, true)
                .AddField("Status", "Accepted", true)
                .WithCurrentTimestamp()
                .Build();

            x.Embed = newEmbed;
            x.Components = newButtons;
        });

        if (Interaction.HasResponded)
        {
            await FollowupAsync("Suggestion Accepted", ephemeral: true);
            return;
        }
        await RespondAsync("Suggestion Accepted", ephemeral: true);
    }

    [ComponentInteraction("suggest-decline")]
    [RequireUserPermission(GuildPermission.Administrator)]
    public async Task SuggestDecline()
    {
        var Interaction = Context.Interaction as SocketMessageComponent;

        // if (!await Database.SuggestionExistAsync(Context.Guild.Id, Interaction!.Message.Interaction.Id))
        // {
        //     await RespondAsync("Suggestion Does Not Exist In The Database", ephemeral: true);
        //     return;
        // }

        var suggestion = await Database.FindDocumentAsync<SuggestionModel>(SuggestionCollection,
            _ => _.GuildId == Context.Guild.Id && _.MessageId == Interaction!.Message.Interaction.Id);
        
        if (suggestion is null)
        {
            await RespondAsync("Suggestion Does Not Exist In The Database", ephemeral: true);
            return;
        }

        await Interaction!.UpdateAsync(x =>
        {
            var suggestion = Interaction.Message.Embeds.FirstOrDefault()!.Fields[0].Value;
            var type = Interaction.Message.Embeds.FirstOrDefault()!.Fields[1].Value;
            var newButtons = new ComponentBuilder()
            .WithButton("✔ Accept", "suggest-accept", ButtonStyle.Success, disabled: false)
            .WithButton("❌ Decline", "suggest-decline", ButtonStyle.Danger, disabled: true)
            .WithButton("📑 Finalize", "suggest-final", ButtonStyle.Secondary, disabled: false)
            .Build();

            var newEmbed = new EmbedBuilder()
                .WithColor(Color.DarkRed)
                .WithAuthor($"{Context.Interaction.User.Username}#{Context.Interaction.User.Discriminator}",
                Context.Interaction.User.GetAvatarUrl() ?? Context.Interaction.User.GetDefaultAvatarUrl())
                .AddField("Suggestion", suggestion, false)
                .AddField("Type", type, true)
                .AddField("Status", "Declined", true)
                .WithCurrentTimestamp()
                .Build();

            x.Embed = newEmbed;
            x.Components = newButtons;
        });

        if (Interaction.HasResponded)
        {
            await FollowupAsync("Suggestion Declined", ephemeral: true);
            return;
        }
        await RespondAsync("Suggestion Declined", ephemeral: true);
    }

    [ComponentInteraction("suggest-final")]
    [RequireUserPermission(GuildPermission.Administrator)]
    public async Task SuggestFinal()
    {
        var Interaction = Context.Interaction as SocketMessageComponent;

        // if (!await Database.SuggestionExistAsync(Context.Guild.Id, Interaction!.Message.Interaction.Id))
        // {
        //     await RespondAsync("Suggestion Does Not Exist In The Database", ephemeral: true);
        //     return;
        // }
        var suggestion = await Database.FindDocumentAsync<SuggestionModel>(SuggestionCollection,
            _ => _.GuildId == Context.Guild.Id && _.MessageId == Interaction!.Message.Interaction.Id);

        if (suggestion is null)
        {
            await RespondAsync("Suggestion Does Not Exist In The Database", ephemeral: true);
            return;
        }

        await Interaction!.UpdateAsync(x =>
        {
            string suggestion = Interaction.Message.Embeds.FirstOrDefault()!.Fields[0].Value;
            string type = Interaction.Message.Embeds.FirstOrDefault()!.Fields[1].Value;
            string status = Interaction.Message.Embeds.FirstOrDefault()!.Fields[2].Value;
            var newEmbed = new EmbedBuilder()
                .WithColor(Color.Gold)
                .WithAuthor($"{Context.Interaction.User.Username}#{Context.Interaction.User.Discriminator}",
                Context.Interaction.User.GetAvatarUrl() ?? Context.Interaction.User.GetDefaultAvatarUrl())
                .AddField("Suggestion", suggestion, false)
                .AddField("Type", type, true)
                .AddField("Status", status, true)
                .WithFooter("Suggestion Finalized")
                .WithCurrentTimestamp()
                .Build();
            x.Embed = newEmbed;
            x.Components = null;
        });
        // await Database.DeleteDocumentAsync(Context.Guild.Id, Interaction.Message.Interaction.Id);
        await Database.DeleteDocumentAsync(SuggestionCollection, suggestion);
        if (Interaction.HasResponded)
        {
            await FollowupAsync("Suggestion Finalized", ephemeral: true);
            return;
        }
        await RespondAsync("Suggestion Finalized", ephemeral: true);
    }
}