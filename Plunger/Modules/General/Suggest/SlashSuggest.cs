using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Plunger.Data;
using Plunger.Data.Entities;

namespace Plunger.Modules.General.Suggestion;

public class SlashSuggest : PlungerInteractionModuleBase
{
    public SlashSuggest(
        IConfiguration configuration,
        IHostEnvironment hostEnvironment,
        IHttpClientFactory httpClientFactory,
        ILogger<SlashSuggest> logger,
        PlungerDbContext database) : base(configuration, hostEnvironment, httpClientFactory, logger, database)
    {
    }

    [SlashCommand("suggest", "Creates a suggestion")]
    public async Task Suggest(SuggestionType type, string suggestion)
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
        var message = await FollowupAsync(embed: Embed, components: Buttons);
        await Database.Suggestions!.AddAsync(new SuggestionEntity()
        {
            GuildId = Context.Guild.Id,
            GuildName = Context.Guild.Name,
            MessageId = message.Id,
            MemberId = Context.User.Id,
            Suggestion = suggestion,
            Type = type.ToString(),
            Status = ""
        });
        await Database.SaveChangesAsync();
    }

    [ComponentInteraction("suggest-accept")]
    [RequireUserPermission(GuildPermission.Administrator)]
    public async Task SuggestAccept()
    {
        var interaction = Context.Interaction as SocketMessageComponent;
        var suggestion = Database.Suggestions!.FirstOrDefault(x => x.MessageId == interaction!.Message.Id);
        if (suggestion is null)
        {
            await RespondAsync("Suggestion does not exist in the database", ephemeral: true);
            return;
        }

        await interaction!.UpdateAsync(x =>
        {
            var suggestion = interaction.Message.Embeds.FirstOrDefault()!.Fields[0].Value;
            var type = interaction.Message.Embeds.FirstOrDefault()!.Fields[1].Value;
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

        if (interaction.HasResponded)
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
        var interaction = Context.Interaction as SocketMessageComponent;
        var suggestion = Database.Suggestions!.FirstOrDefault(x => x.MessageId == interaction!.Message.Id);
        if (suggestion is null)
        {
            await RespondAsync("Suggestion does not exist in the database", ephemeral: true);
            return;
        }

        await interaction!.UpdateAsync(x =>
        {
            var suggestion = interaction.Message.Embeds.FirstOrDefault()!.Fields[0].Value;
            var type = interaction.Message.Embeds.FirstOrDefault()!.Fields[1].Value;
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

        if (interaction.HasResponded)
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
        var interaction = Context.Interaction as SocketMessageComponent;
        var suggestion = Database.Suggestions!.FirstOrDefault(x => x.MessageId == interaction!.Message.Id);
        if (suggestion is null)
        {
            await RespondAsync("Suggestion does not exist in the database", ephemeral: true);
            return;
        }

        await interaction!.UpdateAsync(x =>
        {
            string suggestion = interaction.Message.Embeds.FirstOrDefault()!.Fields[0].Value;
            string type = interaction.Message.Embeds.FirstOrDefault()!.Fields[1].Value;
            string status = interaction.Message.Embeds.FirstOrDefault()!.Fields[2].Value;
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
        Database.Suggestions!.Remove(suggestion);
        await Database.SaveChangesAsync();
        if (interaction.HasResponded)
        {
            await FollowupAsync("Suggestion Finalized", ephemeral: true);
            return;
        }
        await RespondAsync("Suggestion Finalized", ephemeral: true);
    }
}