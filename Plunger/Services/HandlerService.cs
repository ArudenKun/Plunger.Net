using Discord;
using Discord.Addons.Hosting;
using Discord.Addons.Hosting.Util;
using Discord.Interactions;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Plunger.Commons;
using System.Reflection;
using Plunger.TypeConverters;
using Plunger.Data;

namespace Plunger.Services;

public class HandlerService : PlungerService
{
    private readonly IHost _host;
    private string? title;
    private string? description;

    public HandlerService(
        DiscordSocketClient client,
        ILogger<HandlerService> logger,
        IConfiguration configuration,
        IHostEnvironment environment,
        IServiceProvider serviceProvider,
        CommandService commandService,
        InteractionService interactionService,
        PlungerDbContext database,
        IHost host) : base(client, logger, configuration, environment, serviceProvider, commandService, interactionService, database)
    {
        _host = host;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Process the InteractionCreated payloads to execute Interactions commands
        Client.InteractionCreated += HandleInteraction;
        Client.MessageReceived += OnMessageReceived;

        // Process the command execution results
        CommandService.CommandExecuted += CommandExecuted;
        InteractionService.SlashCommandExecuted += SlashCommandExecuted;
        InteractionService.ContextCommandExecuted += ContextCommandExecuted;
        InteractionService.ComponentCommandExecuted += ComponentCommandExecuted;

        InteractionService.AddTypeConverter<TimeSpan>(new TimeSpanConverter());

        await CommandService.AddModulesAsync(Assembly.GetEntryAssembly(), ServiceProvider);
        await InteractionService.AddModulesAsync(Assembly.GetEntryAssembly(), ServiceProvider);

        await Client.WaitForReadyAsync(stoppingToken);
        foreach (var guild in Client.Guilds)
        {
            await InteractionService.RegisterCommandsToGuildAsync(guild.Id, true);
        }
        // If DOTNET_ENVIRONMENT is set to development, only register the commands to a single guild
        // if (Environment.IsDevelopment())
        //     // await InteractionService.RegisterCommandsToGuildAsync(Configuration.GetValue<ulong>("DevGuild"));
        //     foreach (var guild in Client.Guilds)
        //     {
        //         await InteractionService.RegisterCommandsToGuildAsync(guild.Id, true);
        //     }
        // else
        //     await InteractionService.RegisterCommandsGloballyAsync();

        // try
        // {
        //     // Database = new PlungerDatabase(Configuration["PlungerDatabase:DatabaseName"], Configuration["PlungerDatabase:ConnectionString"]);
        //     isDbConnected = Database.IsConnected;
        // }
        // catch (TimeoutException e)
        // {
        //     dbException = e;
        // }
        Logger.LogInformation("Bot is ready");
    }

    //Command
    public async Task CommandExecuted(Optional<CommandInfo> command, ICommandContext context, Discord.Commands.IResult result)
    {
        Logger.LogInformation("User {user} attempted to use command {command}", context.User.Username, command.Value.Name);

        // if (!command.IsSpecified || result.IsSuccess)
        // {
        //     Logger.LogInformation($"Command {command.Value.Name} executed for -> {context.User.Username}");
        //     return;
        // }

        /*if (result.IsSuccess)
        {
            Logger.LogInformation($"Command failed to execute for {context.User.Username} <-> {command.Value.Name}!");
            return;
        }
        Logger.LogError($"Error: {result.ErrorReason}");
        await context.Channel.SendMessageAsync($"Error: {result.Error} {result.ErrorReason}");*/

        switch (result.Error)
        {
            case CommandError.BadArgCount:
                title = "Invalid use of command";
                description = "Please provide the correct amount of parameters.";
                break;

            case CommandError.MultipleMatches:
                title = "Invalid argument";
                description = "Please provide a valid argument.";
                break;

            case CommandError.ObjectNotFound:
                title = "Not found";
                description = "The argument that was provided could not be found.";
                break;

            case CommandError.ParseFailed:
                title = "Invalid argument";
                description = "The argument that you provided could not be parsed correctly.";
                break;

            case CommandError.UnmetPrecondition:
                title = "Access denied";
                description = "You or the bot does not meet the required preconditions.";
                break;

            default:
                title = "An error occurred";
                description = "An error occurred while trying to run this command.";
                break;
        }

        var error = new PlungerEmbedBuilder()
            .WithTitle(title)
            .WithDescription(description)
            .WithStyle(EmbedStyle.Error)
            .Build();

        await context.Channel.SendMessageAsync(embed: error);
    }

    private async Task OnMessageReceived(SocketMessage incomingMessage)
    {
        if (incomingMessage is not SocketUserMessage message) return;
        if (message.Source != MessageSource.User) return;

        //Logger.LogInformation($"{message.Author.Username}: {incomingMessage}");

        
        // if (!message.HasStringPrefix(Configuration["Prefix"], ref argPos) && !message.HasMentionPrefix(Client.CurrentUser, ref argPos)) return;

        // var context = new SocketCommandContext(Client, message);

        // await CommandService.ExecuteAsync(context, argPos, ServiceProvider);

        int argPos = 0;
        var user = message.Author as SocketGuildUser;
        var data = await Database.Guilds!.FindAsync(user!.Guild.Id);
        if (!message.HasStringPrefix(data!.Prefix, ref argPos) && !message.HasMentionPrefix(Client.CurrentUser, ref argPos)) return;
        var context = new SocketCommandContext(Client, message);
        await CommandService.ExecuteAsync(context, argPos, ServiceProvider);
    }


    //Interaction
    private async Task ComponentCommandExecuted(ComponentCommandInfo commandInfo, IInteractionContext context, Discord.Interactions.IResult result)
    {
        if (!result.IsSuccess)
        {
            switch (result.Error)
            {
                case InteractionCommandError.UnmetPrecondition:
                    title = "Access denied";
                    description = $"**Reason:** {result.ErrorReason}";
                    break;

                case InteractionCommandError.UnknownCommand:
                    title = "Unknown Command";
                    description = $"**Reason:** {result.ErrorReason}";
                    break;

                case InteractionCommandError.BadArgs:
                    title = "Wrong Arguments";
                    description = $"**Reason:** {result.ErrorReason}";
                    break;

                case InteractionCommandError.Exception:
                    title = "An Error Occurred";
                    description = $"**Reason:** {result.ErrorReason}";
                    break;

                case InteractionCommandError.Unsuccessful:
                    title = "An Error Occurred";
                    description = $"**Reason:** {result.ErrorReason}";
                    break;

                default:
                    title = "An Error Occurred";
                    description = $"**Reason:** {result.ErrorReason}";
                    break;
            }

            var error = new PlungerEmbedBuilder()
            .WithTitle(title)
            .WithDescription(description)
            .WithStyle(EmbedStyle.Error)
            .Build();

            await context.Interaction.RespondAsync(embed: error, ephemeral: true);
        }
    }

    private async Task ContextCommandExecuted(ContextCommandInfo ctxInfo, IInteractionContext context, Discord.Interactions.IResult result)
    {
        if (!result.IsSuccess)
        {
            switch (result.Error)
            {
                case InteractionCommandError.UnmetPrecondition:
                    title = "Access denied";
                    description = $"**Reason:** {result.ErrorReason}";
                    break;

                case InteractionCommandError.UnknownCommand:
                    title = "Unknown Command";
                    description = $"**Reason:** {result.ErrorReason}";
                    break;

                case InteractionCommandError.BadArgs:
                    title = "Wrong Arguments";
                    description = $"**Reason:** {result.ErrorReason}";
                    break;

                case InteractionCommandError.Exception:
                    title = "An Error Occurred";
                    description = $"**Reason:** {result.ErrorReason}";
                    break;

                case InteractionCommandError.Unsuccessful:
                    title = "An Error Occurred";
                    description = $"**Reason:** {result.ErrorReason}";
                    break;

                default:
                    title = "An Error Occurred";
                    description = $"**Reason:** {result.ErrorReason}";
                    break;
            }

            var error = new PlungerEmbedBuilder()
            .WithTitle(title)
            .WithDescription(description)
            .WithStyle(EmbedStyle.Error)
            .Build();

            await context.Interaction.RespondAsync(embed: error, ephemeral: true);
        }
    }

    private async Task SlashCommandExecuted(SlashCommandInfo ctxInfo, IInteractionContext context, Discord.Interactions.IResult result)
    {
        /*Logger.LogInformation("User {user} attempted to use command /{command}", context.User.Username, ctxInfo.Name);

        if (result.IsSuccess)
        {
            Logger.LogInformation($"Command /{ctxInfo.Name} executed for -> {context.User.Username}");
        }*/
        if (!result.IsSuccess)
        {
            switch (result.Error)
            {
                case InteractionCommandError.UnmetPrecondition:
                    title = "Access denied";
                    description = $"**Reason:** {result.ErrorReason}";
                    break;

                case InteractionCommandError.UnknownCommand:
                    title = "Unknown Command";
                    description = $"**Reason:** {result.ErrorReason}";
                    break;

                case InteractionCommandError.BadArgs:
                    title = "Wrong Arguments";
                    description = $"**Reason:** {result.ErrorReason}";
                    break;

                case InteractionCommandError.Exception:
                    title = "An Exception Occurred";
                    description = $"**Reason:** {result.ErrorReason}";
                    break;

                case InteractionCommandError.Unsuccessful:
                    title = "An Error Occurred";
                    description = $"**Reason:** {result.ErrorReason}";
                    break;

                default:
                    title = "An Error Occurred";
                    description = $"**Reason:** {result.ErrorReason}";
                    break;
            }

            var error = new PlungerEmbedBuilder()
            .WithTitle(title)
            .WithDescription(description)
            .WithStyle(EmbedStyle.Error)
            .Build();

            await context.Interaction.RespondAsync(embed: error, ephemeral: true);
        }
    }

    private async Task HandleInteraction(SocketInteraction interaction)
    {
        try
        {
            // Create an execution context that matches the generic type parameter of your InteractionModuleBase<T> modules
            var ctx = new SocketInteractionContext(Client, interaction);
            await InteractionService.ExecuteCommandAsync(ctx, ServiceProvider);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Exception occurred whilst attempting to handle the interaction.");

            if (interaction.Type == InteractionType.ApplicationCommand)
            {
                var msg = await interaction.GetOriginalResponseAsync();
                await msg.DeleteAsync();
            }
        }
    }
}