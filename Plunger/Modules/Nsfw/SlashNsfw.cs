using Discord.Interactions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Plunger.Data;

namespace Plunger.Modules.Nsfw;

[RequireNsfw]
public class SlashNsfw : PlungerInteractionModuleBase
{
    public SlashNsfw(
        IConfiguration configuration,
        IHostEnvironment hostEnvironment,
        IHttpClientFactory httpClientFactory,
        ILogger<PlungerInteractionModuleBase> logger,
        PlungerDbContext database) : base(configuration, hostEnvironment, httpClientFactory, logger, database)
    {
    }

    
}
