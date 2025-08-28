using IPodnik.Infrastructure.DTO.Server;


namespace IPodnik.Agent.Infrastructure.Modules
{
    public interface IJobStrategy
    {
        public ModuleReportDto Execute(AgentJobDto job);
    }
}
