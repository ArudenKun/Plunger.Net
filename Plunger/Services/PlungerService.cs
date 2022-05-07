using Discord.Addons.Hosting;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Plunger.Data;

namespace Plunger.Services
{
    public abstract class PlungerService : DiscordClientService
    {
        private protected readonly IConfiguration Configuration;
        private protected readonly IHostEnvironment Environment;
        private protected readonly IServiceProvider ServiceProvider;
        private protected readonly CommandService CommandService;
        private protected readonly InteractionService InteractionService;
        private protected readonly PlungerDbContext Database;

        public PlungerService(
            DiscordSocketClient client,
            ILogger<PlungerService> logger,
            IConfiguration configuration,
            IHostEnvironment environment,
            IServiceProvider serviceProvider,
            CommandService commandService,
            InteractionService interactionService,
            PlungerDbContext database)
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