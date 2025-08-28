using IPodnik.Agent.Infrastructure.TaskProcessor;
using IPodnik.Infrastructure.DTO.Server;

namespace IPodnik.Agent.Infrastructure.Modules.TestInvalidInput
{
    public class TestInvalidInputInstaller : BaseModule
    {
        public TestInvalidInputInstaller(IAgentTaskProgressQueue progressQueue) : base(progressQueue)
        {
        }

        public ModuleReportDto Run(AgentJobDto job)
        {
            LogProgress(job, "Validating input...", 10);

            if (job.Params != "test")
            {
                LogProgress(job, "Invalid input detected.", 20);
                return new ModuleReportDto
                {
                    Success = false,
                    Error = "Invalid input"
                };
            }

            LogProgress(job, "Input OK", 100);
            return new ModuleReportDto { Success = true };
        }
    }
}
