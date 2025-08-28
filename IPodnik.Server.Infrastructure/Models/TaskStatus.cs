using System;
using System.Collections.Generic;

namespace IPodnik.Server.Infrastructure.Models
{
    public partial class TaskStatus
    {
        public TaskStatus()
        {
            AgentTasks = new HashSet<AgentTask>();
        }

        public int TaskStatusId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual ICollection<AgentTask> AgentTasks { get; set; }
    }
}
