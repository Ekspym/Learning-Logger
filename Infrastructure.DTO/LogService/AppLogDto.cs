using Infrastructure.DTO.Enums;

namespace Infrastructure.DTO.LogService
{
    public class AppLogDto
    {
        public string Title { get; set; }
        public string VirtualMachineName { get; set; }
        public LogTypeEnum LogType { get; set; }
        public string Message { get; set; }
        public string ApplicationName { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
