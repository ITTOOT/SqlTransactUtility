USE [TestData]
GO
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
GO
-- first create the Table to hold the test data
IF (NOT EXISTS(
SELECT so.*
FROM sys.objects so
WHERE so.[name] = 'ImportTestData'
AND so.[type] = 'U'
))
BEGIN
CREATE TABLE ImportTestData (
NewSerialNo INT, 
NewName VARCHAR(16), 
NewStatus VARCHAR(4), 
NewTimestamp DATETIME
)
END
GO


-- second create a Type that is the same structure as the Table
-- we just created that will be used as the datatype of the input parameter
-- of the Stored Procedure that will import the data
IF (NOT EXISTS(
SELECT st.*
FROM sys.types st
WHERE st.[name] = 'ImportTestStructure'
                ))
BEGIN
CREATE TYPE ImportTestStructure AS TABLE (
NewSerialNo INT, 
NewName VARCHAR(16), 
NewStatus VARCHAR(4), 
NewTimestamp DATETIME
)
END
GO


-- Create TVP stored procedure
IF (EXISTS(
SELECT so.*
FROM sys.objects so
WHERE so.[name] = 'ImportTestDataTVP'
AND so.[type] = 'P'
))
BEGIN
DROP PROCEDURE dbo.ImportTestDataTVP
END
GO

CREATE PROCEDURE dbo.ImportTestDataTVP (
@ImportTable dbo.ImportTestStructure READONLY
)
AS
SET NOCOUNT ON
INSERT INTO dbo.ImportTestData (NewSerialNo, NewName, NewStatus, NewTimestamp)
SELECT SerialNo, Name, Status, Timestamp
FROM @TestTable

GO