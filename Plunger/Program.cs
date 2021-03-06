using Discord;
using Discord.Addons.Hosting;
using Discord.WebSocket;
using Fergun.Interactive;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Plunger.APIs.Nekobot;
using Plunger.APIs.Popcat;
using Plunger.APIs.Reddit;
using Plunger.APIs.Waifu.Im;
using Plunger.APIs.Waifu.Pics;
using Plunger.Data;
using Plunger.Services;
using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Verbose()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
    .Enrich.FromLogContext()
    // .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}", theme: AnsiConsoleTheme.Literate)
    .WriteTo.Console()
    .CreateLogger();

try
{
    Log.Information("Starting Bot...");
    var host = Host.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration(x =>
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true)
                .Build();
            x.AddConfiguration(configuration);
        })
        .ConfigureDiscordHost((context, config) =>
        {
            config.SocketConfig = new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Info,
                AlwaysDownloadUsers = true,
                MessageCacheSize = 200,
                GatewayIntents = GatewayIntents.All,
                LogGatewayIntentWarnings = false
            };
            config.Token = context.Configuration["Token"];
            config.LogFormat = (message, exception) => $"{message.Source}: {message.Message}";
        })
        .UseCommandService((context, config) =>
        {
            config.LogLevel = LogSeverity.Info;
            config.DefaultRunMode = Discord.Commands.RunMode.Async;
            config.CaseSensitiveCommands = false;
            config.IgnoreExtraArgs = true;
            config.SeparatorChar = ' ';
        })
        .UseInteractionService((context, config) =>
        {
            config.LogLevel = LogSeverity.Verbose;
            config.DefaultRunMode = Discord.Interactions.RunMode.Async;
            config.UseCompiledLambda = true;
        })
        .ConfigureServices((context, services) =>
        {
            services
                .AddHostedService<HandlerService>()
                .AddHostedService<EventsService>()
                .AddHostedService<EventListenerService>()
                .AddHttpClient()
                .AddSingleton<InteractiveService>()
                .AddSingleton(new InteractiveConfig { DefaultTimeout = TimeSpan.FromMinutes(1) })
                .AddDbContext<PlungerDbContext>(_ =>
                    _.UseSqlite(context.Configuration.GetConnectionString("Default")));


            // * APIs
            services
                .AddScoped<IPopcatClient, PopcatClient>()
                .AddScoped<INekobotClient, NekobotClient>()
                .AddScoped<IWaifuPicsClient, WaifuPicsClient>()
                .AddScoped<IWaifuImClient, WaifuImClient>()
                .AddScoped<IRedditClient, RedditClient>();

        })
        .UseSerilog()
        .UseConsoleLifetime()
        .Build();
    await host.RunAsync();
    return 0;
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
    return 1;
}
finally
{
    Log.CloseAndFlush();
}