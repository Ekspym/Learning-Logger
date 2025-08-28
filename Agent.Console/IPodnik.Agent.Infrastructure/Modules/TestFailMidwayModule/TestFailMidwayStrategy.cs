using IPodnik.Agent.Infrastructure.TaskProcessor;
using IPodnik.Infrastructure.DTO.Server;

namespace IPodnik.Agent.Infrastructure.Modules.TestFailMidway
{
    public class TestFailMidwayStrategy : IJobStrategy
    {
        private readonly TestFailMidwayInstaller installer;

        public TestFailMidwayStrategy(IAgentTaskProgressQueue statusQueue) : base()
        {
            this.installer = new TestFailMidwayInstaller(statusQueue);
        }

        public ModuleReportDto Execute(AgentJobDto job)
        {
            return installer.Run(job);
        }
    }
}