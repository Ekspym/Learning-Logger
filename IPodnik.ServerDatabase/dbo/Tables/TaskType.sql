CREATE TABLE [dbo].[TaskType] (
    [TaskTypeId]  INT            IDENTITY (1, 1) NOT NULL,
    [Name]        NVARCHAR (255) NOT NULL,
    [Description] NVARCHAR (500) NULL,
    CONSTRAINT [UIX_TaskType_TaskTypeId] PRIMARY KEY CLUSTERED ([TaskTypeId] ASC)
);





