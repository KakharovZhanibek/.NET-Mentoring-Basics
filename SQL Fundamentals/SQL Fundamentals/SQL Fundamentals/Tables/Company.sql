CREATE TABLE [dbo].[Company]
(
    [Id]        INT          NOT NULL,
    [Name]      NVARCHAR(20) NOT NULL,
    [AddressId] INT          NOT NULL,

    CONSTRAINT [PK_Company]             PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Company_Address]     FOREIGN KEY ([AddressId]) REFERENCES [dbo].[Address] ([Id])
);
