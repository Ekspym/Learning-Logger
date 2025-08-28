using IPodnik.Agent.Infrastructure.Helpers;
using IPodnik.Agent.Infrastructure.TaskProcessor;
using IPodnik.Infrastructure.Bridge;
using IPodnik.Infrastructure.Core.Lifestyles;
using IPodnik.Infrastructure.Core.Logging;
using IPodnik.Infrastructure.DTO.Enums;
using IPodnik.Infrastructure.DTO.Server;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace IPodnik.Agent.Infrastructure.Services;

public class HeartbeatService : BackgroundService
{
    private readonly Factory<IAgentTaskQueueHelper> taskQueueHelperFactory;
    private readonly IAgentMediatorBridge agentMediatorBridge;
    private readonly IAgentTaskProgressQueue taskProgressQueue;
    private readonly IOptions<AgentConfiguration> configuration;

    public HeartbeatService(Factory<IAgentTaskQueueHelper> taskQueueHelperFactory, IOptions<AgentConfiguration> configuration, IAgentMediatorBridge agentMediatorBridge, IAgentTaskProgressQueue taskProgressQueue)
    {
        this.taskQueueHelperFactory = taskQueueHelperFactory;
        this.configuration = configuration;
        this.taskProgressQueue = taskProgressQueue;
        this.agentMediatorBridge = agentMediatorBridge;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await InitQueue();

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await PerformHeartbeatAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                BaseLogger.LogError($"{nameof(HeartbeatService)} Error", ex.Message, DateTime.UtcNow);
            }
            finally
            {
                await Task.Delay(configuration.Value.HeartbeatPingInterval, stoppingToken);
            }
        }
    }

    private async Task PerformHeartbeatAsync(CancellationToken cancellationToken)
    {
        try
        {
            var taskQueue = taskQueueHelperFactory.Create();
            var taskOrders = await GetTasksAsync();

            foreach (var taskOrder in taskOrders)
            {
                if (!taskQueue.AddToList(taskOrder)) 
                {
                    //TODO Logger? Provolani chyby na frontend/podporu ? Opakovnej pokus ? 
                }
            }

        }
        catch (Exception ex)
        {
            BaseLogger.LogError("Error Performing HeartbeatService", ex.Message, DateTime.UtcNow);
        }
    }

    private async Task<List<AgentTaskDto>> GetTasksAsync()
    {
        try
        {
            return await agentMediatorBridge.GetTasksByMachineNameAsync(taskProgressQueue.GetInfo());
        }
        catch (Exception ex)
        {
            BaseLogger.LogError("Error Retrieving Tasks", ex.Message, DateTime.UtcNow);
            return [];
        }
    }

    private async Task InitQueue()
    {
        var taskQueue = taskQueueHelperFactory.Create();

        var filter = new AgentTaskFilterDto { MachineName = MachineInfo.Name, TaskStatus = TaskStatusEnum.Queued };
        var taskOrders = await GetTasksAsync();
        if (taskOrders?.Any() == true)
        {
            foreach (var taskOrder in taskOrders)
            {
                taskQueue.SyncQueue(taskOrder);
            }
        }
    }

}
