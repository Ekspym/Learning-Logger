using IPodnik.Infrastructure.Bridge;
using IPodnik.Infrastructure.Core.Lifestyles;
using IPodnik.Infrastructure.DTO.Server;


namespace IPodnik.Mediator.Infrastructure.Operations
{
    public interface IServerCommunicationOperation : IScopedOperation
    {
        Task<List<AgentTaskDto>> GetAgentTask(AgentTaskFilterDto filter);
        Task<List<AgentTaskDto>> GetAgentTaskByMachineName(HeartbeatInfoDto data);
    }
    public class ServerCommunicationOperation : IServerCommunicationOperation
    {
        private readonly IMediatorServerBridge agentServerBridge;

        public ServerCommunicationOperation(IMediatorServerBridge agentServerBridge)
        {
            this.agentServerBridge = agentServerBridge;
        }

        public async Task<List<AgentTaskDto>> GetAgentTask(AgentTaskFilterDto filter)
        {
            List<AgentTaskDto> tasks = await agentServerBridge.GetTasksAsync(filter);
            return tasks;
        }

        public async Task<List<AgentTaskDto>> GetAgentTaskByMachineName(HeartbeatInfoDto data)
        {
            List<AgentTaskDto> tasks = await agentServerBridge.GetTasksByMachineNameAsync(data);
            return tasks;
        }
    }
}
