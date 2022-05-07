using Discord.Interactions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Plunger.Data;

namespace Plunger.Modules;

public class PlungerInteractionModuleBase : InteractionModuleBase<SocketInteractionContext>
{
    private protected readonly IConfiguration Configuration;
    private protected readonly IHostEnvironment HostEnvironment;
    private protected readonly IHttpClientFactory HttpClientFactory;
    private protected readonly ILogger<PlungerInteractionModuleBase> Logger;
    private protected readonly PlungerDbContext Database;

    public PlungerInteractionModuleBase(
        IConfiguration configuration,
        IHostEnvironment hostEnvironment,
        IHttpClientFactory httpClientFactory,
        ILogger<PlungerInteractionModuleBase> logger,
        PlungerDbContext database)
    {
        Configuration = configuration;
        HostEnvironment = hostEnvironment;
        HttpClientFactory = httpClientFactory;
        Logger = logger;
        Database = database;
    }
}