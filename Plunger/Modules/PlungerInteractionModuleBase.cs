using Discord.Interactions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Plunger.Database;

namespace Plunger.Modules;

public class PlungerInteractionModuleBase : InteractionModuleBase<SocketInteractionContext>
{
    public readonly IConfiguration Configuration;
    public readonly IHostEnvironment HostEnvironment;
    public readonly IHttpClientFactory HttpClientFactory;
    public readonly ILogger<PlungerInteractionModuleBase> Logger;
    public readonly PlungerDatabase Database;

    // public readonly IOptions<PlungerDatabaseConfig> PlungerDatabaseConfig;

    // private readonly IServiceScope scope;
    // private readonly IServiceProvider? serviceProvider;

    public PlungerInteractionModuleBase(
        IConfiguration configuration,
        IHostEnvironment hostEnvironment,
        IHttpClientFactory httpClientFactory,
        ILogger<PlungerInteractionModuleBase> logger,
        PlungerDatabase database)
    {
        Configuration = configuration;
        HostEnvironment = hostEnvironment;
        HttpClientFactory = httpClientFactory;
        Logger = logger;
        Database = database;
    }

    // public PlungerInteractionModuleBase(
    //     IConfiguration configuration,
    //     IHostEnvironment hostEnvironment,
    //     IHttpClientFactory httpClientFactory,
    //     ILogger<PlungerInteractionModuleBase> logger,
    //     PlungerDatabase database,
    //     IOptions<PlungerDatabaseConfig> plungerDatabaseConfig)
    // {
    //     Configuration = configuration;
    //     HostEnvironment = hostEnvironment;
    //     HttpClientFactory = httpClientFactory;
    //     Logger = logger;
    //     Database = database;
    //     PlungerDatabaseConfig = plungerDatabaseConfig;
    // }

    public string SuggestionCollection => Configuration["PlungerDatabase:SuggestionCollection"];
    public string GuildCollection => Configuration["PlungerDatabase:GuildCollection"];
    public string LockdownCollection => Configuration["PlungerDatabase:LockdownCollection"];
}