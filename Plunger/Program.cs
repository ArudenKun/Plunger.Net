using Discord;
using Discord.Addons.Hosting;
using Discord.WebSocket;
using Fergun.Interactive;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Plunger.Database;
using Plunger.Services;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
    .MinimumLevel.Override("System", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}", theme: AnsiConsoleTheme.Literate)
    .CreateLogger();

try
{
    Log.Information("Starting Bot");
    var host = Host.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration(x =>
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true)
                .AddUserSecrets<Program>()
                .AddEnvironmentVariables()
                .Build();
            x.AddConfiguration(configuration);
        })
        .UseSerilog()
        .UseConsoleLifetime()
        .ConfigureDiscordHost((context, config) =>
        {
            config.SocketConfig = new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Info,
                AlwaysDownloadUsers = true,
                MessageCacheSize = 200,
                GatewayIntents = GatewayIntents.All
            };

            config.Token = context.Configuration["Token"];
            // config.Token = "ODQ0ODg1NTc5MTYyMDU4Nzgy.YKY7Aw.1z0QMVzYzUwg5_WG6UdyrukRaxI";
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
            config.LogLevel = LogSeverity.Info;
            config.DefaultRunMode = Discord.Interactions.RunMode.Async;
            config.UseCompiledLambda = true;
        })
        .ConfigureServices((context, services) =>
        {
            services
                .AddHostedService<HandlerService>()
                .AddSingleton(new PlungerDatabase(
                    context.Configuration["PlungerDatabase:DatabaseName"],
                    context.Configuration["PlungerDatabase:ConnectionString"]))
                .AddSingleton(new InteractiveConfig { DefaultTimeout = TimeSpan.FromMinutes(1) })
                .AddSingleton<InteractiveService>()
                .AddHttpClient();

            services
                .Configure<PlungerDatabaseConfig>(context.Configuration.GetSection("PlungerDatabase"));
            // services.AddHttpClient("Popcat", x => x.BaseAddress = new Uri(context.Configuration.GetSection("API")["Popcat"]));
            // services.AddHttpClient("WaifuPics", x => x.BaseAddress = new Uri(context.Configuration.GetSection("API")["WaifuPics"]));

            // services.AddLavaNode(x =>
            // {
            //     x.Hostname = "lavalink.eu";
            //     x.Authorization = "Raccoon";
            //     x.Port = 2333;

            //     x.SelfDeaf = false;
            //     x.LogSeverity = LogSeverity.Info;
            //     x.BufferSize = 5000;
            // });
        })
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