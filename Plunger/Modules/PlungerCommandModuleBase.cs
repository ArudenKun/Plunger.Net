using Discord.Commands;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Plunger.Data;

namespace Plunger.Modules;

public class PlungerCommandModuleBase : ModuleBase<SocketCommandContext>
{
    private protected readonly IConfiguration Configuration;
    private protected readonly IHostEnvironment HostEnvironment;
    private protected readonly IHttpClientFactory HttpClientFactory;
    private protected readonly ILogger<PlungerCommandModuleBase> Logger;
    private protected readonly PlungerDbContext Database;

    public PlungerCommandModuleBase(
        IConfiguration configuration,
        IHostEnvironment hostEnvironment,
        IHttpClientFactory httpClientFactory,
        ILogger<PlungerCommandModuleBase> logger,
        PlungerDbContext database)
    {
        Configuration = configuration;
        HostEnvironment = hostEnvironment;
        HttpClientFactory = httpClientFactory;
        Logger = logger;
        Database = database;
    }
}
