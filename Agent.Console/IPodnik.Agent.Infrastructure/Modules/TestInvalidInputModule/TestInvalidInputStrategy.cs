using IPodnik.Agent.Infrastructure.TaskProcessor;
using IPodnik.Infrastructure.DTO.Server;

namespace IPodnik.Agent.Infrastructure.Modules.TestInvalidInput
{
    public class TestInvalidInputStrategy : IJobStrategy
    {
        private readonly TestInvalidInputInstaller installer;

        public TestInvalidInputStrategy(IAgentTaskProgressQueue statusQueue) : base() 
        {
            this.installer = new TestInvalidInputInstaller(statusQueue);
        }

        public ModuleReportDto Execute(AgentJobDto job)
        {
            return installer.Run(job);
        }
    }
}