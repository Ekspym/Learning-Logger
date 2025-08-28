CREATE TABLE [dbo].[UserProfile] (
    [UserProfileId] INT            IDENTITY (1, 1) NOT NULL,
    [FirstName]     NVARCHAR (100) NOT NULL,
    [LastName]      NVARCHAR (100) NOT NULL,
    [Phone]         VARCHAR (50)   NULL,
    [Email]         VARCHAR (100)  NULL,
    [PasswordHash]  VARCHAR (255)  NULL,
    CONSTRAINT [UIX_UserProfile_UserProfileId] PRIMARY KEY CLUSTERED ([UserProfileId] ASC)
);









