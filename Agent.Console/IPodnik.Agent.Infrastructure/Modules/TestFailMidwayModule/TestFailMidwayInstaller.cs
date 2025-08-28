using IPodnik.Agent.Infrastructure.TaskProcessor;
using IPodnik.Infrastructure.DTO.Server;
using Mapster;

namespace IPodnik.Agent.Infrastructure.Modules.TestFailMidway
{
    public class TestFailMidwayInstaller : BaseModule
    {
        public TestFailMidwayInstaller(IAgentTaskProgressQueue progressQueue) : base(progressQueue)
        {
        }

        public ModuleReportDto Run(AgentJobDto job)
        {
            for (int i = 1; i <= 3; i++)
            {
                Thread.Sleep(300);
                job.Progress = i * 20;
                job.StatusMessage = $"Step {i}/5 in progress...";
                progressQueue.Add(job.Adapt<ProgressUpdateDto>());
            }

            Thread.Sleep(300);
            job.Progress = 40;
            job.StatusMessage = "Step 4/5 failed.";
            progressQueue.Add(job.Adapt<ProgressUpdateDto>());

            return new ModuleReportDto
            {
                Success = false,
                Error = "Simulated failure at 40% progress"
            };
        }
    }
}
