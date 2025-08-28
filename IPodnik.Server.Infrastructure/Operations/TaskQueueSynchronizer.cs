using IPodnik.Infrastructure.Core.Lifestyles;
using IPodnik.Infrastructure.DTO.Enums;
using IPodnik.Infrastructure.DTO.Server;
using IPodnik.Server.Infrastructure.Helpers;
using Mapster;
using Nito.AsyncEx;
using System.Collections.Concurrent;


namespace IPodnik.Server.Infrastructure.Operations
{
    public interface ITaskQueueSynchronizer : IScopedOperation
    {
        List<AgentTaskDto> GetTask(string MachineName);
        Task SynchronizeDatabaseAsync();
        Task AddTask(AgentTaskOrderDto task);
        Task UpdateTask(AgentTaskUpdateDto agentTaskUpdate);
        AgentTaskDto GetTaskById(int agentTaskId);
        Task<bool> TryUpdateTask(AgentTaskUpdateDto agentTaskUpdate);
        Task<bool> TryUpdateTaskProgress(HeartbeatInfoDto heartbeatInfos);
    }
    public class TaskQueueSynchronizer : ITaskQueueSynchronizer
    {
        private static readonly ConcurrentDictionary<int, AsyncLock> taskLocks = new();
        private readonly IServerTaskQueue taskQueue;
        private readonly IAgentTaskOperation agentTaskOperation;
        private readonly IMachineOperation machineOperation;

        public TaskQueueSynchronizer(IServerTaskQueue taskQueue, IAgentTaskOperation agentTaskOperation, IMachineOperation machineOperation)
        {
            this.taskQueue = taskQueue;
            this.agentTaskOperation = agentTaskOperation;
            this.machineOperation = machineOperation;
        }

        public List<AgentTaskDto> GetTask(string machineName)
        {
            var tasks = taskQueue.GetDueTasks(machineName);
            return tasks.Adapt<List<AgentTaskDto>>();
        }

        public AgentTaskDto GetTaskById(int agentTaskId)
        {
            var task = taskQueue.GetById(agentTaskId);
            return task.Adapt<AgentTaskDto>();
        }

        public async Task SynchronizeDatabaseAsync()
        {
            var modifiedTasks = taskQueue.GetModifiedTasks();

            if (!modifiedTasks.Any())
                return;

            await agentTaskOperation.BulkUpdateTasks(modifiedTasks.Adapt<List<AgentTaskUpdateDto>>());
        }

        public async Task AddTask(AgentTaskOrderDto task)
        {
            var agentTasks = (await agentTaskOperation.AddTaskAsync(task)).Adapt<List<AgentTaskDto>>();

            foreach (var agentTask in agentTasks)
            {
                var machine = await machineOperation.GetByIdsAsync(agentTask.MachineId);
                if (machine != null)
                {
                    agentTask.MachineName = machine.Name;
                }
            }

            taskQueue.Add(agentTasks);
        }

        public async Task UpdateTask(AgentTaskUpdateDto agentTaskUpdate)
        {
            var updatedInMemory = taskQueue.UpdateOrRemoveTask(agentTaskUpdate);

            if (updatedInMemory)
            {
                if (agentTaskUpdate.NewStatus == TaskStatusEnum.Completed)
                {
                    agentTaskUpdate.Progress = 100;
                }
                await agentTaskOperation.UpdateTask(agentTaskUpdate);
            }
        }

        public async Task<bool> TryUpdateTask(AgentTaskUpdateDto agentTaskUpdate)
        {
            var taskLock = taskLocks.GetOrAdd(agentTaskUpdate.AgentTaskId, _ => new AsyncLock());

            using (await taskLock.LockAsync())
            {
                var task = taskQueue.GetById(agentTaskUpdate.AgentTaskId);
                if (task == null)
                {
                    return false;
                }

                if (TaskStatusUpdateRules.IsUpdateAllowed(task.TaskStatus, agentTaskUpdate.NewStatus))
                {

                    await UpdateTask(agentTaskUpdate);

                    if (task.TaskStatus == TaskStatusEnum.Completed)
                    {
                        taskLocks.TryRemove(agentTaskUpdate.AgentTaskId, out _);
                    }

                    return true;
                }
            }
            return false;
        }


        public async Task<bool> TryUpdateTaskProgress(HeartbeatInfoDto heartbeatInfos)
        {
            foreach (var info in heartbeatInfos.ProgressUpdates)
            {
                await TryUpdateTask(info.Adapt<AgentTaskUpdateDto>());
            }
            return true;
        }
    }
}
