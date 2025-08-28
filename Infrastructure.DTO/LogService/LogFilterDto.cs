using Infrastructure.DTO.Enums;

namespace Infrastructure.DTO.LogService
{
    public class LogFilterDto
    {
        public int? FromId { get; set; }
        public int? ToId { get; set; }
        public List<LogTypeEnum> LogType { get; set; }
        public string Message { get; set; }
        public List<string> ApplicationName { get; set; }
        public List<string> VirtualMachineName { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }  
        public SortTypeEnum SortType { get; set; }
        public bool IsAscending { get; set; }
        public int LogCount { get; set;}
        public int Page { get; set; }
    }
}
