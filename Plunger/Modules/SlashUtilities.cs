using System.Data;
using Discord;
using Discord.Interactions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Plunger.APIs.Interfaces;
using Plunger.Commons;
using Plunger.Data;
using Plunger.Utilities;

namespace Plunger.Modules;

public class SlashUtilities : PlungerInteractionModuleBase
{
    private readonly IPopcat _popcat;
    public SlashUtilities(
        IConfiguration configuration,
        IHostEnvironment hostEnvironment,
        IHttpClientFactory httpClientFactory,
        ILogger<SlashUtilities> logger,
        PlungerDbContext database,
        IPopcat popcat) : base(configuration, hostEnvironment, httpClientFactory, logger, database)
    {
        _popcat = popcat;
    }

    [SlashCommand("math", "Calculates the given arguments")]
    public async Task Math(string args)
    {
        var dt = new DataTable();
        try
        {
            var result = dt.Compute(args, null);
            var Embed = new EmbedBuilder()
                .WithColor(Color.Green)
                .AddField("Question", $"```{args}```")
                .AddField("Answer", $"```{result}```")
                .WithCurrentTimestamp()
                .Build();

            await RespondAsync(embed: Embed);
        }
        catch (Exception)
        {
            var error = new PlungerEmbedBuilder()
           .WithTitle("An Error Occurred")
           .WithDescription($"**Reason:** Input is not a number")
           .WithStyle(EmbedStyle.Error)
           .Build();

            await RespondAsync(embed: error, ephemeral: true); ;
        }
    }

    [Group("binary", "Binary utility")]
    public class Binary : PlungerInteractionModuleBase
    {
        private readonly IPopcat _popcat;
        public Binary(
            IConfiguration configuration,
            IHostEnvironment hostEnvironment,
            IHttpClientFactory httpClientFactory,
            ILogger<Binary> logger,
            PlungerDbContext database,
            IPopcat popcat) : base(configuration, hostEnvironment, httpClientFactory, logger, database)
        {
            _popcat = popcat;
        }

        [SlashCommand("encode", "Encode the message you provided in Binary")]
        public async Task Encode(string text)
        {
            var binary = _popcat.BinaryEncode(text).Result.Binary;
            if (binary is not null)
            {
                var Embed = new EmbedBuilder()
                        .WithColor(Color.Green)
                        .WithTitle("**Binary Encoder**")
                        .AddField("Text", $"```{text}```")
                        .AddField("Binary", $"```{binary}```")
                        .WithCurrentTimestamp()
                        .Build();
                await RespondAsync(embed: Embed);
                return;
            }
            await RespondAsync("```⛔ An Error Occurred, Please Try again Later```");
        }

        [SlashCommand("decode", "The binary you want to decode")]
        public async Task Decode(string binary)
        {
            var text = _popcat.BinaryDecode(binary).Result.Text;
            if (text is not null)
            {
                var Embed = new EmbedBuilder()
                        .WithColor(Color.Green)
                        .WithTitle("**Binary Decoder**")
                        .AddField("Binary", $"```{binary}```")
                        .AddField("Text", $"```{text}```")
                        .WithCurrentTimestamp()
                        .Build();
                await RespondAsync(embed: Embed);
                return;
            }
            await RespondAsync("```⛔ An Error Occurred, Please Try again Later```");
        }
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
