using System.Data;
using Discord;
using Discord.Interactions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Plunger.APIs.Popcat;
using Plunger.APIs.Popcat.Enums;
using Plunger.Commons;
using Plunger.Data;
using Plunger.Utilities;

namespace Plunger.Modules.Utilities;

public class SlashUtilities : PlungerInteractionModuleBase
{
    private readonly IPopcatClient _popcatClient;
    public SlashUtilities(
        IConfiguration configuration,
        IHostEnvironment hostEnvironment,
        IHttpClientFactory httpClientFactory,
        ILogger<SlashUtilities> logger,
        PlungerDbContext database,
        IPopcatClient popcatClient) : base(configuration, hostEnvironment, httpClientFactory, logger, database)
    {
        _popcatClient = popcatClient;
    }

    [SlashCommand("math", "Calculates the given arguments")]
    public async Task Math(string input)
    {
        var dt = new DataTable();
        try
        {
            var result = dt.Compute(input, null);
            var Embed = new EmbedBuilder()
                .WithColor(Color.Green)
                .AddField("Question", $"```{input}```")
                .AddField("Answer", $"```{result}```")
                .WithCurrentTimestamp()
                .Build();

            await RespondAsync(embed: Embed);
        }
        catch (Exception)
        {
            var error = new PlungerEmbedBuilder()
           .WithTitle("An Error Occurred")
           .WithDescription($"**Reason:** Input cannot be parsed")
           .WithStyle(EmbedStyle.Error)
           .Build();

            await RespondAsync(embed: error, ephemeral: true); ;
        }
    }

    [SlashCommand("binary", "encodes or decodes input")]
    public async Task Binary(BinaryOption option, string input)
    {
        await DeferAsync();
        string? response;
        switch (option)
        {
            case BinaryOption.Encode:
                var encode = await _popcatClient.Encode(input);
                response = encode.Binary;
                break;
            default:
                var decode = await _popcatClient.Decode(input);
                response = decode.Text;
                break;
        }
        var Embed = new EmbedBuilder()
            .WithColor(Color.Green)
            .WithTitle("**Binary Encoder**")
            .AddField("Message", $"```{input}```")
            .AddField("Binary", $"```{response}```")
            .WithCurrentTimestamp()
            .Build();
        await FollowupAsync(embed: Embed, ephemeral: true);
    }

    [SlashCommand("password-generator", "Generates a password (Default Length: 8)")]
    public async Task PasswordGenerator(
        [Summary("Lowercase", "Include lowercase characters in the password (Default: true)")] bool lowercase = true,
        [Summary("Uppercase", "Include uppercase characters in the password (Default: true)")] bool uppercase = true,
        [Summary("Numerics", "Include numbers in the password (Default: true)")] bool numerics = true,
        [Summary("Special", "Include Special characters in the password (Default: false)")] bool special = false,
        [Summary("Length", "The length of the password (Default: 8)")] int length = 8)
    {
        Password pwd = new(lowercase, uppercase, numerics, special, length);
        string password = pwd.Next();

        var Embed = new EmbedBuilder()
                .WithColor(Color.Green)
                .WithTitle("**Password Generator**")
                .AddField("Password", $"```{password}```")
                .AddField("Length", $"```{length}```")
                .WithCurrentTimestamp()
                .Build();
        await RespondAsync(embed: Embed, ephemeral: true);
    }
}
