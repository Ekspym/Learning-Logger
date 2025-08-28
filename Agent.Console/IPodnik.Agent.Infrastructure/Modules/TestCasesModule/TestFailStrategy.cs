using IPodnik.Agent.Infrastructure.TaskProcessor;
using IPodnik.Infrastructure.DTO.Server;


namespace IPodnik.Agent.Infrastructure.Modules.TestCasesModule
{
    public class TestFailStrategy : IJobStrategy
    {
        private readonly DummyWordpadLogic installer;

        public TestFailStrategy(IAgentTaskProgressQueue statusQueue) : base()
        {
            this.installer = new DummyWordpadLogic(statusQueue, 500, failAtWord: 400);
        }

        public ModuleReportDto Execute(AgentJobDto job)
        {
            try
            {
                installer.TryExecute(job);

                return new ModuleReportDto
                {
                    Success = true,
                    Error = "Unexpected: task completed without failure."
                };
            }
            catch (Exception ex)
            {
                return new ModuleReportDto
                {
                    Success = false,
                    Error = $"Dummy logic failed: {ex.Message}"
                };
            }
        }
    }
}
