using IPodnik.Agent.Infrastructure.TaskProcessor;
using IPodnik.Infrastructure.DTO.Server;

namespace IPodnik.Agent.Infrastructure.Modules.TestHang
{
    public class TestHangInstaller : BaseModule
    {
        public TestHangInstaller(IAgentTaskProgressQueue progressQueue) : base(progressQueue)
        {
        }

        public ModuleReportDto Run(AgentJobDto job)
        {
            LogProgress(job, "Started and working...", 30);

            Task.Delay(10000);

            LogProgress(job, "Mid-task...", 60);

            Task.Delay(Timeout.Infinite);

            return new ModuleReportDto
            {
                Success = false,
                Error = "This should never be reached"
            };
        }
    }
}
