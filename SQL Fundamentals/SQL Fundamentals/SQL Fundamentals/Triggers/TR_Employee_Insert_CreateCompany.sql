CREATE TRIGGER [dbo].[TR_Employee_Insert_CreateCompany]
ON [dbo].[Employee]
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    -- Map each newly inserted Employee.Id to a brand-new Address.Id
    DECLARE @MaxAddressId INT;
    SELECT @MaxAddressId = ISNULL(MAX([Id]), 0) FROM [dbo].[Address];

    CREATE TABLE #NewAddresses
    (
        EmployeeId   INT NOT NULL,
        NewAddressId INT NOT NULL
    );

    -- Assign sequential IDs after the current Address maximum
    INSERT INTO #NewAddresses (EmployeeId, NewAddressId)
    SELECT
        i.[Id],
        @MaxAddressId + ROW_NUMBER() OVER (ORDER BY i.[Id])
    FROM INSERTED AS i;

    -- Copy the employee's address into a new Address row
    INSERT INTO [dbo].[Address] ([Id],              [Street],  [City],  [State],  [ZipCode])
    SELECT                       na.[NewAddressId],  a.[Street], a.[City], a.[State], a.[ZipCode]
    FROM        #NewAddresses      AS na
    INNER JOIN  INSERTED           AS i  ON i.[Id]      = na.[EmployeeId]
    INNER JOIN  [dbo].[Address]    AS a  ON a.[Id]      = i.[AddressId];

    -- Create a new Company row using the employee's CompanyName and the copied address
    DECLARE @MaxCompanyId INT;
    SELECT @MaxCompanyId = ISNULL(MAX([Id]), 0) FROM [dbo].[Company];

    INSERT INTO [dbo].[Company] ([Id],                                                              [Name],        [AddressId])
    SELECT                       @MaxCompanyId + ROW_NUMBER() OVER (ORDER BY na.[EmployeeId]),  i.[CompanyName], na.[NewAddressId]
    FROM        #NewAddresses  AS na
    INNER JOIN  INSERTED       AS i  ON i.[Id] = na.[EmployeeId];

    DROP TABLE #NewAddresses;
END;
