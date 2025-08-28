using IPodnik.Infrastructure.Core.Lifestyles;
using IPodnik.Infrastructure.DTO.Enums;
using IPodnik.Infrastructure.DTO.Server;
using System.Collections.Concurrent;

namespace IPodnik.Server.Infrastructure.Operations
{
    public interface IServerTaskQueue : ISingletonHelper
    {
        bool Add(List<AgentTaskDto> entries);
        AgentTaskDto GetById(int id);
        List<AgentTaskDto> GetDueTasks(string machineName);
        bool RemoveTask(int agentTaskId);
        void SynchronizeQueue(AgentTaskDto agentTaskOrder);
        bool UpdateOrRemoveTask(AgentTaskUpdateDto agentTaskOrder);
        List<AgentTaskDto> GetModifiedTasks();
    }

    public class ServerTaskQueue : IServerTaskQueue
    {
        private readonly ConcurrentDictionary<int, AgentTaskDto> taskQueue;

        public ServerTaskQueue()
        {
            taskQueue = new ConcurrentDictionary<int, AgentTaskDto>();
        }

        public bool Add(List<AgentTaskDto> entries)
        {
            if (entries == null || entries.Count == 0)
            {
                return false;
            }

            foreach (var item in entries)
            {
                if (item == null)
                    continue;

                item.IsModified = true;
                taskQueue.TryAdd(item.AgentTaskId, item);
            }
            return true;
        }

        public AgentTaskDto GetById(int id)
        {
            return taskQueue.TryGetValue(id, out var value) ? value : null;
        }

        public List<AgentTaskDto> GetDueTasks(string machineName)
        {
            if (string.IsNullOrWhiteSpace(machineName))
            {
                return [];
            }

            var now = DateTime.UtcNow;
            var fiveMinutesFromNow = now.AddMinutes(5);

            var result = taskQueue.Values
                .Where(t => t.StartTime <= fiveMinutesFromNow)
                .Where(t => string.Equals(t.MachineName, machineName, StringComparison.OrdinalIgnoreCase))
                .Where(t => t.TaskStatus == TaskStatusEnum.Pending || t.TaskStatus == TaskStatusEnum.Queued)
                .Where(t => !t.Sent)
                .ToList();

            foreach (var task in result)
            {
                task.Sent = true;
            }

            return result;
        }

        public bool RemoveTask(int agentTaskId)
        {
            return taskQueue.TryRemove(agentTaskId, out _);
        }

        public void SynchronizeQueue(AgentTaskDto agentTaskOrder)
        {
            if (agentTaskOrder == null)
                return;

            taskQueue.AddOrUpdate(
                agentTaskOrder.AgentTaskId,
                agentTaskOrder,
                (_, _) => agentTaskOrder
            );
        }

        public bool UpdateOrRemoveTask(AgentTaskUpdateDto agentTaskOrder)
        {
            if (agentTaskOrder == null)
            {
                return false;
            }

            taskQueue.TryGetValue(agentTaskOrder.AgentTaskId, out var existingTask);

            if (agentTaskOrder.NewStatus == TaskStatusEnum.Completed)
            {
                return RemoveTask(agentTaskOrder.AgentTaskId);
            }

            var updatedTask = ApplyUpdate(existingTask, agentTaskOrder);

            return taskQueue.TryUpdate(agentTaskOrder.AgentTaskId, updatedTask, existingTask);
        }

        private AgentTaskDto ApplyUpdate(AgentTaskDto existing, AgentTaskUpdateDto update)
        {
            if (existing == null || update == null)
            {
                return existing;
            }

            if (update.NewStatus != TaskStatusEnum.NoChange)
            {
                existing.TaskStatus = update.NewStatus;
            }

            existing.EndTime = update.EndTime;
            existing.Comment = update.Comment;
            existing.Updated = update.Updated;
            existing.IsModified = true;
            existing.Progress = update.Progress;
            existing.Result = update.StatusMessage; //TODO Make log for statusMessage and resolve Result x StatusMessage

            return existing;
        }


        public List<AgentTaskDto> GetModifiedTasks()
        {
            return taskQueue.Values
                .Where(t => t.IsModified)
                .ToList();
        }
    }
}
