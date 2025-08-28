using System;
using System.Collections.Generic;

namespace IPodnik.Server.Infrastructure.Models
{
    public partial class AgentTask
    {
        public int AgentTaskId { get; set; }
        public int TaskTypeId { get; set; }
        public int? TaskStatusId { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string Result { get; set; }
        public DateTime? Updated { get; set; }
        public string Comment { get; set; }
        public int MachineId { get; set; }
        public int UserProfileId { get; set; }
        public int Progress { get; set; }

        public virtual Machine Machine { get; set; }
        public virtual TaskStatus TaskStatus { get; set; }
        public virtual TaskType TaskType { get; set; }
        public virtual UserProfile UserProfile { get; set; }
    }
}
