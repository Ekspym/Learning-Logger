using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Core.Logging;

public static class LoggingExtensions
{
    public static IApplicationBuilder ConfigureMachineInfo(this IApplicationBuilder app, bool withLogging = true)
    {
        ConfigureMachineInfoCore(app.ApplicationServices, withLogging);
        return app;
    }
    
    public static void ConfigureMachineInfo(this IServiceProvider serviceProvider, bool withLogging = true)
    {
        ConfigureMachineInfoCore(serviceProvider, withLogging);
    }
    
    public static void InstallWebMachineInitializer(this IServiceCollection services)
    {
        services.AddSingleton<IMachineInitializer, WebMachineInitializer>();
    }
    
    public static void InstallServiceMachineInitializer(this IServiceCollection services)
    {
        services.AddSingleton<IMachineInitializer, ServiceMachineInitializer>();
    }
    
    private static void ConfigureMachineInfoCore(IServiceProvider serviceProvider, bool withLogging)
    {
        using var scope = serviceProvider.CreateScope();
        var initializer = scope.ServiceProvider.GetRequiredService<IMachineInitializer>();
        var loggerUrl = withLogging ? scope.ServiceProvider.GetRequiredService<IConfiguration>().GetSection("ApiUrls:BaseLogger").Value : null;
            
        initializer.InitializeLogging(loggerUrl);
    }
}