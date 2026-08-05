-- Create Product table
CREATE TABLE Product (
    Id          INT IDENTITY(1,1) PRIMARY KEY,
    Name        NVARCHAR(200)  NOT NULL,
    Description NVARCHAR(1000) NULL,
    Weight      DECIMAL(18,4)  NOT NULL,
    Height      DECIMAL(18,4)  NOT NULL,
    Width       DECIMAL(18,4)  NOT NULL,
    Length      DECIMAL(18,4)  NOT NULL
);

-- Create Order table
CREATE TABLE [Order] (
    Id          INT IDENTITY(1,1) PRIMARY KEY,
    Status      NVARCHAR(50)   NOT NULL,
    CreatedDate DATETIME2      NOT NULL,
    UpdatedDate DATETIME2      NOT NULL,
    ProductId   INT            NOT NULL,
    CONSTRAINT FK_Order_Product FOREIGN KEY (ProductId) REFERENCES Product(Id)
);
