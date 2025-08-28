using IPodnik.Infrastructure.DTO.Server;
using Serilog;
using System.Text.Json;

namespace IPodnik.Agent.Infrastructure.Modules.LogModule
{
    public class LogStrategy : IJobStrategy
    {
        public LogStrategy()
        {
        }

        public ModuleReportDto Execute(AgentJobDto job)
        {
            Log.Error(JsonToLog(job.Params));
            return new ModuleReportDto() {Success = true};
        }

        private string JsonToLog(string json)
        {
            var log = JsonSerializer.Deserialize<LogJson>(json);
            return log.Message;
        }
    }

    internal class LogJson
    {
        public string Message { get; set; }
    }
}
