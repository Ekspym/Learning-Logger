using IPodnik.Infrastructure.DTO.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPodnik.Server.Infrastructure.Helpers
{
    internal class TaskStatusUpdateRules
    {
        private static readonly Dictionary<(TaskStatusEnum Original, TaskStatusEnum New), bool> allowedTransitions = new()
        {
            { (TaskStatusEnum.Queued, TaskStatusEnum.NoChange), true },
            { (TaskStatusEnum.Pending, TaskStatusEnum.NoChange), true },
            { (TaskStatusEnum.InProgress, TaskStatusEnum.NoChange), true },
            { (TaskStatusEnum.Completed, TaskStatusEnum.NoChange), false },
            { (TaskStatusEnum.Canceled, TaskStatusEnum.NoChange), true },
            { (TaskStatusEnum.Failed, TaskStatusEnum.NoChange), true },
            { (TaskStatusEnum.Paused, TaskStatusEnum.NoChange), true },

            { (TaskStatusEnum.Queued, TaskStatusEnum.Pending), true },
            { (TaskStatusEnum.Queued, TaskStatusEnum.InProgress), true },
            { (TaskStatusEnum.Queued, TaskStatusEnum.Canceled), true },
            { (TaskStatusEnum.Queued, TaskStatusEnum.Completed), false },
            { (TaskStatusEnum.Queued, TaskStatusEnum.Paused), true },
            { (TaskStatusEnum.Queued, TaskStatusEnum.Failed), true },


            { (TaskStatusEnum.Pending, TaskStatusEnum.InProgress), true },
            { (TaskStatusEnum.Pending, TaskStatusEnum.Canceled), true },
            { (TaskStatusEnum.Pending, TaskStatusEnum.Completed), true },
            { (TaskStatusEnum.Pending, TaskStatusEnum.Failed), true },

            { (TaskStatusEnum.InProgress, TaskStatusEnum.Completed), true },
            { (TaskStatusEnum.InProgress, TaskStatusEnum.Canceled), true },
            { (TaskStatusEnum.InProgress, TaskStatusEnum.Queued), false },
            { (TaskStatusEnum.InProgress, TaskStatusEnum.Paused), true },
            { (TaskStatusEnum.InProgress, TaskStatusEnum.Failed), true },


            { (TaskStatusEnum.Completed, TaskStatusEnum.Queued), false },
            { (TaskStatusEnum.Completed, TaskStatusEnum.Pending), false },
            { (TaskStatusEnum.Completed, TaskStatusEnum.InProgress), false },
            { (TaskStatusEnum.Completed, TaskStatusEnum.Canceled), false },
            { (TaskStatusEnum.Completed, TaskStatusEnum.Paused), false },

            { (TaskStatusEnum.Canceled, TaskStatusEnum.Queued), false },
            { (TaskStatusEnum.Canceled, TaskStatusEnum.Pending), false },
            { (TaskStatusEnum.Canceled, TaskStatusEnum.InProgress), false },
            { (TaskStatusEnum.Canceled, TaskStatusEnum.Completed), false },

            { (TaskStatusEnum.Failed, TaskStatusEnum.Queued), false },
            { (TaskStatusEnum.Failed, TaskStatusEnum.Pending), false },
            { (TaskStatusEnum.Failed, TaskStatusEnum.InProgress), false },
            { (TaskStatusEnum.Failed, TaskStatusEnum.Completed), true },

            { (TaskStatusEnum.Paused, TaskStatusEnum.Queued), true },
            { (TaskStatusEnum.Paused, TaskStatusEnum.Canceled), true },
            { (TaskStatusEnum.Paused, TaskStatusEnum.InProgress), true },
            { (TaskStatusEnum.Paused, TaskStatusEnum.Completed), true },
        };

        public static bool IsUpdateAllowed(TaskStatusEnum original, TaskStatusEnum newStatus)
        {
            return allowedTransitions.TryGetValue((original, newStatus), out var allowed) && allowed;
        }
    }
}
