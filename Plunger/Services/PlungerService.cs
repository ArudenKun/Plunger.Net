using Discord.Addons.Hosting;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Plunger.Database;

namespace Plunger.Services
{
    public abstract class PlungerService : DiscordClientService
    {
        public readonly IConfiguration Configuration;
        public readonly IHostEnvironment Environment;
        public readonly IServiceProvider ServiceProvider;
        public readonly CommandService CommandService;
        public readonly InteractionService InteractionService;
        public readonly PlungerDatabase Database;
        // public readonly DataAccessLayer DataAcessLayer;

        public PlungerService(
            DiscordSocketClient client,
            ILogger<DiscordClientService> logger,
            IConfiguration configuration,
            IHostEnvironment environment,
            IServiceProvider serviceProvider,
            CommandService commandService,
            InteractionService interactionService,
            PlungerDatabase database)
           : base(client, logger)
        {
            Configuration = configuration;
            Environment = environment;
            ServiceProvider = serviceProvider;
            CommandService = commandService;
            InteractionService = interactionService;
            Database = database;
        }
    }
}