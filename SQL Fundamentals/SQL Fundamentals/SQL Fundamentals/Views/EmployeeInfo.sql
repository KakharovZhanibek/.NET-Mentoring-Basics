CREATE VIEW [dbo].[EmployeeInfo]
AS
    SELECT TOP (100) PERCENT
        e.[Id]                                                                 AS [EmployeeId],
        COALESCE(e.[EmployeeName], p.[FirstName] + N' ' + p.[LastName])        AS [EmployeeFullName],
        a.[ZipCode] + N'_' + a.[State] + N', ' + a.[City] + N'-' + a.[Street]  AS [EmployeeFullAddress],
        e.[CompanyName] + N'(' + ISNULL(e.[Position], N'') + N')'              AS [EmployeeCompanyInfo]
    FROM       [dbo].[Employee] AS e
    INNER JOIN [dbo].[Person]   AS p ON p.[Id] = e.[PersonId]
    INNER JOIN [dbo].[Address]  AS a ON a.[Id] = e.[AddressId]
    ORDER BY   e.[CompanyName] ASC, a.[City] ASC;
