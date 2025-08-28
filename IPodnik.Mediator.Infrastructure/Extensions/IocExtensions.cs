using IPodnik.Infrastructure.Core.Extensions;
using IPodnik.Infrastructure.Core.Logging;
using IPodnik.Infrastructure.DTO.Configurations;
using IPodnik.MessageBus.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IPodnik.Mediator.Infrastructure.Extensions;

public static class IocExtensions
{
    public static void InstallMediatorService(this IServiceCollection services, IConfiguration configuration)
    {
        services.InstallSender(configuration);
        services.InstallQueueWriter();
        
        services.AddOptions<ApiUrls>().BindConfiguration(nameof(ApiUrls));
        
        services.ScanBaseInterfaces<RabbitQueueOperation>();
        
        services.InstallWebMachineInitializer();
    }
}
