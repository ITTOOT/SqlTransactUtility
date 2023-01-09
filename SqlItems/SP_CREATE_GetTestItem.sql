SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:  XORtech
-- Create date: 07/25/2021
-- Description: SP_TestItemsRowNum
-- EXEC SP_GetTestItemsByStatus
-- Refresh >Programmability >Open Stored Procedures
-- =============================================
CREATE PROCEDURE SP_GetTestItemsByStatus
AS
BEGIN
   SET NOCOUNT ON;
   SELECT ROW_NUMBER() OVER(
       ORDER BY Status DESC) AS RowNum, 
       SerialNo, 
       Name, 
       Description, 
       MeasurementValue, 
       Status, 
       Timestamp
	FROM [TestData].[dbo].[TestTable]
END
GO
