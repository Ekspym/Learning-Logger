using Microsoft.AspNetCore.Hosting;

namespace Infrastructure.Core.Logging
{
    public class WebMachineInitializer : BaseMachineInitializer
    {
        public WebMachineInitializer(IWebHostEnvironment environment)
        {
            var instanceId = $"{environment.ApplicationName}-{environment.EnvironmentName}";
            MachineInfo.InitInfo(Environment.MachineName, instanceId);
        }
    }
}
