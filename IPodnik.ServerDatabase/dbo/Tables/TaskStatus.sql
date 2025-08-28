CREATE TABLE [dbo].[TaskStatus] (
    [TaskStatusId] INT            IDENTITY (1, 1) NOT NULL,
    [Name]         NVARCHAR (50)  NOT NULL,
    [Description]  NVARCHAR (255) NULL,
    CONSTRAINT [UIX_TaskStatus_TaskStatusId] PRIMARY KEY CLUSTERED ([TaskStatusId] ASC)
);





