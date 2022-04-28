using Discord.Commands;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Plunger.Database;

namespace Plunger.Modules;

public class PlungerCommandModuleBase : ModuleBase<SocketCommandContext>
{
    public readonly IConfiguration Configuration;
    public readonly IHostEnvironment HostEnvironment;
    public readonly IHttpClientFactory HttpClientFactory;
    public readonly ILogger<PlungerCommandModuleBase> Logger;
    public readonly PlungerDatabase Database;

    public PlungerCommandModuleBase(
        IConfiguration configuration,
        IHostEnvironment hostEnvironment,
        IHttpClientFactory httpClientFactory,
        ILogger<PlungerCommandModuleBase> logger,
        PlungerDatabase database)
    {
        Configuration = configuration;
        HostEnvironment = hostEnvironment;
        HttpClientFactory = httpClientFactory;
        Logger = logger;
        Database = database;
    }
}
