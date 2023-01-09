-- =============================================
-- Author:  XORtech
-- Create date: 07/25/2021
-- Description: Enable CLR within SQL Server
-- execute sp_configure system stored procedure by passing two parameters viz. 
-- clr_enabled and 1. 
-- To disable  call the same stored procedure with second parameter as 0, 
-- Call RECONFIGURE so that the new settings are in effect.
-- =============================================
sp_configure 'clr enabled', 1 
GO 
RECONFIGURE 
GO
