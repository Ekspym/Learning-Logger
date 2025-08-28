using IPodnik.Agent.Infrastructure.Helpers;
using IPodnik.Agent.Infrastructure.Modules;
using IPodnik.Agent.Infrastructure.Operations;
using IPodnik.Infrastructure.Core.Lifestyles;
using IPodnik.Infrastructure.Core.Logging;
using IPodnik.Infrastructure.DTO.Server;
using Microsoft.Extensions.Options;

namespace IPodnik.Agent.Infrastructure.TaskProcessor
{
    public interface ITaskProcessor : ISingletonHelper
    {
        void Start();
        void Stop();
    }

    public class TaskProcessor : ITaskProcessor
    {
        private readonly IOptions<AgentConfiguration> configuration;
        private readonly Factory<IAgentTaskQueueHelper> taskQueueHelperFactory;
        private readonly IStrategyProvider strategyProvider;
        private CancellationTokenSource cancellationTokenSource;

        public TaskProcessor(IStrategyProvider strategyProvider, Factory<IAgentTaskQueueHelper> taskQueueHelperFactory, IOptions<AgentConfiguration> configuration)
        {
            this.configuration = configuration;
            this.strategyProvider = strategyProvider;
            this.taskQueueHelperFactory = taskQueueHelperFactory;
        }

        public void Start()
        {
            if (cancellationTokenSource != null)
            {
                BaseLogger.LogError("TaskProcessor Start Error", "TaskProcessor is already running.", DateTime.UtcNow);
                throw new InvalidOperationException("TaskProcessor is already running.");
            }

            cancellationTokenSource = new CancellationTokenSource();
            BaseLogger.LogInformation("TaskProcessor Started", "TaskProcessor has started.", DateTime.UtcNow);
            Task.Run(() => ProcessQueueAsync(cancellationTokenSource.Token));
        }

        public void Stop()
        {
            if (cancellationTokenSource == null)
            {
                BaseLogger.LogError("TaskProcessor Stop Error", "TaskProcessor is not running.", DateTime.UtcNow);
                throw new InvalidOperationException("TaskProcessor is not running.");
            }

            cancellationTokenSource.Cancel();
            cancellationTokenSource = null;

            BaseLogger.LogInformation("TaskProcessor Stopped", "TaskProcessor has stopped.", DateTime.UtcNow);
        }

        private async Task ProcessQueueAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                AgentTaskDto task = null;

                try
                {
                    var taskQueue = taskQueueHelperFactory.Create();
                    task =  taskQueue.GetTaskToDo();
                }
                catch (Exception ex)
                {
                    BaseLogger.LogError("Queue Retrieval Error", ex.Message, DateTime.UtcNow);
                }

                if (task != null)
                {
                    ExecuteTask(task);
                }

                await Task.Delay(configuration.Value.TaskProcessorCheckInterval, cancellationToken);
            }

            BaseLogger.LogInformation("Queue Processing Stopped", "Processing queue has stopped.", DateTime.UtcNow);
        }

        private void ExecuteTask(AgentTaskDto task)
        {
            var taskQueue = taskQueueHelperFactory.Create();
            try
            {

                var strategy = strategyProvider.GetStrategy(task.TaskType);

                var job = new AgentJobDto()
                {
                    AgentTaskId = task.AgentTaskId,
                    TaskType = task.TaskType,
                    MachineId = task.MachineId,
                    StartTime = task.StartTime,
                    MachineName = task.MachineName,
                    UserProfileId = task.UserProfileId,
                    TaskStatus = task.TaskStatus,
                    Params = String.Empty
                };

                var report = strategy.Execute(job);

                bool success;
                
                if (report.Success)
                {
                    success = taskQueue.CompleteTask(task.AgentTaskId);
                }
                else 
                {
                    success = taskQueue.FailedQueuedTask(task.AgentTaskId);
                }

                if (!success)
                {
                    BaseLogger.LogError("AgentTaskQueueHelper", $"Failed to update Task {task.AgentTaskId} to {task.TaskStatus}", DateTime.UtcNow);
                }
            }
            catch (Exception ex)
            {
                BaseLogger.LogError("Task Execution Error", $"Error executing task with ID {task.AgentTaskId}: {ex.Message}", DateTime.UtcNow);

                taskQueue.FailedQueuedTask(task.AgentTaskId);
            }
        }
    }
}
