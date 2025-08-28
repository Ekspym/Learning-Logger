using System;
using System.Collections.Generic;

namespace LogService.Infrastructure.Contexts
{
    public partial class AppLog
    {
        public int AppLogId { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string ApplicationName { get; set; }
        public DateTime CreateDate { get; set; }
        public int LogTypeId { get; set; }
        public string VirtualMachineName { get; set; }
        public string SessionId { get; set; }
    }
}
