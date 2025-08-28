using IPodnik.Agent.Infrastructure.TaskProcessor;
using Microsoft.Extensions.Hosting;


namespace IPodnik.Agent.Infrastructure.Services
{
    public class AgentApp : BackgroundService
    {
        private readonly ITaskProcessor taskProcessor;

        public AgentApp(ITaskProcessor taskProcessor)
        {
            this.taskProcessor = taskProcessor;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            taskProcessor.Start();
            return Task.CompletedTask;
        }
    }
}

