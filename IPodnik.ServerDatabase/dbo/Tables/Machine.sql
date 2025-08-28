CREATE TABLE [dbo].[Machine] (
    [MachineId]    INT            IDENTITY (1, 1) NOT NULL,
    [Name]         NVARCHAR (255) NOT NULL,
    [Status]       NVARCHAR (50)  NULL,
    [RAM]          INT            NULL,
    [Storage]      INT            NULL,
    [IPAddress]    NVARCHAR (45)  NULL,
    [HostSystemId] INT            NULL,
    CONSTRAINT [UIX_Machine_MachineId] PRIMARY KEY CLUSTERED ([MachineId] ASC),
    CONSTRAINT [FK_Machine_HostSystemId] FOREIGN KEY ([HostSystemId]) REFERENCES [dbo].[HostSystem] ([HostSystemId])
);










GO
CREATE NONCLUSTERED INDEX [IX_Machine_HostSystemId]
    ON [dbo].[Machine]([HostSystemId] ASC);

