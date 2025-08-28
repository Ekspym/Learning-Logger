using IPodnik.Agent.Infrastructure.TaskProcessor;
using IPodnik.Infrastructure.DTO.Server;
using Mapster;


namespace IPodnik.Agent.Infrastructure.Modules
{
    public abstract class BaseModule
    {
        protected IAgentTaskProgressQueue progressQueue;

        protected BaseModule(IAgentTaskProgressQueue progressQueue)
        {
            this.progressQueue = progressQueue;
        }

        protected void LogProgress(AgentJobDto job, string message, int? progress = null)
        {
            if (progress.HasValue)
            {
                job.Progress = progress.Value;
            }

            job.StatusMessage = message;
            progressQueue.Add(job.Adapt<ProgressUpdateDto>());
        }
    }
}
