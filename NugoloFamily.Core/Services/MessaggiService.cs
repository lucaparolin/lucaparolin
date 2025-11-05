using NugoloFamily.Core.Interfaces.Repositories;
using NugoloFamily.Core.Interfaces.Services;
using NugoloFamily.Core.Models.DTOs;
using NugoloFamily.Core.Models.Entities;
using NugoloFamily.Core.Services.AI;

namespace NugoloFamily.Core.Services;

/// <summary>
/// Servizio per la gestione dei Messaggi con integrazione AI
/// </summary>
public class MessaggiService : IMessaggiService
{
    private readonly IMessaggiRepository _messaggiRepository;
    private readonly IConversazioniRepository _conversazioniRepository;
    private readonly IAssistentiRepository _assistentiRepository;
    private readonly AIProviderFactory _aiProviderFactory;

    public MessaggiService(
        IMessaggiRepository messaggiRepository,
        IConversazioniRepository conversazioniRepository,
        IAssistentiRepository assistentiRepository,
        AIProviderFactory aiProviderFactory)
    {
        _messaggiRepository = messaggiRepository;
        _conversazioniRepository = conversazioniRepository;
        _assistentiRepository = assistentiRepository;
        _aiProviderFactory = aiProviderFactory;
    }

    public async Task<MessaggioDataModel?> GetByIdAsync(long idMessaggio)
    {
        var messaggio = await _messaggiRepository.GetByIdAsync(idMessaggio);
        return messaggio != null ? MapToDataModel(messaggio) : null;
    }

    public async Task<IEnumerable<MessaggioDataModel>> GetByConversazioneAsync(int idConversazione)
    {
        var messaggi = await _messaggiRepository.GetByConversazioneAsync(idConversazione);
        return messaggi.Select(MapToDataModel);
    }

    public async Task<IEnumerable<MessaggioDataModel>> GetByConversazionePaginatiAsync(int idConversazione, int pagina, int dimensionePagina)
    {
        if (pagina < 1) pagina = 1;
        if (dimensionePagina < 1 || dimensionePagina > 100) dimensionePagina = 50;

        var messaggi = await _messaggiRepository.GetByConversazionePaginatiAsync(idConversazione, pagina, dimensionePagina);
        return messaggi.Select(MapToDataModel);
    }

    public async Task<MessaggioDataModel> InviaMessaggioAsync(InviaMessaggioDataModel model, string utenteCorrente, string? ipAddress)
    {
        // Validazioni
        var conversazione = await _conversazioniRepository.GetByIdAsync(model.IdConversazione);
        if (conversazione == null)
            throw new InvalidOperationException("Conversazione non trovata");

        if (conversazione.StatoConversazione == "Chiusa")
            throw new InvalidOperationException("Non è possibile inviare messaggi in una conversazione chiusa");

        if (string.IsNullOrWhiteSpace(model.TestoMessaggio) && string.IsNullOrWhiteSpace(model.URLContenuto))
            throw new ArgumentException("Il messaggio deve contenere testo o un contenuto multimediale");

        // Valida tipo contenuto
        var tipiValidi = new[] { "Testo", "Audio", "Video", "Immagine", "Documento" };
        if (!tipiValidi.Contains(model.TipoContenuto))
            throw new ArgumentException($"Tipo contenuto non valido. Tipi supportati: {string.Join(", ", tipiValidi)}");

        // Crea messaggio
        var entity = new MessaggioEntityModel
        {
            IdConversazione = model.IdConversazione,
            IdUtente = model.IdUtente,
            TipoMittente = "Utente",
            TestoMessaggio = model.TestoMessaggio?.Trim(),
            TipoContenuto = model.TipoContenuto,
            URLContenuto = model.URLContenuto,
            DurataContenuto = model.DurataContenuto,
            DimensioneFile = model.DimensioneFile,
            MimeType = model.MimeType,
            DataInserimento = DateTime.Now,
            UtenteInserimento = utenteCorrente,
            IPInserimento = ipAddress
        };

        var idMessaggio = await _messaggiRepository.CreateAsync(entity);
        entity.IdMessaggio = idMessaggio;

        // Aggiorna conversazione
        await _conversazioniRepository.IncrementaContatoreMessaggiAsync(model.IdConversazione);

        return MapToDataModel(entity);
    }

    public async Task<RispostaAssistenteDataModel> ElaboraRispostaAssistenteAsync(int idConversazione, long idMessaggioUtente)
    {
        // 1. Recuperare il messaggio dell'utente
        var messaggioUtente = await _messaggiRepository.GetByIdAsync(idMessaggioUtente);
        if (messaggioUtente == null)
            throw new InvalidOperationException("Messaggio utente non trovato");

        // 2. Recuperare il contesto della conversazione
        var conversazione = await _conversazioniRepository.GetByIdAsync(idConversazione);
        if (conversazione == null)
            throw new InvalidOperationException("Conversazione non trovata");

        // 3. Recuperare l'assistente associato
        var assistente = await _assistentiRepository.GetByIdAsync(conversazione.IdAssistente);
        if (assistente == null)
            throw new InvalidOperationException("Assistente non trovato");

        // 4. Recuperare lo storico dei messaggi (ultimi 20 per contesto)
        var storicoMessaggi = await _messaggiRepository.GetByConversazionePaginatiAsync(idConversazione, 1, 20);

        // 5. Ottieni il provider AI appropriato per questo assistente
        IAIProvider aiProvider;
        try
        {
            aiProvider = _aiProviderFactory.GetProviderForAssistant(assistente.CodiceAssistente);
        }
        catch
        {
            // Fallback al migliore disponibile
            aiProvider = await _aiProviderFactory.GetBestAvailableProviderAsync();
        }

        // 6. Prepara il contesto conversazione
        var conversationHistory = storicoMessaggi
            .OrderBy(m => m.DataInserimento)
            .Select(m => new AIMessageDataModel
            {
                Role = m.TipoMittente == "Utente" ? "user" : "assistant",
                Content = m.TestoMessaggio ?? string.Empty
            })
            .ToList();

        // 7. Crea system prompt personalizzato per l'assistente
        var systemPrompt = GetSystemPromptForAssistant(assistente);

        // 8. Prepara la richiesta AI
        var aiRequest = new AIRequestDataModel
        {
            Provider = aiProvider.ProviderName,
            UserMessage = messaggioUtente.TestoMessaggio ?? string.Empty,
            ConversationHistory = conversationHistory,
            SystemPrompt = systemPrompt,
            Temperature = 0.7,
            MaxTokens = 1000
        };

        // 9. Chiama il provider AI
        AIResponseDataModel aiResponse;
        try
        {
            aiResponse = await aiProvider.SendMessageAsync(aiRequest);
        }
        catch (Exception ex)
        {
            // In caso di errore, crea una risposta di fallback
            aiResponse = new AIResponseDataModel
            {
                Success = false,
                ErrorMessage = ex.Message,
                Content = "Mi dispiace, sto riscontrando un problema tecnico. Riprova tra qualche istante.",
                Provider = aiProvider.ProviderName,
                Model = "error"
            };
        }

        // 10. Analizza intent (semplificato, potrebbe essere migliorato)
        var (intent, confidenza) = AnalyzeIntent(messaggioUtente.TestoMessaggio);

        // 11. Crea messaggio di risposta
        var rispostaEntity = new MessaggioEntityModel
        {
            IdConversazione = idConversazione,
            IdUtente = null, // Messaggio dall'assistente
            TipoMittente = "Assistente",
            TestoMessaggio = aiResponse.Content,
            TipoContenuto = "Testo",
            IntentRilevato = intent,
            ConfidenzaIntent = confidenza,
            TokenUtilizzati = aiResponse.TotalTokens ?? 0,
            ModelloAIUtilizzato = $"{aiResponse.Provider}/{aiResponse.Model}",
            DataInserimento = DateTime.Now,
            UtenteInserimento = "SYSTEM",
            IPInserimento = null
        };

        var idRisposta = await _messaggiRepository.CreateAsync(rispostaEntity);
        rispostaEntity.IdMessaggio = idRisposta;

        // 12. Aggiorna conversazione
        await _conversazioniRepository.IncrementaContatoreMessaggiAsync(idConversazione);

        // 13. Log consumo AI (opzionale, da implementare)
        // await LogAIUsageAsync(conversazione.IdFamiglia, aiResponse);

        return new RispostaAssistenteDataModel
        {
            IdMessaggio = rispostaEntity.IdMessaggio,
            IdConversazione = idConversazione,
            TestoRisposta = aiResponse.Content,
            IntentRilevato = rispostaEntity.IntentRilevato,
            ConfidenzaIntent = rispostaEntity.ConfidenzaIntent,
            TokenUtilizzati = rispostaEntity.TokenUtilizzati,
            ModelloAIUtilizzato = rispostaEntity.ModelloAIUtilizzato,
            DataRisposta = rispostaEntity.DataInserimento
        };
    }

    /// <summary>
    /// Genera system prompt personalizzato per ogni assistente
    /// </summary>
    private string GetSystemPromptForAssistant(AssistenteEntityModel assistente)
    {
        var basePrompt = $"Sei {assistente.NomeAssistente}, {assistente.Descrizione}. " +
                        "Rispondi sempre in italiano, in modo cordiale e professionale. ";

        var specificPrompts = new Dictionary<string, string>
        {
            { "HUB", "Sei il coordinatore centrale della famiglia. Aiuti a organizzare e gestire tutte le attività familiari." },
            { "AGENDA", "Sei l'assistente per la gestione dell'agenda familiare. Aiuti con appuntamenti, eventi e promemoria." },
            { "FINANZE", "Sei l'esperto finanziario della famiglia. Aiuti con budget, spese e pianificazione finanziaria." },
            { "BANCA", "Sei il consulente bancario. Fornisci informazioni su servizi bancari, conti e transazioni." },
            { "ASSICURAZIONI", "Sei il consulente assicurativo. Aiuti con polizze, sinistri e coperture assicurative." },
            { "CUCINA", "Sei lo chef di famiglia. Suggerisci ricette, aiuti con la spesa e dai consigli culinari." },
            { "SALUTE", "Sei l'assistente sanitario. Fornisci informazioni su salute, benessere e visite mediche. NON fai diagnosi mediche." },
            { "DOCUMENTI", "Sei l'archivista digitale. Aiuti a organizzare, catalogare e recuperare documenti familiari." },
            { "EMAIL", "Sei l'assistente per la gestione email. Aiuti a organizzare, scrivere e rispondere alle email." },
            { "CASA", "Sei l'assistente per la gestione della casa. Aiuti con manutenzione, pulizie e organizzazione domestica." },
            { "VIAGGI", "Sei il consulente viaggi. Aiuti con prenotazioni, itinerari e consigli di viaggio." },
            { "COMPITI", "Sei il tutor per lo studio. Aiuti con i compiti scolastici e l'apprendimento." }
        };

        if (specificPrompts.TryGetValue(assistente.CodiceAssistente, out var specificPrompt))
        {
            return basePrompt + specificPrompt;
        }

        return basePrompt;
    }

    /// <summary>
    /// Analizza l'intent del messaggio (implementazione semplificata)
    /// In produzione si potrebbe usare un servizio di NLU dedicato
    /// </summary>
    private (string intent, decimal confidenza) AnalyzeIntent(string? messaggio)
    {
        if (string.IsNullOrWhiteSpace(messaggio))
            return ("Unknown", 0m);

        var messaggioLower = messaggio.ToLowerInvariant();

        // Intent mapping semplice basato su keywords
        var intentKeywords = new Dictionary<string, string[]>
        {
            { "Domanda", new[] { "cosa", "come", "perché", "quando", "dove", "chi", "quale", "?", "puoi", "sai" } },
            { "Richiesta", new[] { "vorrei", "voglio", "puoi", "potresti", "fammi", "aiutami", "aiuto" } },
            { "Informazione", new[] { "dimmi", "spiegami", "raccontami", "informazioni su" } },
            { "Saluto", new[] { "ciao", "buongiorno", "buonasera", "salve", "hey" } },
            { "Ringraziamento", new[] { "grazie", "grazie mille", "ti ringrazio", "perfetto" } }
        };

        foreach (var (intent, keywords) in intentKeywords)
        {
            if (keywords.Any(k => messaggioLower.Contains(k)))
            {
                return (intent, 0.75m);
            }
        }

        return ("General", 0.5m);
    }

    // Helper methods

    private MessaggioDataModel MapToDataModel(MessaggioEntityModel entity)
    {
        return new MessaggioDataModel
        {
            IdMessaggio = entity.IdMessaggio,
            IdConversazione = entity.IdConversazione,
            IdUtente = entity.IdUtente,
            TipoMittente = entity.TipoMittente,
            TestoMessaggio = entity.TestoMessaggio,
            TipoContenuto = entity.TipoContenuto,
            URLContenuto = entity.URLContenuto,
            DurataContenuto = entity.DurataContenuto,
            DimensioneFile = entity.DimensioneFile,
            MimeType = entity.MimeType,
            Sentiment = entity.Sentiment,
            IntentRilevato = entity.IntentRilevato,
            DataInserimento = entity.DataInserimento
        };
    }
}
