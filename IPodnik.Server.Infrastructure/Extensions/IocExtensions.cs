using IPodnik.Infrastructure.Core.Extensions;
using IPodnik.Infrastructure.Core.Logging;
using IPodnik.Infrastructure.UnitOfWork;
using IPodnik.MessageBus.Core;
using IPodnik.Server.Infrastructure.Mappers;
using IPodnik.Server.Infrastructure.Models;
using IPodnik.Server.Infrastructure.Repository;
using IPodnik.Server.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IPodnik.Server.Infrastructure.Extensions
{
    public static class IocExtensions
    {
        public static void InstallAgentServer(this IServiceCollection services, ConfigurationManager configuration)
        {
            services.InstallLazy();
            services.InstallAgentMonitor();
            services.InstallGaiaDbContext(configuration);
            
            services.InstallConsumer<AgentTaskMessageHandler>(configuration,"taskQueue");
            services.InstallTaskQueue();
            services.InstallQueueWriter();
            
            services.ScanBaseInterfaces<AgentMonitorService>();
            
            services.InstallWebMachineInitializer();
            
            MapperConfiguration.Configure();
        }
        
        private static void InstallGaiaDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<IPodnikGaiaContext>(options => options.UseSqlServer(configuration.GetConnectionString("DatabaseConnection")));
            services.AddScoped<IUnitOfWork, GaiaUnitOfWork>(); // Register Unit of Work

            services.Scan(scan => scan
                .FromAssemblyOf<GaiaUnitOfWork>()
                .AddClasses(classes => classes.AssignableTo<IRepository>())
                .AsImplementedInterfaces()
                .WithTransientLifetime()
            );
        }

        private static void InstallAgentMonitor(this IServiceCollection services)
        {
            services.AddHostedService<AgentMonitorService>();
        }

        private static void InstallTaskQueue(this IServiceCollection services)
        {
            services.AddHostedService<StartupServerTaskQueue>();
        }
    }
}
