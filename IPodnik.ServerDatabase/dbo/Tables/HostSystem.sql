CREATE TABLE [dbo].[HostSystem] (
    [HostSystemId] INT            IDENTITY (1, 1) NOT NULL,
    [Name]         NVARCHAR (255) NOT NULL,
    CONSTRAINT [UIX_HostSystem_HostSystemId] PRIMARY KEY CLUSTERED ([HostSystemId] ASC)
);





