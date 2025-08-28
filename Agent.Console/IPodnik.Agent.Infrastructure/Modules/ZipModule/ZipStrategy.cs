using IPodnik.Agent.Infrastructure.TaskProcessor;
using IPodnik.Infrastructure.DTO.Server;

namespace IPodnik.Agent.Infrastructure.Modules.ZipModule
{
    public class ZipStrategy : IJobStrategy
    {
        ZipInstaller installer;

        public ZipStrategy(IAgentTaskProgressQueue statusQueue) : base()
        {
           installer = new ZipInstaller(statusQueue);
        }

        public ModuleReportDto Execute(AgentJobDto job)
        {
            installer.TryInstallZip(job);
            return new ModuleReportDto { Success = true };
        }
    }
}
