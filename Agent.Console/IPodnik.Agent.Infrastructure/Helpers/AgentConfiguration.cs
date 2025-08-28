namespace IPodnik.Agent.Infrastructure.Helpers;

public class AgentConfiguration
{
    public string AgentMediatorUrl { get; set; } 
    public int HeartbeatPingInterval { get; set; }
    public int TaskProcessorCheckInterval { get; set; }
}
