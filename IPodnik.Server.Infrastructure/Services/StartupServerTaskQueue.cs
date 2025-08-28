using IPodnik.Infrastructure.Core.Lifestyles;
using IPodnik.Infrastructure.Core.Logging;
using IPodnik.Server.Infrastructure.Operations;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;


namespace IPodnik.Server.Infrastructure.Services
{
    public class StartupServerTaskQueue : BackgroundService
    {
        private readonly Factory<IAgentTaskOperation> agentTaskOperationFactory;

        public StartupServerTaskQueue(Factory<IAgentTaskOperation> agentTaskOperationFactory)
        {
            this.agentTaskOperationFactory = agentTaskOperationFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var taskOperation = agentTaskOperationFactory.Create();

            try
            {
                await taskOperation.EnqueueAllActiveTasks();
            }
            catch (Exception ex)
            {
                BaseLogger.LogError("Task sync failed", "Task sync on startup failed", DateTime.UtcNow);
            }
        }
    }
}
