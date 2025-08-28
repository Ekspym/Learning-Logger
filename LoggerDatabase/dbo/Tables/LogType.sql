CREATE TABLE [dbo].[LogType] (
    [LogTypeID]   INT           NOT NULL,
    [Name]        VARCHAR (15)  NOT NULL,
    [Description] VARCHAR (200) NULL,
    PRIMARY KEY CLUSTERED ([LogTypeID] ASC)
);

