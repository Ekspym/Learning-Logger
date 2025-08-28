using LogService.Infrastructure.Contexts;
using LogService.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LogService.Infrastructure.Extensions;

/// <summary>
/// Provides extension methods to register logging-related services, repositories, 
/// and hosted services in the DI container.
/// </summary>
public static class IocExtensions
{
    public static void InstallLogContext(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddDbContext<LoggerContext>(options => options.UseSqlServer(configuration.GetConnectionString("DatabaseConnection")));   
    }

    public static void InstallLoggerService(this IServiceCollection services)
    {
        services.AddHostedService<LoggerService>();
    }
}