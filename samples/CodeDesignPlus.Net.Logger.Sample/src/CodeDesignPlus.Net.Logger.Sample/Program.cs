// See https://aka.ms/new-console-template for more information


using CodeDesignPlus.Net.Logger.Extensions;
using CodeDesignPlus.Net.Logger.Sample;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        config.AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true);
    })
    .ConfigureServices((context, services) =>
    {
        services.AddLogging();
        services.AddLogger(context.Configuration);

        services.AddHostedService<FakeBackgroundService>();
    })
    .UseSerilog();

var host = builder.Build();

host.Run();
