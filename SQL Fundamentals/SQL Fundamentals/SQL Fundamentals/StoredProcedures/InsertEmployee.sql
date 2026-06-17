CREATE PROCEDURE [dbo].[InsertEmployee]
    @EmployeeName NVARCHAR(100) = NULL,
    @FirstName    NVARCHAR(50)  = NULL,
    @LastName     NVARCHAR(50)  = NULL,
    @CompanyName  NVARCHAR(100),
    @Position     NVARCHAR(30)  = NULL,
    @Street       NVARCHAR(50),
    @City         NVARCHAR(20)  = NULL,
    @State        NVARCHAR(50)  = NULL,
    @ZipCode      NVARCHAR(50)  = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- Condition 1: at least one name field must be non-NULL, non-empty, non-whitespace
    IF (
        NULLIF(LTRIM(RTRIM(@EmployeeName)), N'') IS NULL AND
        NULLIF(LTRIM(RTRIM(@FirstName)),    N'') IS NULL AND
        NULLIF(LTRIM(RTRIM(@LastName)),     N'') IS NULL
    )
    BEGIN
        RAISERROR(N'At least one of EmployeeName, FirstName or LastName must be a non-empty, non-whitespace value.', 16, 1);
        RETURN;
    END;

    -- Condition 2: truncate CompanyName to 20 characters if it exceeds the limit
    IF LEN(@CompanyName) > 20
    BEGIN
        SET @CompanyName = LEFT(@CompanyName, 20);
    END;

    DECLARE @NewPersonId   INT;
    DECLARE @NewAddressId  INT;
    DECLARE @NewEmployeeId INT;

    SELECT @NewPersonId   = ISNULL(MAX([Id]), 0) + 1 FROM [dbo].[Person];
    SELECT @NewAddressId  = ISNULL(MAX([Id]), 0) + 1 FROM [dbo].[Address];
    SELECT @NewEmployeeId = ISNULL(MAX([Id]), 0) + 1 FROM [dbo].[Employee];

    INSERT INTO [dbo].[Person] ([Id], [FirstName],              [LastName])
    VALUES                     (@NewPersonId, ISNULL(@FirstName, N''), ISNULL(@LastName, N''));

    INSERT INTO [dbo].[Address] ([Id],           [Street], [City], [State], [ZipCode])
    VALUES                      (@NewAddressId,  @Street,  @City,  @State,  @ZipCode);

    INSERT INTO [dbo].[Employee] ([Id],            [AddressId],   [PersonId],   [CompanyName], [Position], [EmployeeName])
    VALUES                       (@NewEmployeeId,  @NewAddressId, @NewPersonId, @CompanyName,  @Position,  @EmployeeName);
END;
