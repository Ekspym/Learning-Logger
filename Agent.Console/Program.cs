using IPodnik.Agent.Infrastructure.Extensions;
using IPodnik.Infrastructure.Core.Logging;
using Microsoft.Extensions.Hosting;

var host = CreateDefaultApp(args).Build();
host.Services.ConfigureMachineInfo();

await host.RunAsync();

IHostBuilder CreateDefaultApp(string[] args)
{
    var builder = Host.CreateDefaultBuilder(args);

    builder.ConfigureServices((services) =>
    {
        services.InstallConfiguration();
        services.InstallServices();
        services.InstallMessageBus();
        services.InstallServiceMachineInitializer();
    });

    return builder;
}
