-- =============================================
-- Script: Seed Data NugoloFamily
-- Descrizione: Dati iniziali per il sistema
-- =============================================

USE NugoloFamilyDB;
GO

-- =============================================
-- Seed: Assistenti predefiniti
-- =============================================
SET IDENTITY_INSERT Assistenti ON;
GO

INSERT INTO Assistenti (IdAssistente, NomeAssistente, CodiceAssistente, Descrizione, Categoria, Attivo, Ordinamento, RichiedeConfigurazione, DataInserimento, UtenteInserimento)
VALUES
(1, 'Hub Centrale', 'HUB', 'Assistente centrale per orchestrazione e smistamento richieste', 'Sistema', 1, 0, 0, GETDATE(), 'SYSTEM'),
(2, 'Gestione Agenda', 'AGENDA', 'Assistente per la gestione di appuntamenti, eventi e promemoria familiari', 'Produttività', 1, 1, 0, GETDATE(), 'SYSTEM'),
(3, 'Finanze Familiari', 'FINANZE', 'Assistente per la gestione di budget, spese e pianificazione finanziaria', 'Finanza', 1, 2, 0, GETDATE(), 'SYSTEM'),
(4, 'Consulenza Bancaria', 'BANCA', 'Assistente per operazioni bancarie e consigli su conti e investimenti', 'Finanza', 1, 3, 1, GETDATE(), 'SYSTEM'),
(5, 'Consulenza Assicurativa', 'ASSICURAZIONI', 'Assistente per gestione polizze assicurative e consulenza', 'Finanza', 1, 4, 1, GETDATE(), 'SYSTEM'),
(6, 'Assistente Cucina', 'CUCINA', 'Assistente per ricette, consigli alimentari e pianificazione pasti', 'Casa', 1, 5, 0, GETDATE(), 'SYSTEM'),
(7, 'Salute Familiare', 'SALUTE', 'Assistente per gestione cartelle sanitarie e promemoria medici', 'Salute', 1, 6, 0, GETDATE(), 'SYSTEM'),
(8, 'Gestione Documenti', 'DOCUMENTI', 'Assistente per archiviazione, catalogazione e ricerca documenti', 'Documenti', 1, 7, 0, GETDATE(), 'SYSTEM'),
(9, 'Assistente Email', 'EMAIL', 'Assistente per composizione e gestione email', 'Produttività', 1, 8, 1, GETDATE(), 'SYSTEM'),
(10, 'Gestione Casa', 'CASA', 'Assistente per manutenzione casa, bollette e servizi domestici', 'Casa', 1, 9, 0, GETDATE(), 'SYSTEM'),
(11, 'Pianificazione Viaggi', 'VIAGGI', 'Assistente per organizzazione viaggi e vacanze familiari', 'Tempo Libero', 1, 10, 0, GETDATE(), 'SYSTEM'),
(12, 'Supporto Compiti', 'COMPITI', 'Assistente per supporto scolastico e gestione compiti dei figli', 'Educazione', 1, 11, 0, GETDATE(), 'SYSTEM');

SET IDENTITY_INSERT Assistenti OFF;
GO

PRINT 'Assistenti predefiniti inseriti con successo!';
GO

-- =============================================
-- Seed: Famiglia di test (SOLO PER SVILUPPO)
-- =============================================
-- Decommentare per ambiente di sviluppo
/*
SET IDENTITY_INSERT Famiglie ON;
GO

INSERT INTO Famiglie (IdFamiglia, NomeFamiglia, CodiceUnivoco, Email, StatoAttivazione, PianoAbbonamento, LimiteUtenti, DataInserimento, UtenteInserimento)
VALUES
(1, 'Famiglia Rossi', 'FAM-ROSSI-2024', 'famiglia.rossi@example.com', 'Attivo', 'Premium', 10, GETDATE(), 'SYSTEM');

SET IDENTITY_INSERT Famiglie OFF;
GO

-- Password: Test@1234 (Hash semplificato per test - in produzione usare bcrypt o PBKDF2)
SET IDENTITY_INSERT Utenti ON;
GO

INSERT INTO Utenti (IdUtente, IdFamiglia, Username, Email, PasswordHash, Salt, Nome, Cognome, Ruolo, Attivo, DataInserimento, UtenteInserimento)
VALUES
(1, 1, 'mario.rossi', 'mario.rossi@example.com', 'HASH_PLACEHOLDER', 'SALT_PLACEHOLDER', 'Mario', 'Rossi', 'Amministratore', 1, GETDATE(), 'SYSTEM'),
(2, 1, 'laura.rossi', 'laura.rossi@example.com', 'HASH_PLACEHOLDER', 'SALT_PLACEHOLDER', 'Laura', 'Rossi', 'Familiare', 1, GETDATE(), 'SYSTEM');

SET IDENTITY_INSERT Utenti OFF;
GO

PRINT 'Famiglia di test creata!';
GO
*/
