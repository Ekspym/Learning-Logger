using Infrastructure.DTO.Enums;

namespace Infrastructure.DTO.LogService
{
    public class LogInfoDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string VirtualMachineName { get; set; }
        public LogTypeEnum LogType { get; set; }
        public string Message { get; set; }
        public string ApplicationName { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
