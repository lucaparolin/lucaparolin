-- =============================================
-- Script: Creazione Tabelle NugoloFamily
-- Descrizione: Creazione di tutte le tabelle del sistema
-- =============================================

USE NugoloFamilyDB;
GO

-- =============================================
-- Tabella: Famiglie
-- Descrizione: Gestisce le famiglie registrate (multi-tenant)
-- =============================================
CREATE TABLE Famiglie (
    IdFamiglia INT IDENTITY(1,1) PRIMARY KEY,
    NomeFamiglia NVARCHAR(200) NOT NULL,
    CodiceUnivoco NVARCHAR(50) NOT NULL UNIQUE,
    Email NVARCHAR(255) NOT NULL,
    Telefono NVARCHAR(50),
    Indirizzo NVARCHAR(500),
    StatoAttivazione NVARCHAR(50) NOT NULL DEFAULT 'Attivo', -- Attivo, Sospeso, Disattivato
    DataScadenzaAbbonamento DATETIME,
    PianoAbbonamento NVARCHAR(50), -- Free, Basic, Premium, Enterprise
    LimiteUtenti INT DEFAULT 5,
    DataInserimento DATETIME NOT NULL DEFAULT GETDATE(),
    UtenteInserimento NVARCHAR(100) NOT NULL,
    IPInserimento NVARCHAR(50),
    DataModifica DATETIME,
    UtenteModifica NVARCHAR(100),
    IPModifica NVARCHAR(50)
);
GO

CREATE INDEX IX_Famiglie_CodiceUnivoco ON Famiglie(CodiceUnivoco);
CREATE INDEX IX_Famiglie_StatoAttivazione ON Famiglie(StatoAttivazione);
GO

-- =============================================
-- Tabella: Utenti
-- Descrizione: Utenti appartenenti alle famiglie
-- =============================================
CREATE TABLE Utenti (
    IdUtente INT IDENTITY(1,1) PRIMARY KEY,
    IdFamiglia INT NOT NULL,
    Username NVARCHAR(100) NOT NULL,
    Email NVARCHAR(255) NOT NULL,
    PasswordHash NVARCHAR(500) NOT NULL,
    Salt NVARCHAR(100) NOT NULL,
    Nome NVARCHAR(100) NOT NULL,
    Cognome NVARCHAR(100) NOT NULL,
    DataNascita DATE,
    Ruolo NVARCHAR(50) NOT NULL DEFAULT 'Familiare', -- Amministratore, Familiare
    Attivo BIT NOT NULL DEFAULT 1,
    UltimoAccesso DATETIME,
    TokenRefresh NVARCHAR(500),
    DataScadenzaToken DATETIME,
    DataInserimento DATETIME NOT NULL DEFAULT GETDATE(),
    UtenteInserimento NVARCHAR(100) NOT NULL,
    IPInserimento NVARCHAR(50),
    DataModifica DATETIME,
    UtenteModifica NVARCHAR(100),
    IPModifica NVARCHAR(50),
    CONSTRAINT FK_Utenti_Famiglie FOREIGN KEY (IdFamiglia) REFERENCES Famiglie(IdFamiglia)
);
GO

CREATE INDEX IX_Utenti_IdFamiglia ON Utenti(IdFamiglia);
CREATE INDEX IX_Utenti_Username ON Utenti(Username);
CREATE INDEX IX_Utenti_Email ON Utenti(Email);
GO

-- =============================================
-- Tabella: Assistenti
-- Descrizione: Assistenti specializzati disponibili nel sistema
-- =============================================
CREATE TABLE Assistenti (
    IdAssistente INT IDENTITY(1,1) PRIMARY KEY,
    NomeAssistente NVARCHAR(100) NOT NULL UNIQUE,
    CodiceAssistente NVARCHAR(50) NOT NULL UNIQUE, -- es: AGENDA, FINANZE, CUCINA, SALUTE
    Descrizione NVARCHAR(1000),
    Icona NVARCHAR(200), -- URL o path icona
    Categoria NVARCHAR(100), -- Produttività, Finanza, Salute, Casa, Documenti
    Attivo BIT NOT NULL DEFAULT 1,
    Ordinamento INT DEFAULT 0,
    RichiedeConfigurazione BIT DEFAULT 0,
    ParametriConfigurazione NVARCHAR(MAX), -- JSON con parametri configurabili
    DataInserimento DATETIME NOT NULL DEFAULT GETDATE(),
    UtenteInserimento NVARCHAR(100) NOT NULL,
    IPInserimento NVARCHAR(50),
    DataModifica DATETIME,
    UtenteModifica NVARCHAR(100),
    IPModifica NVARCHAR(50)
);
GO

CREATE INDEX IX_Assistenti_CodiceAssistente ON Assistenti(CodiceAssistente);
CREATE INDEX IX_Assistenti_Categoria ON Assistenti(Categoria);
GO

-- =============================================
-- Tabella: AssistentiFamiglie
-- Descrizione: Associazione tra famiglie e assistenti attivati
-- =============================================
CREATE TABLE AssistentiFamiglie (
    IdAssistenteFamiglia INT IDENTITY(1,1) PRIMARY KEY,
    IdFamiglia INT NOT NULL,
    IdAssistente INT NOT NULL,
    Attivo BIT NOT NULL DEFAULT 1,
    ConfigurazionePersonalizzata NVARCHAR(MAX), -- JSON con configurazioni specifiche
    DataAttivazione DATETIME NOT NULL DEFAULT GETDATE(),
    DataInserimento DATETIME NOT NULL DEFAULT GETDATE(),
    UtenteInserimento NVARCHAR(100) NOT NULL,
    IPInserimento NVARCHAR(50),
    DataModifica DATETIME,
    UtenteModifica NVARCHAR(100),
    IPModifica NVARCHAR(50),
    CONSTRAINT FK_AssistentiFamiglie_Famiglie FOREIGN KEY (IdFamiglia) REFERENCES Famiglie(IdFamiglia),
    CONSTRAINT FK_AssistentiFamiglie_Assistenti FOREIGN KEY (IdAssistente) REFERENCES Assistenti(IdAssistente),
    CONSTRAINT UQ_AssistentiFamiglie UNIQUE (IdFamiglia, IdAssistente)
);
GO

CREATE INDEX IX_AssistentiFamiglie_IdFamiglia ON AssistentiFamiglie(IdFamiglia);
CREATE INDEX IX_AssistentiFamiglie_IdAssistente ON AssistentiFamiglie(IdAssistente);
GO

-- =============================================
-- Tabella: ConfigurazioniAI
-- Descrizione: Configurazioni AI per famiglia (multi-modello)
-- =============================================
CREATE TABLE ConfigurazioniAI (
    IdConfigurazioneAI INT IDENTITY(1,1) PRIMARY KEY,
    IdFamiglia INT NOT NULL,
    NomeConfigurazione NVARCHAR(100) NOT NULL,
    ProviderAI NVARCHAR(50) NOT NULL, -- OpenAI, Claude, DeepSeek, Gemini, Local
    Modello NVARCHAR(100) NOT NULL, -- gpt-4, claude-3, deepseek-chat, gemini-pro
    ChiaveAPI NVARCHAR(500), -- Criptata
    Endpoint NVARCHAR(500),
    ParametriModello NVARCHAR(MAX), -- JSON: temperature, max_tokens, ecc.
    PredefinitaPerFamiglia BIT DEFAULT 0,
    AssistenteSpecifico INT, -- NULL = globale per famiglia
    Attiva BIT NOT NULL DEFAULT 1,
    DataInserimento DATETIME NOT NULL DEFAULT GETDATE(),
    UtenteInserimento NVARCHAR(100) NOT NULL,
    IPInserimento NVARCHAR(50),
    DataModifica DATETIME,
    UtenteModifica NVARCHAR(100),
    IPModifica NVARCHAR(50),
    CONSTRAINT FK_ConfigurazioniAI_Famiglie FOREIGN KEY (IdFamiglia) REFERENCES Famiglie(IdFamiglia),
    CONSTRAINT FK_ConfigurazioniAI_Assistenti FOREIGN KEY (AssistenteSpecifico) REFERENCES Assistenti(IdAssistente)
);
GO

CREATE INDEX IX_ConfigurazioniAI_IdFamiglia ON ConfigurazioniAI(IdFamiglia);
CREATE INDEX IX_ConfigurazioniAI_ProviderAI ON ConfigurazioniAI(ProviderAI);
GO

-- =============================================
-- Tabella: Conversazioni
-- Descrizione: Conversazioni tra utenti e assistenti
-- =============================================
CREATE TABLE Conversazioni (
    IdConversazione INT IDENTITY(1,1) PRIMARY KEY,
    IdFamiglia INT NOT NULL,
    IdUtente INT NOT NULL,
    IdAssistente INT NOT NULL,
    TitoloConversazione NVARCHAR(500),
    StatoConversazione NVARCHAR(50) DEFAULT 'Aperta', -- Aperta, Chiusa, Archiviata
    UltimoMessaggio DATETIME,
    ContatoreMessaggi INT DEFAULT 0,
    MetadatiConversazione NVARCHAR(MAX), -- JSON con info aggiuntive
    DataInserimento DATETIME NOT NULL DEFAULT GETDATE(),
    UtenteInserimento NVARCHAR(100) NOT NULL,
    IPInserimento NVARCHAR(50),
    DataModifica DATETIME,
    UtenteModifica NVARCHAR(100),
    IPModifica NVARCHAR(50),
    CONSTRAINT FK_Conversazioni_Famiglie FOREIGN KEY (IdFamiglia) REFERENCES Famiglie(IdFamiglia),
    CONSTRAINT FK_Conversazioni_Utenti FOREIGN KEY (IdUtente) REFERENCES Utenti(IdUtente),
    CONSTRAINT FK_Conversazioni_Assistenti FOREIGN KEY (IdAssistente) REFERENCES Assistenti(IdAssistente)
);
GO

CREATE INDEX IX_Conversazioni_IdFamiglia ON Conversazioni(IdFamiglia);
CREATE INDEX IX_Conversazioni_IdUtente ON Conversazioni(IdUtente);
CREATE INDEX IX_Conversazioni_IdAssistente ON Conversazioni(IdAssistente);
CREATE INDEX IX_Conversazioni_DataInserimento ON Conversazioni(DataInserimento);
GO

-- =============================================
-- Tabella: Messaggi
-- Descrizione: Messaggi scambiati nelle conversazioni (supporto multimodale)
-- =============================================
CREATE TABLE Messaggi (
    IdMessaggio BIGINT IDENTITY(1,1) PRIMARY KEY,
    IdConversazione INT NOT NULL,
    IdUtente INT, -- NULL se messaggio da assistente
    TipoMittente NVARCHAR(50) NOT NULL, -- Utente, Assistente
    TestoMessaggio NVARCHAR(MAX),
    TipoContenuto NVARCHAR(50) DEFAULT 'Testo', -- Testo, Audio, Video, Immagine, Documento
    URLContenuto NVARCHAR(1000), -- URL file multimediale
    DurataContenuto INT, -- Durata in secondi per audio/video
    DimensioneFile BIGINT, -- Dimensione in bytes
    MimeType NVARCHAR(100),
    MetadatiContenuto NVARCHAR(MAX), -- JSON con info aggiuntive
    Sentiment NVARCHAR(50), -- Positivo, Negativo, Neutro
    IntentRilevato NVARCHAR(200), -- Intent classificato dall'NLP
    ConfidenzaIntent DECIMAL(5,2), -- 0.00 - 100.00
    TokenUtilizzati INT, -- Per tracciamento costi AI
    ModelloAIUtilizzato NVARCHAR(100),
    DataInserimento DATETIME NOT NULL DEFAULT GETDATE(),
    UtenteInserimento NVARCHAR(100) NOT NULL,
    IPInserimento NVARCHAR(50),
    CONSTRAINT FK_Messaggi_Conversazioni FOREIGN KEY (IdConversazione) REFERENCES Conversazioni(IdConversazione),
    CONSTRAINT FK_Messaggi_Utenti FOREIGN KEY (IdUtente) REFERENCES Utenti(IdUtente)
);
GO

CREATE INDEX IX_Messaggi_IdConversazione ON Messaggi(IdConversazione);
CREATE INDEX IX_Messaggi_IdUtente ON Messaggi(IdUtente);
CREATE INDEX IX_Messaggi_TipoContenuto ON Messaggi(TipoContenuto);
CREATE INDEX IX_Messaggi_DataInserimento ON Messaggi(DataInserimento);
GO

-- =============================================
-- Tabella: Documenti
-- Descrizione: Gestione documenti caricati dalle famiglie
-- =============================================
CREATE TABLE Documenti (
    IdDocumento INT IDENTITY(1,1) PRIMARY KEY,
    IdFamiglia INT NOT NULL,
    IdUtente INT NOT NULL,
    NomeFile NVARCHAR(500) NOT NULL,
    PercorsoFile NVARCHAR(1000) NOT NULL,
    TipoDocumento NVARCHAR(100), -- Fattura, Contratto, Ricetta, Referto, ecc.
    Categoria NVARCHAR(100), -- Salute, Finanza, Casa, Legale, Altro
    DimensioneFile BIGINT,
    MimeType NVARCHAR(100),
    TestoEstratto NVARCHAR(MAX), -- Testo estratto da Textract
    MetadatiDocumento NVARCHAR(MAX), -- JSON con metadati estratti
    Indicizzato BIT DEFAULT 0,
    AssistenteAssociato INT, -- Assistente che gestisce questo tipo di documento
    DataDocumento DATE, -- Data del documento (se rilevata)
    DataScadenza DATE, -- Data scadenza (se applicabile)
    DataInserimento DATETIME NOT NULL DEFAULT GETDATE(),
    UtenteInserimento NVARCHAR(100) NOT NULL,
    IPInserimento NVARCHAR(50),
    DataModifica DATETIME,
    UtenteModifica NVARCHAR(100),
    IPModifica NVARCHAR(50),
    CONSTRAINT FK_Documenti_Famiglie FOREIGN KEY (IdFamiglia) REFERENCES Famiglie(IdFamiglia),
    CONSTRAINT FK_Documenti_Utenti FOREIGN KEY (IdUtente) REFERENCES Utenti(IdUtente),
    CONSTRAINT FK_Documenti_Assistenti FOREIGN KEY (AssistenteAssociato) REFERENCES Assistenti(IdAssistente)
);
GO

CREATE INDEX IX_Documenti_IdFamiglia ON Documenti(IdFamiglia);
CREATE INDEX IX_Documenti_IdUtente ON Documenti(IdUtente);
CREATE INDEX IX_Documenti_TipoDocumento ON Documenti(TipoDocumento);
CREATE INDEX IX_Documenti_Categoria ON Documenti(Categoria);
CREATE INDEX IX_Documenti_DataDocumento ON Documenti(DataDocumento);
GO

-- =============================================
-- Tabella: PermessiUtenti
-- Descrizione: Permessi granulari per utenti
-- =============================================
CREATE TABLE PermessiUtenti (
    IdPermesso INT IDENTITY(1,1) PRIMARY KEY,
    IdUtente INT NOT NULL,
    IdAssistente INT, -- NULL = permesso globale
    TipoPermesso NVARCHAR(100) NOT NULL, -- Lettura, Scrittura, Eliminazione, Configurazione
    Concesso BIT NOT NULL DEFAULT 1,
    DataInserimento DATETIME NOT NULL DEFAULT GETDATE(),
    UtenteInserimento NVARCHAR(100) NOT NULL,
    IPInserimento NVARCHAR(50),
    DataModifica DATETIME,
    UtenteModifica NVARCHAR(100),
    IPModifica NVARCHAR(50),
    CONSTRAINT FK_PermessiUtenti_Utenti FOREIGN KEY (IdUtente) REFERENCES Utenti(IdUtente),
    CONSTRAINT FK_PermessiUtenti_Assistenti FOREIGN KEY (IdAssistente) REFERENCES Assistenti(IdAssistente)
);
GO

CREATE INDEX IX_PermessiUtenti_IdUtente ON PermessiUtenti(IdUtente);
CREATE INDEX IX_PermessiUtenti_IdAssistente ON PermessiUtenti(IdAssistente);
GO

-- =============================================
-- Tabella: LogAttivita
-- Descrizione: Log di tutte le attività del sistema per audit
-- =============================================
CREATE TABLE LogAttivita (
    IdLog BIGINT IDENTITY(1,1) PRIMARY KEY,
    IdFamiglia INT,
    IdUtente INT,
    TipoAttivita NVARCHAR(100) NOT NULL, -- Login, Logout, InvioMessaggio, CreazioneDocumento, ecc.
    Descrizione NVARCHAR(MAX),
    EntitaCoinvolta NVARCHAR(100), -- Nome tabella
    IdEntita INT, -- ID record coinvolto
    IPAddress NVARCHAR(50),
    UserAgent NVARCHAR(500),
    Durata INT, -- Millisecondi
    Esito NVARCHAR(50), -- Successo, Fallimento
    DettagliErrore NVARCHAR(MAX),
    DataInserimento DATETIME NOT NULL DEFAULT GETDATE()
);
GO

CREATE INDEX IX_LogAttivita_IdFamiglia ON LogAttivita(IdFamiglia);
CREATE INDEX IX_LogAttivita_IdUtente ON LogAttivita(IdUtente);
CREATE INDEX IX_LogAttivita_TipoAttivita ON LogAttivita(TipoAttivita);
CREATE INDEX IX_LogAttivita_DataInserimento ON LogAttivita(DataInserimento);
GO

-- =============================================
-- Tabella: ConsumiAI
-- Descrizione: Tracciamento consumi AI per famiglia (billing)
-- =============================================
CREATE TABLE ConsumiAI (
    IdConsumo BIGINT IDENTITY(1,1) PRIMARY KEY,
    IdFamiglia INT NOT NULL,
    IdUtente INT,
    IdConfigurazioneAI INT,
    IdMessaggio BIGINT,
    ProviderAI NVARCHAR(50) NOT NULL,
    Modello NVARCHAR(100),
    TokenInput INT DEFAULT 0,
    TokenOutput INT DEFAULT 0,
    TokenTotali INT DEFAULT 0,
    CostoStimato DECIMAL(10,4), -- In valuta base
    DurataElaborazione INT, -- Millisecondi
    TipoRichiesta NVARCHAR(100), -- Chat, Classificazione, Estrazione, TTS, STT
    DataInserimento DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_ConsumiAI_Famiglie FOREIGN KEY (IdFamiglia) REFERENCES Famiglie(IdFamiglia),
    CONSTRAINT FK_ConsumiAI_Utenti FOREIGN KEY (IdUtente) REFERENCES Utenti(IdUtente),
    CONSTRAINT FK_ConsumiAI_ConfigurazioniAI FOREIGN KEY (IdConfigurazioneAI) REFERENCES ConfigurazioniAI(IdConfigurazioneAI),
    CONSTRAINT FK_ConsumiAI_Messaggi FOREIGN KEY (IdMessaggio) REFERENCES Messaggi(IdMessaggio)
);
GO

CREATE INDEX IX_ConsumiAI_IdFamiglia ON ConsumiAI(IdFamiglia);
CREATE INDEX IX_ConsumiAI_DataInserimento ON ConsumiAI(DataInserimento);
CREATE INDEX IX_ConsumiAI_ProviderAI ON ConsumiAI(ProviderAI);
GO

PRINT 'Tabelle create con successo!';
GO
