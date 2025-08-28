using IPodnik.Infrastructure.Bridge;
using IPodnik.Infrastructure.Core.Lifestyles;
using IPodnik.Infrastructure.Core.Logging;
using IPodnik.Infrastructure.DTO.Enums;
using IPodnik.Infrastructure.DTO.MessageBus;
using IPodnik.Infrastructure.DTO.Server;

namespace IPodnik.Agent.Infrastructure.TaskProcessor
{
    public interface IAgentTaskQueueHelper : IScopedOperation
    {
        bool AddToList(AgentTaskDto task);
        AgentTaskDto GetTaskToDo();
        bool CancelQueuedTask(int taskId);
        void SyncQueue(AgentTaskDto task);
        bool UpdateTask(AgentTaskMessageDto agentTaskUpdate);
        bool CompleteTask(int taskId);
        bool FailedQueuedTask(int taskId);
        Task<bool> ProcessStatusQueue(AgentTaskMessageDto agentTaskUpdate);
    }

    public class AgentTaskQueueHelper : IAgentTaskQueueHelper
    {
        private readonly IAgentTaskQueue agentTaskQueue;
        private readonly IAgentMediatorBridge agentMediatorBridge;
        private readonly IAgentTaskStatusQueue agentTaskStatusQueue;

        public AgentTaskQueueHelper(IAgentTaskQueue agentTaskQueue, IAgentMediatorBridge agentMediatorBridge, IAgentTaskStatusQueue agentTaskStatusQueue)
        {
            this.agentTaskQueue = agentTaskQueue;
            this.agentMediatorBridge = agentMediatorBridge;
            this.agentTaskStatusQueue = agentTaskStatusQueue;
        }

        public bool AddToList(AgentTaskDto task)
        {
            var success = agentTaskQueue.Add(task);

            if (success)
            {
                UpdateTask(new AgentTaskMessageDto()
                {
                    AgentTaskId = task.AgentTaskId,
                    Updated = DateTime.UtcNow,
                    Status = TaskStatusEnum.Queued
                });
            }
            return success;
        }

        public AgentTaskDto GetTaskToDo()
        {
            var task = agentTaskQueue.GetNewest();

            if (task != null && task.StartTime <= DateTime.UtcNow)
            {
                UpdateTask(new AgentTaskMessageDto
                {
                    AgentTaskId = task.AgentTaskId,
                    Updated = DateTime.UtcNow,
                    Status = TaskStatusEnum.InProgress
                });
                return task;
            }
            return null;
        }

        public bool CancelQueuedTask(int taskId)
        {
            var task = agentTaskQueue.GetById(taskId);

            if (task == null || task.TaskStatus != TaskStatusEnum.Queued)
            {
                return false;
            }

            return UpdateTask(new AgentTaskMessageDto()
            {
                AgentTaskId = task.AgentTaskId,
                Updated = DateTime.UtcNow,
                Status = TaskStatusEnum.Canceled
            });
        }

        public bool FailedQueuedTask(int taskId)
        {
            var task = agentTaskQueue.GetById(taskId);

            if (task == null)
            {
                return false;
            }

            return UpdateTask(new AgentTaskMessageDto()
            {
                AgentTaskId = task.AgentTaskId,
                Updated = DateTime.UtcNow,
                Status = TaskStatusEnum.Failed
            });
        }

        public bool UpdateTask(AgentTaskMessageDto agentTaskUpdate)
        {
            return agentTaskStatusQueue.Add(agentTaskUpdate);
        }

        public async Task<bool> ProcessStatusQueue(AgentTaskMessageDto agentTaskUpdate)
        {
            var success = await agentMediatorBridge.SendTaskUpdateAsync(agentTaskUpdate);
            if (!success)
            {
                agentTaskStatusQueue.Add(agentTaskUpdate);
                BaseLogger.LogError("AgentTaskQueueHelper", $"Failed to update Task {agentTaskUpdate.AgentTaskId} to {agentTaskUpdate.Status}", DateTime.UtcNow);
                return false;
            }

            return true;
        }

        public void SyncQueue(AgentTaskDto task)
        {
            agentTaskQueue.SynchronizeQueue(task);
        }

        public bool CompleteTask(int taskId)
        {
            var task = agentTaskQueue.GetById(taskId);
            var result = agentTaskQueue.Remove(taskId);

            if (result)
            {
                return UpdateTask(new AgentTaskMessageDto()
                {
                    AgentTaskId = task.AgentTaskId,
                    Updated = DateTime.UtcNow,
                    Status = TaskStatusEnum.Completed
                });
            }
            return false;
        }

        public bool RemoveTask(int taskId)
        {
            return agentTaskQueue.Remove(taskId);
        }
    }
}