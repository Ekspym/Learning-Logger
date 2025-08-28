using Microsoft.Extensions.Hosting;
using IPodnik.Agent.Infrastructure.TaskProcessor;
using IPodnik.Infrastructure.DTO.MessageBus;
using IPodnik.Infrastructure.Core.Lifestyles;

namespace IPodnik.Agent.Infrastructure.Services
{

    public class AgentTaskStatusService : BackgroundService
    {
        private readonly Factory<IAgentTaskQueueHelper> taskQueueHelperFactory;
        private readonly IAgentTaskStatusQueue agentTaskStatusQueue;
        private readonly TimeSpan interval = TimeSpan.FromSeconds(1);

        public AgentTaskStatusService(Factory<IAgentTaskQueueHelper> taskQueueHelperFactory, IAgentTaskStatusQueue agentTaskStatusQueue)
        {
            this.taskQueueHelperFactory = taskQueueHelperFactory;
            this.agentTaskStatusQueue = agentTaskStatusQueue;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await UpdateAgentTaskStatus();
                await Task.Delay(interval, stoppingToken);
            }
        }

        private async Task UpdateAgentTaskStatus()
        {
            var taskQueue = taskQueueHelperFactory.Create();

            AgentTaskMessageDto item;
            while ((item = agentTaskStatusQueue.Dequeue()) != null)
            {
                 await taskQueue.ProcessStatusQueue(item);
            }

        }
    }

}
