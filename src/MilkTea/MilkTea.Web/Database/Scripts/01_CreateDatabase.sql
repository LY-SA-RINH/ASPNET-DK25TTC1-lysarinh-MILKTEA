-- =============================================
-- Project: MilkTea House
-- File: 01_CreateDatabase.sql
-- Purpose: Tạo cơ sở dữ liệu MilkTeaHouse
-- =============================================

USE master;
GO

IF EXISTS (SELECT name FROM sys.databases WHERE name = N'MilkTeaHouse')
BEGIN
    ALTER DATABASE MilkTeaHouse
    SET SINGLE_USER WITH ROLLBACK IMMEDIATE;

    DROP DATABASE MilkTeaHouse;
END
GO

CREATE DATABASE MilkTeaHouse;
GO

USE MilkTeaHouse;
GO