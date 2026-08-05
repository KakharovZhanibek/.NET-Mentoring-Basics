-- =============================================
-- Stored Procedure: GetOrders
-- Fetches orders filtered by month, year, status and/or productId.
-- All parameters are optional.
-- =============================================
CREATE OR ALTER PROCEDURE GetOrders
    @Month     INT          = NULL,
    @Year      INT          = NULL,
    @Status    NVARCHAR(50) = NULL,
    @ProductId INT          = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, Status, CreatedDate, UpdatedDate, ProductId
    FROM [Order]
    WHERE
        (@Month     IS NULL OR MONTH(CreatedDate) = @Month)
        AND (@Year  IS NULL OR YEAR(CreatedDate)  = @Year)
        AND (@Status IS NULL OR Status            = @Status)
        AND (@ProductId IS NULL OR ProductId      = @ProductId);
END;
GO

-- =============================================
-- Stored Procedure: DeleteOrders
-- Deletes orders in bulk on the same filter conditions as GetOrders.
-- =============================================
CREATE OR ALTER PROCEDURE DeleteOrders
    @Month     INT          = NULL,
    @Year      INT          = NULL,
    @Status    NVARCHAR(50) = NULL,
    @ProductId INT          = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM [Order]
    WHERE
        (@Month     IS NULL OR MONTH(CreatedDate) = @Month)
        AND (@Year  IS NULL OR YEAR(CreatedDate)  = @Year)
        AND (@Status IS NULL OR Status            = @Status)
        AND (@ProductId IS NULL OR ProductId      = @ProductId);
END;
GO
