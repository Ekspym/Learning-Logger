using Microsoft.Extensions.Hosting;

namespace Infrastructure.Core.Logging
{
    public class ServiceMachineInitializer : BaseMachineInitializer
    {
        public ServiceMachineInitializer(IHostEnvironment environment)
        {
            var instanceId = $"{AppDomain.CurrentDomain.FriendlyName}-{environment.EnvironmentName}";
            MachineInfo.InitInfo(Environment.MachineName, instanceId);
        }
    }
}
