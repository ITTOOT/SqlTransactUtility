-- =============================================
-- Author:  XORtech
-- Create date: 07/25/2021
-- Description: clr enabled 
-- Enable CLR integration feature for your SQL Server. 
-- By default this feature is turned off. 
-- To turn it on open SQL Server Management Studio and 
-- execute the following script.
-- =============================================
sp_configure 'clr enabled', 1 
GO 
RECONFIGURE 
GO 