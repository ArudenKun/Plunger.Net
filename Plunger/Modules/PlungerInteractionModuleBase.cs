using Discord.Interactions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Plunger.Data;

namespace Plunger.Modules;

public class PlungerInteractionModuleBase : InteractionModuleBase<SocketInteractionContext>
{
    public readonly IConfiguration Configuration;
    public readonly IHostEnvironment HostEnvironment;
    public readonly IHttpClientFactory HttpClientFactory;
    public readonly ILogger<PlungerInteractionModuleBase> Logger;
    public readonly PlungerDbContext Database;

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