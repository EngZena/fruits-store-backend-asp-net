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
        UserId INT IDENTITY(1, 1) PRIMARY KEY,
        FirstName NVARCHAR(50),
        LastName NVARCHAR(50),
        Email NVARCHAR(50),
        Gender NVARCHAR(50),
        Active BIT
    );

GO