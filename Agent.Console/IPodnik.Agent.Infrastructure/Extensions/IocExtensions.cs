using IPodnik.Agent.Infrastructure.Helpers;
using IPodnik.Agent.Infrastructure.Services;
using IPodnik.Infrastructure.Core.Extensions;
using IPodnik.Infrastructure.Core.Logging;
using IPodnik.MessageBus.Core;
using Microsoft.Extensions.DependencyInjection;

namespace IPodnik.Agent.Infrastructure.Extensions
{
    public static class IocExtensions
    {
        public static void InstallAgentService(this IServiceCollection services)
        {
            services.InstallConfiguration();
            
            services.InstallServices();
            services.ScanBaseInterfaces<HeartbeatService>();
            
            services.InstallWebMachineInitializer();
        }

        public static void InstallConfiguration(this IServiceCollection services)
        {
            services.AddOptions<AgentConfiguration>().BindConfiguration(nameof(AgentConfiguration));
        }
        
        public static void InstallServices(this IServiceCollection services)
        {
            services.AddHostedService<HeartbeatService>();
            services.AddHostedService<AgentApp>();
            services.AddHostedService<AgentTaskStatusService>();
        }

        public static void InstallMessageBus(this IServiceCollection services)
        {
            services.AddScoped<IQueueWriter>();
        }
    }
}
