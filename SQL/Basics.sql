CREATE DATABASE [FruitsStoreBackendASPNETDatabase]
GO
    USE FruitsStoreBackendASPNETDatabase
GO
    CREATE SCHEMA FruitsStoreBackendSchema;

GO
    CREATE TABLE [FruitsStoreBackendSchema].Auth(
        Email NVARCHAR(50) PRIMARY KEY,
        PasswordHash VARBINARY(MAX),
        PasswordSalt VARBINARY(MAX)
    )
GO
    CREATE TABLE [FruitsStoreBackendSchema].Users (
        UserId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
        FirstName NVARCHAR(50),
        LastName NVARCHAR(50),
        Email NVARCHAR(50),
        Gender NVARCHAR(50),
        Active BIT,
        CreatedAt DATETIME2
    );

GO
    CREATE TABLE [FruitsStoreBackendSchema].Fruit(
        FruitId UNIQUEIDENTIFIER PRIMARY KEY,
        Name NVARCHAR(255) NOT NULL,
        Price DECIMAL(18, 2) NOT NULL,
        FruitType INT NOT NULL,
        ImageBase64 NVARCHAR(MAX) NULL,
        AddedBy UNIQUEIDENTIFIER NOT NULL,
        UpdatedBy UNIQUEIDENTIFIER NOT NULL,
        CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
        UpdatedAt DATETIME NOT NULL DEFAULT GETDATE()
    )
GO