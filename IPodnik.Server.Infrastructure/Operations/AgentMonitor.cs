using IPodnik.Infrastructure.Core.Lifestyles;
using IPodnik.Infrastructure.DTO.Server;
using System.Collections.Concurrent;

namespace IPodnik.Server.Infrastructure.Operations
{
    public interface IAgentMonitor : ISingletonHelper
    {
        void RecordPing(string machineName);
        IEnumerable<string> GetInactiveAgents();
        void Cleanup();
        Dictionary<string, bool> GetAgentStatuses();
        bool IsAgentActive(string machineName);
        Task AddMachines(List<MachineInfoDto> machines);
    }

    public class AgentMonitor : IAgentMonitor
    {
        private readonly ConcurrentDictionary<string, DateTime> agentPings = new();
        private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(Constants.AgentMonitor.AgentTimeoutMilliseconds);

        public async Task AddMachines(List<MachineInfoDto> machines)
        {
            var now = DateTime.UtcNow;
            foreach (var machine in machines)
            {
                agentPings[machine.Name] = now;
            }
        }

        public void RecordPing(string machineName)
        {
            agentPings[machineName] = DateTime.UtcNow;
        }

        public IEnumerable<string> GetInactiveAgents()
        {
            var threshold = DateTime.UtcNow - Timeout;
            return agentPings.Where(pair => pair.Value < threshold).Select(pair => pair.Key);
        }

        public void Cleanup()
        {
            var threshold = DateTime.UtcNow - Timeout;
            foreach (var machine in GetInactiveAgents())
            {
                agentPings.TryRemove(machine, out _);
            }
        }

        public Dictionary<string, bool> GetAgentStatuses()
        {
            var now = DateTime.UtcNow;
            return agentPings.ToDictionary(pair => pair.Key, pair => now - pair.Value <= Timeout);
        }

        public bool IsAgentActive(string machineName)
        {
            return agentPings.TryGetValue(machineName, out var lastPing) && DateTime.UtcNow - lastPing <= Timeout;
        }
    }
}
