CREATE TABLE [dbo].[AppLog] (
    [AppLogID]           INT           IDENTITY (1, 1) NOT NULL,
    [Title]              VARCHAR (50)  NULL,
    [Message]            VARCHAR (255) NULL,
    [ApplicationName]    VARCHAR (255) NULL,
    [CreateDate]         DATETIME      NOT NULL,
    [LogTypeID]          INT           NOT NULL,
    [VirtualMachineName] VARCHAR (50)  NULL,
    [SessionID]          VARCHAR (60)  NULL,
    CONSTRAINT [PK_AppLog] PRIMARY KEY CLUSTERED ([AppLogID] ASC),
    CONSTRAINT [FK_AppLog_LogType] FOREIGN KEY ([LogTypeID]) REFERENCES [dbo].[LogType] ([LogTypeID])
);












GO
CREATE NONCLUSTERED INDEX [VirtualMachineName_idx]
    ON [dbo].[AppLog]([VirtualMachineName] ASC);


GO
CREATE NONCLUSTERED INDEX [LogTypeID_idx]
    ON [dbo].[AppLog]([LogTypeID] ASC);

