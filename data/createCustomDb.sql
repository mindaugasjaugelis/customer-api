IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'Custom')
BEGIN
    CREATE DATABASE Custom;
END;
GO

USE Custom;

IF OBJECT_ID('Customer', 'U') IS NULL
BEGIN
	CREATE TABLE dbo.Customer (
		Id INT NOT NULL PRIMARY KEY IDENTITY(1,1),
		Name NVARCHAR(255),
		Address NVARCHAR(255),
		PostCode NVARCHAR(20)
	);
END;
GO

IF OBJECT_ID('Log', 'U') IS NULL
BEGIN
	CREATE TABLE dbo.Log (
		Id INT NOT NULL PRIMARY KEY IDENTITY(1,1),
		EntityTypeId INT NOT NULL, --Customer or any other Entity in the future
		EntityId INT NOT NULL,
		ActionId INT NOT NULL, --Create, Update, Delete
		LogCreatedAt DATETIME2 NOT NULL,
		EntityJson NVARCHAR(MAX)
	);
END;
GO
