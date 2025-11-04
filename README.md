# NugoloFamily - Assistente Digitale Familiare

## Descrizione

NugoloFamily è un'applicazione PWA scritta in C# che funge da assistente digitale familiare personalizzabile e modulare. Gestisce tutte le informazioni e le interazioni quotidiane di una famiglia attraverso un'interfaccia vocale, visiva e testuale.

## Caratteristiche Principali

- **Architettura Multi-Tenant**: Ogni famiglia ha i propri dati protetti e separati
- **Assistenti Specializzati**: Moduli dedicati per domini specifici (Agenda, Finanze, Cucina, Salute, Documenti, ecc.)
- **Hub Centrale**: Orchestrazione intelligente delle richieste tra assistenti
- **Supporto Multi-AI**: Integrazione con ChatGPT, DeepSeek, Claude, Gemini
- **Interfaccia Multimodale**: Chat con supporto per testo, audio, video e immagini
- **Voice Interface**: Amazon Polly per TTS, Alexa SDK per STT
- **Document Processing**: AWS Textract per parsing documenti

## Struttura del Progetto

```
/NugoloFamily.API          - ASP.NET Core Web API
  /Controllers             - Controller REST API
  /Middleware              - Middleware custom

/NugoloFamily.Core         - Logica di business
  /Models
    /Entities              - Modelli database (*EntityModel.cs)
    /DTOs                  - Data Transfer Objects (*DataModel.cs)
  /Interfaces
    /Repositories          - Interfacce repository
    /Services              - Interfacce servizi
  /Services                - Implementazione logica business

/NugoloFamily.Infrastructure  - Accesso dati e servizi esterni
  /Data
    /Repositories          - Repository ADO.NET
  /ExternalServices
    /AI                    - Integrazione AI providers
    /AWS                   - Servizi AWS (Polly, Textract)

/NugoloFamily.Shared       - Utilities condivise
  /Constants               - Costanti
  /Helpers                 - Helper functions
  /Extensions              - Extension methods

/Database
  /Scripts                 - Script SQL
```

## Tecnologie Utilizzate

- **Backend**: ASP.NET Core 8.0
- **Database**: SQL Server (accesso tramite ADO.NET)
- **Authentication**: JWT Bearer Tokens
- **AI Integration**: OpenAI, Claude, DeepSeek, Gemini
- **Cloud Services**: AWS (Polly, Textract, Alexa)
- **Frontend**: Progressive Web App (da implementare)

## Setup Database

### 1. Prerequisiti

- SQL Server 2019 o superiore
- SQL Server Management Studio (opzionale)

### 2. Creazione Database

Eseguire gli script SQL nell'ordine:

```bash
# 1. Creare il database
sqlcmd -S localhost -U sa -P YourPassword -i Database/Scripts/01_CreateDatabase.sql

# 2. Creare le tabelle
sqlcmd -S localhost -U sa -P YourPassword -i Database/Scripts/02_CreateTables.sql

# 3. Inserire dati iniziali
sqlcmd -S localhost -U sa -P YourPassword -i Database/Scripts/03_SeedData.sql
```

### 3. Configurazione Connection String

Modificare `appsettings.json` in NugoloFamily.API:

```json
{
  "ConnectionStrings": {
    "NugoloFamilyDB": "Server=YOUR_SERVER;Database=NugoloFamilyDB;User Id=YOUR_USER;Password=YOUR_PASSWORD;TrustServerCertificate=True;"
  }
}
```

## Tabelle Database Principali

### Famiglie
Gestione delle famiglie registrate (multi-tenant)

### Utenti
Utenti appartenenti alle famiglie con ruoli (Amministratore, Familiare)

### Assistenti
Assistenti specializzati disponibili:
- Hub Centrale
- Gestione Agenda
- Finanze Familiari
- Consulenza Bancaria
- Consulenza Assicurativa
- Assistente Cucina
- Salute Familiare
- Gestione Documenti
- Assistente Email
- Gestione Casa
- Pianificazione Viaggi
- Supporto Compiti

### Conversazioni & Messaggi
Gestione chat multimodale tra utenti e assistenti

### Documenti
Archiviazione e catalogazione documenti con estrazione testo (Textract)

### ConfigurazioniAI
Configurazioni AI personalizzabili per famiglia (multi-modello)

### ConsumiAI
Tracciamento utilizzo AI per billing

## Convenzioni di Codifica

### Naming
- Campi database: **Italiano** (es. `IdFamiglia`, `NomeFamiglia`)
- Suffissi:
  - Entity Models: `*EntityModel.cs`
  - Data Models (DTO): `*DataModel.cs`
  - Controllers: `*Controller.cs`
  - Repositories: `*Repository.cs`
  - Services: `*Service.cs`

### Campi di Tracciamento
Ogni tabella include:
- `DataInserimento`, `UtenteInserimento`, `IPInserimento`
- `DataModifica`, `UtenteModifica`, `IPModifica`

## API Endpoints (Esempio Famiglie)

```
GET    /api/famiglie              - Ottiene tutte le famiglie
GET    /api/famiglie/{id}         - Ottiene famiglia per ID
GET    /api/famiglie/codice/{cod} - Ottiene famiglia per codice
POST   /api/famiglie              - Crea nuova famiglia
PUT    /api/famiglie/{id}         - Aggiorna famiglia
DELETE /api/famiglie/{id}         - Elimina famiglia
```

## Esecuzione del Progetto

### Prerequisiti
- .NET 8.0 SDK
- SQL Server

### Build & Run

```bash
# Restore dependencies
dotnet restore

# Build solution
dotnet build

# Run API
cd NugoloFamily.API
dotnet run
```

L'API sarà disponibile su:
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`
- Swagger UI: `https://localhost:5001/swagger`

## Prossimi Step di Sviluppo

### Backend
1. ✅ Setup struttura progetto
2. ✅ Creazione database e tabelle
3. ✅ Modelli Entity e DTO
4. ✅ Interfacce Repository
5. ✅ Repository ADO.NET (FamiglieRepository completato)
6. ⏳ Repository rimanenti (Utenti, Assistenti, Conversazioni, Messaggi, Documenti)
7. ⏳ Servizi Business Logic
8. ⏳ Authentication & Authorization (JWT)
9. ⏳ Integrazione AI Providers
10. ⏳ Integrazione AWS (Polly, Textract)
11. ⏳ Hub Centrale e Routing Intelligente
12. ⏳ Implementazione assistenti specializzati

### Frontend
1. ⏳ Progressive Web App
2. ⏳ Interfaccia chat multimodale
3. ⏳ Voice interface
4. ⏳ Gestione documenti
5. ⏳ Dashboard famiglia

### Back Office
1. ⏳ Gestione piattaforma
2. ⏳ Sistema di billing
3. ⏳ Monitoraggio utilizzo
4. ⏳ Analytics

## Configurazione AI Providers

### OpenAI
```json
{
  "AIProviders": {
    "OpenAI": {
      "BaseUrl": "https://api.openai.com/v1",
      "DefaultModel": "gpt-4"
    }
  }
}
```

### Claude (Anthropic)
```json
{
  "AIProviders": {
    "Claude": {
      "BaseUrl": "https://api.anthropic.com/v1",
      "DefaultModel": "claude-3-opus-20240229"
    }
  }
}
```

### DeepSeek
```json
{
  "AIProviders": {
    "DeepSeek": {
      "BaseUrl": "https://api.deepseek.com/v1",
      "DefaultModel": "deepseek-chat"
    }
  }
}
```

### Gemini
```json
{
  "AIProviders": {
    "Gemini": {
      "BaseUrl": "https://generativelanguage.googleapis.com/v1",
      "DefaultModel": "gemini-pro"
    }
  }
}
```

## Principi SOLID Applicati

- **Single Responsibility**: Ogni classe ha una singola responsabilità
- **Open/Closed**: Estensibile tramite interfacce
- **Liskov Substitution**: Implementazioni repository sostituibili
- **Interface Segregation**: Interfacce specifiche per ogni repository
- **Dependency Inversion**: Dipendenza da astrazioni (interfacce)

## Sicurezza

- Autenticazione JWT
- Isolamento dati multi-tenant (filtro per IdFamiglia)
- Password hash con salt
- Validazione input
- Protezione CSRF
- Rate limiting (da implementare)
- Audit logging completo

## Licenza

Proprietario: NugoloFamily
Tutti i diritti riservati

## Contatti

Per informazioni: info@nugolofamily.com
