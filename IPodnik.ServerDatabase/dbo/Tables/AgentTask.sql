CREATE TABLE [dbo].[AgentTask] (
    [AgentTaskId]   INT            IDENTITY (1, 1) NOT NULL,
    [TaskTypeId]    INT            NOT NULL,
    [TaskStatusId]  INT            NULL,
    [CreateDate]    DATETIME       CONSTRAINT [DF__JobStatus__CreateDate] DEFAULT (getdate()) NULL,
    [StartTime]     DATETIME       NOT NULL,
    [EndTime]       DATETIME       NULL,
    [Result]        NVARCHAR (50)  NULL,
    [Updated]       DATETIME       CONSTRAINT [DF__JobStatus__UpdatedDate] DEFAULT (getdate()) NULL,
    [Comment]       NVARCHAR (255) NULL,
    [MachineId]     INT            NOT NULL,
    [UserProfileId] INT            NOT NULL,
    CONSTRAINT [UIX_AgentTask_AgentTaskId] PRIMARY KEY CLUSTERED ([AgentTaskId] ASC),
    CONSTRAINT [FK__AgentTask__MachineId] FOREIGN KEY ([MachineId]) REFERENCES [dbo].[Machine] ([MachineId]),
    CONSTRAINT [FK__AgentTask__TaskTypeId] FOREIGN KEY ([TaskTypeId]) REFERENCES [dbo].[TaskType] ([TaskTypeId]),
    CONSTRAINT [FK__AgentTask__UserProfileId] FOREIGN KEY ([UserProfileId]) REFERENCES [dbo].[UserProfile] ([UserProfileId]),
    CONSTRAINT [FK_AgentTask_TaskStatusId] FOREIGN KEY ([TaskStatusId]) REFERENCES [dbo].[TaskStatus] ([TaskStatusId])
);












GO



GO



GO



GO
CREATE NONCLUSTERED INDEX [IX_AgentTask_UserProfileId]
    ON [dbo].[AgentTask]([UserProfileId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_AgentTask_TaskTypeId]
    ON [dbo].[AgentTask]([TaskTypeId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_AgentTask_TaskStatusId]
    ON [dbo].[AgentTask]([TaskStatusId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_AgentTask_MachineId]
    ON [dbo].[AgentTask]([MachineId] ASC);

