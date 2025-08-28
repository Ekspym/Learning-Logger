using IPodnik.Agent.Infrastructure.TaskProcessor;
using IPodnik.Infrastructure.DTO.Server;

namespace IPodnik.Agent.Infrastructure.Modules.TestHang
{
    public class TestHangStrategy : IJobStrategy
    {
        private readonly TestHangInstaller installer;

        public TestHangStrategy(IAgentTaskProgressQueue statusQueue) : base() 
        {
           this.installer = new TestHangInstaller(statusQueue);
        }

        public ModuleReportDto Execute(AgentJobDto job)
        {
            return installer.Run(job);
        }
    }
}