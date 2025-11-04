-- =============================================
-- Script: Creazione Database NugoloFamily
-- Descrizione: Creazione del database principale
-- =============================================

USE master;
GO

-- Elimina il database se esiste (solo per sviluppo)
IF EXISTS (SELECT name FROM sys.databases WHERE name = 'NugoloFamilyDB')
BEGIN
    ALTER DATABASE NugoloFamilyDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE NugoloFamilyDB;
END
GO

-- Crea il database
CREATE DATABASE NugoloFamilyDB
ON PRIMARY
(
    NAME = N'NugoloFamilyDB_Data',
    FILENAME = N'C:\SQLData\NugoloFamilyDB_Data.mdf',
    SIZE = 100MB,
    MAXSIZE = UNLIMITED,
    FILEGROWTH = 10MB
)
LOG ON
(
    NAME = N'NugoloFamilyDB_Log',
    FILENAME = N'C:\SQLData\NugoloFamilyDB_Log.ldf',
    SIZE = 50MB,
    MAXSIZE = 1GB,
    FILEGROWTH = 10MB
);
GO

ALTER DATABASE NugoloFamilyDB SET RECOVERY SIMPLE;
GO

USE NugoloFamilyDB;
GO

PRINT 'Database NugoloFamilyDB creato con successo!';
GO
