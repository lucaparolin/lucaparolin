# Guida Integrazione AI - NugoloFamily

## 📋 Panoramica

L'integrazione AI è ora completamente implementata e funzionale. Il sistema supporta **4 provider AI** diversi con routing intelligente basato sul tipo di assistente.

## ✅ Componenti Implementati

### Backend
- **IAIProvider**: Interfaccia generica per tutti i provider AI
- **BaseAIProvider**: Classe base con funzionalità comuni
- **4 Provider Concreti**:
  - OpenAIProvider (ChatGPT)
  - ClaudeProvider (Anthropic)
  - DeepSeekProvider
  - GeminiProvider (Google)
- **AIProviderFactory**: Routing intelligente e selezione provider
- **MessaggiService**: Orchestrazione completa del flusso conversazionale

### Frontend
- **api-client.js**: Metodo `elaboraRispostaAssistente()` per chiamare l'AI
- **chat.html**: Integrazione completa con indicatore "sta scrivendo..."
- **Gestione errori**: Toast notifications per feedback utente

## 🔧 Configurazione

### 1. Aggiungi le API Keys

Modifica `NugoloFamily.API/appsettings.json`:

```json
"AIProviders": {
  "Default": "OpenAI",
  "OpenAI": {
    "ApiKey": "sk-YOUR-OPENAI-KEY-HERE",
    "BaseUrl": "https://api.openai.com/v1",
    "DefaultModel": "gpt-3.5-turbo"
  },
  "Claude": {
    "ApiKey": "sk-ant-YOUR-CLAUDE-KEY-HERE",
    "BaseUrl": "https://api.anthropic.com/v1",
    "DefaultModel": "claude-3-sonnet-20240229"
  },
  "DeepSeek": {
    "ApiKey": "YOUR-DEEPSEEK-KEY-HERE",
    "BaseUrl": "https://api.deepseek.com/v1",
    "DefaultModel": "deepseek-chat"
  },
  "Gemini": {
    "ApiKey": "YOUR-GEMINI-KEY-HERE",
    "BaseUrl": "https://generativelanguage.googleapis.com/v1",
    "DefaultModel": "gemini-pro"
  }
}
```

### 2. Ottieni le API Keys

- **OpenAI**: https://platform.openai.com/api-keys
- **Claude**: https://console.anthropic.com/settings/keys
- **DeepSeek**: https://platform.deepseek.com/api_keys
- **Gemini**: https://makersuite.google.com/app/apikey

**IMPORTANTE**: Puoi configurare anche solo UN provider per iniziare. Non è necessario avere tutte le chiavi.

## 🎯 Routing Assistenti → Provider

Il sistema mappa automaticamente ogni assistente al provider più adatto:

| Assistente | Provider | Motivazione |
|------------|----------|-------------|
| HUB | OpenAI | Coordinamento generale |
| AGENDA | Claude | Comprensione contestuale calendario |
| FINANZE | OpenAI | Analisi numerica |
| BANCA | Claude | Interpretazione documenti |
| ASSICURAZIONI | OpenAI | Analisi contratti |
| CUCINA | Gemini | Ricette e multimodalità |
| SALUTE | Claude | Comprensione medica |
| DOCUMENTI | OpenAI | Elaborazione testi |
| EMAIL | OpenAI | Comunicazione |
| CASA | Gemini | Gestione smart home |
| VIAGGI | Claude | Pianificazione complessa |
| COMPITI | DeepSeek | Aiuto educativo |

**Personalizzabile**: Modifica il mapping in `AIProviderFactory.cs` (linee 72-86).

## 🚀 Come Funziona

### Flusso di una Conversazione

1. **Utente invia messaggio** → Frontend chiama `/api/messaggi` (POST)
2. **Messaggio salvato** nel database
3. **Frontend richiede risposta AI** → Chiama `/api/messaggi/{id}/elabora-risposta` (POST)
4. **Backend carica contesto**:
   - Ultimi 20 messaggi della conversazione
   - Dati assistente (nome, tipo, configurazione)
   - System prompt personalizzato
5. **Selezione provider AI** basato su assistente
6. **Invio richiesta** al provider AI esterno
7. **Risposta salvata** nel database con tracking token
8. **Frontend riceve risposta** e la mostra all'utente

### System Prompts Personalizzati

Ogni assistente ha un system prompt specifico che definisce personalità e capacità:

**Esempio - HUB**:
```
Sei il coordinatore centrale della famiglia Nugolo. Aiuti a gestire tutte le attività
familiari, smisti le richieste agli assistenti specializzati, e fornisci una visione
d'insieme. Sei amichevole, organizzato e sempre pronto ad aiutare.
```

**Esempio - AGENDA**:
```
Sei l'assistente per la gestione dell'agenda familiare. Aiuti a organizzare appuntamenti,
eventi, compleanni e scadenze. Sei preciso con date e orari, e proattivo nel ricordare
gli impegni importanti.
```

Vedi tutti i prompt in `MessaggiService.cs` (linee 180-245).

## 📊 Tracking e Monitoraggio

Il sistema traccia automaticamente:

- **Token utilizzati** (prompt + completion)
- **Costo stimato** per ogni chiamata
- **Provider e modello** utilizzati
- **Tempo di risposta**
- **Errori** e fallimenti

Dati salvati in:
- `Log_AI` (tabella database per analytics)
- Console logs (per debugging)

## 🧪 Test del Sistema

### 1. Avvia l'applicazione
```bash
cd NugoloFamily.API
dotnet run
```

### 2. Apri il browser
Vai a: `https://localhost:5001` (o porta configurata)

### 3. Login
Usa le credenziali di test configurate nel database

### 4. Seleziona un assistente
Dalla home, clicca su un assistente (es. "HUB")

### 5. Invia un messaggio
Scrivi qualcosa come: "Ciao! Come puoi aiutarmi?"

### 6. Osserva la risposta
Dovresti vedere:
- Indicatore "sta scrivendo..." mentre l'AI elabora
- Risposta personalizzata dall'AI

## ⚠️ Troubleshooting

### Errore: "Provider non configurato"
**Causa**: API key mancante o errata
**Soluzione**: Verifica appsettings.json e aggiungi la chiave corretta

### Errore: "HTTP 401 Unauthorized"
**Causa**: API key non valida
**Soluzione**: Rigenera la chiave sul sito del provider

### Errore: "HTTP 429 Too Many Requests"
**Causa**: Rate limit superato
**Soluzione**: Aspetta qualche minuto o passa a un altro provider

### Nessuna risposta dall'AI
**Causa**: Problema di rete o timeout
**Soluzione**:
1. Controlla connessione internet
2. Verifica logs in console per dettagli
3. Aumenta timeout se necessario

### Risposta generica o errata
**Causa**: System prompt non ottimizzato
**Soluzione**: Modifica il prompt in `MessaggiService.GetSystemPromptForAssistant()`

## 💰 Gestione Costi

### Stima Costi per Provider (indicativa)

**OpenAI GPT-3.5-turbo**:
- Input: $0.0005/1K tokens
- Output: $0.0015/1K tokens
- Conversazione media: ~$0.001-0.005

**Claude Sonnet**:
- Input: $3/1M tokens
- Output: $15/1M tokens
- Conversazione media: ~$0.0005-0.003

**DeepSeek**:
- Input: $0.0001/1K tokens
- Output: $0.0002/1K tokens
- Più economico, ottimo per test

**Gemini**:
- Gratuito fino a limiti generosi
- Ottimo per sviluppo

### Consigli per Risparmiare

1. **Usa Gemini o DeepSeek** per sviluppo/test
2. **Limita storico conversazione** (attualmente 20 messaggi)
3. **Usa modelli più piccoli** (gpt-3.5-turbo invece di gpt-4)
4. **Monitora Log_AI** per analisi costi

## 🔐 Sicurezza

### Best Practices Implementate

✅ **API keys in configurazione** (non hardcoded)
✅ **Autenticazione JWT** su tutti gli endpoint
✅ **Validazione input** (lunghezza messaggi, parametri)
✅ **Rate limiting** (configurabile)
✅ **Logging errori** (senza esporre dati sensibili)
✅ **Multi-tenancy** (isolamento dati per famiglia)

### Raccomandazioni Produzione

1. **Non committare mai appsettings.json** con API keys reali
2. **Usa Azure Key Vault** o simile per secrets in produzione
3. **Abilita HTTPS** obbligatorio
4. **Configura rate limiting** per prevenire abusi
5. **Monitora costi** giornalmente

## 📚 Prossimi Passi

### Funzionalità Future (opzionali)

- [ ] **Streaming responses**: Mostra testo mentre viene generato
- [ ] **Supporto immagini**: Vision API per analisi documenti
- [ ] **Voice to text**: Integrazione Whisper per messaggi vocali
- [ ] **Text to speech**: Risposta vocale dagli assistenti
- [ ] **Function calling**: Tool use per azioni concrete (crea evento, invia email)
- [ ] **Context caching**: Riduzione costi ripetendo meno contesto
- [ ] **A/B testing**: Confronta qualità risposte tra provider
- [ ] **Fine-tuning**: Modelli personalizzati per casi specifici

### Miglioramenti Immediati

1. **Aggiungi retry logic** per chiamate AI fallite
2. **Implementa cache** per richieste simili
3. **Dashboard analytics** per monitoraggio uso/costi
4. **User feedback** (👍/👎 su risposte AI)
5. **Export conversazioni** in PDF/JSON

## 🎉 Conclusione

L'integrazione AI è **completa e funzionale**. Hai un sistema professionale con:

- ✅ Supporto multi-provider (OpenAI, Claude, DeepSeek, Gemini)
- ✅ Routing intelligente basato su assistente
- ✅ System prompts personalizzati
- ✅ Storico conversazione per contesto
- ✅ Tracking token e costi
- ✅ UI con feedback visivo
- ✅ Gestione errori robusta

**Per iniziare**: Aggiungi almeno una API key in `appsettings.json` e prova a chattare con un assistente!

---

**Documentazione creata**: 2025-11-05
**Versione**: 1.0
**Autore**: Claude (AI Integration Implementation)
