CREATE TABLE [dbo].[Address]
(
    [Id]      INT          NOT NULL,
    [Street]  NVARCHAR(50) NOT NULL,
    [City]    NVARCHAR(20) NULL,
    [State]   NVARCHAR(50) NULL,
    [ZipCode] NVARCHAR(50) NULL,

    CONSTRAINT [PK_Address] PRIMARY KEY CLUSTERED ([Id] ASC)
);
