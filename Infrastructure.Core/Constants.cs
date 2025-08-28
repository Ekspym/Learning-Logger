namespace Infrastructure.Core;

public static class Constants
{
    public static TimeSpan DefaultTimeToLive = TimeSpan.FromMinutes(5);
}

public static class Endpoints
{
    public static class MediatorServer
    {
        public static string TaskUpdate = $"api/task/UpdateAgentTask";
        public static string TaskHartBeat = $"api/task/GetAgentTasks";
        public static string TaskPing = $"api/task/GetAgentTasksPing";
    }

    public static class AgentMediator
    {
        public static string TaskUpdate = $"api/task/PostAgentTaskStatus";
        public static string TaskHartBeat = $"api/task/GetAgentTasks";
        public static string TaskPing = $"api/task/GetAgentTasksPing";
    }

    public static class LogService
    {
        public static string InsertLog = $"api/log/InsertToLog";
    }
}