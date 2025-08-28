using System.Collections.Concurrent;
using IPodnik.Infrastructure.Core.Lifestyles;
using IPodnik.Infrastructure.DTO.Server;

namespace IPodnik.Agent.Infrastructure.TaskProcessor
{
    public interface IAgentTaskQueue : ISingletonHelper
    {
        bool Add(AgentTaskDto agentTask);
        AgentTaskDto GetById(int id);
        AgentTaskDto GetNewest();
        AgentTaskDto DequeueById(int id);
        bool Update(AgentTaskDto agentTask);
        void SynchronizeQueue(AgentTaskDto agentTask);
        bool Remove(int agentTaskId);
    }

    public class AgentTaskQueue : IAgentTaskQueue
    {
        private ConcurrentDictionary<int, AgentTaskDto> agentTaskOrderQueue;


        public AgentTaskQueue()
        {
            agentTaskOrderQueue = new ConcurrentDictionary<int, AgentTaskDto>();

        }

        public bool Add(AgentTaskDto agentTask)
        {
            ValidateAgentTask(agentTask);
            return agentTaskOrderQueue.TryAdd(agentTask.AgentTaskId, agentTask);
        }

        public AgentTaskDto GetNewest()
        {
            var newest = agentTaskOrderQueue.Values.OrderBy(a => a.StartTime).FirstOrDefault();
            return newest != null ? GetById(newest.AgentTaskId) : null;
        }

        public AgentTaskDto GetById(int id)
        {
            return agentTaskOrderQueue.TryGetValue(id, out var value) ? value : null;
        }

        public AgentTaskDto DequeueById(int id)
        {
            return agentTaskOrderQueue.TryRemove(id, out var value) ? value : null;
        }

        public bool Update(AgentTaskDto agentTask)
        {
            ValidateAgentTask(agentTask);
            return agentTaskOrderQueue.TryGetValue(agentTask.AgentTaskId, out var existingagentTask) && agentTaskOrderQueue.TryUpdate(agentTask.AgentTaskId, agentTask, existingagentTask);
        }

        private void ValidateAgentTask(AgentTaskDto agentTask)
        {
            if (agentTask == null)
            {
                throw new ArgumentNullException(nameof(agentTask));
            }
        }

        public void SynchronizeQueue(AgentTaskDto agentTask)
        {
            if (agentTask == null)
            {
                throw new ArgumentNullException(nameof(agentTask));
            }

            agentTaskOrderQueue.AddOrUpdate(
                agentTask.AgentTaskId,
                agentTask,
                (key, existingTask) =>
                {
                    return agentTask;
                });
        }

        public bool Remove(int agentTaskId)
        {
            return agentTaskOrderQueue.TryRemove(agentTaskId, out _);
        }
    }
}
