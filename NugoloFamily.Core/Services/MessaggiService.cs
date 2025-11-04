using NugoloFamily.Core.Interfaces.Repositories;
using NugoloFamily.Core.Interfaces.Services;
using NugoloFamily.Core.Models.DTOs;
using NugoloFamily.Core.Models.Entities;

namespace NugoloFamily.Core.Services;

/// <summary>
/// Servizio per la gestione dei Messaggi
/// </summary>
public class MessaggiService : IMessaggiService
{
    private readonly IMessaggiRepository _messaggiRepository;
    private readonly IConversazioniRepository _conversazioniRepository;

    public MessaggiService(
        IMessaggiRepository messaggiRepository,
        IConversazioniRepository conversazioniRepository)
    {
        _messaggiRepository = messaggiRepository;
        _conversazioniRepository = conversazioniRepository;
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
        // Questa è una implementazione placeholder
        // In una implementazione reale, questo metodo dovrebbe:
        // 1. Recuperare il messaggio dell'utente
        // 2. Recuperare il contesto della conversazione
        // 3. Chiamare il servizio AI appropriato
        // 4. Classificare l'intent
        // 5. Salvare la risposta dell'assistente
        // 6. Tracciare i token utilizzati

        var messaggioUtente = await _messaggiRepository.GetByIdAsync(idMessaggioUtente);
        if (messaggioUtente == null)
            throw new InvalidOperationException("Messaggio utente non trovato");

        var conversazione = await _conversazioniRepository.GetByIdAsync(idConversazione);
        if (conversazione == null)
            throw new InvalidOperationException("Conversazione non trovata");

        // Placeholder: genera risposta semplice
        var testoRisposta = "Ciao! Questa è una risposta placeholder. " +
                           "L'integrazione AI verrà implementata successivamente.";

        // Crea messaggio di risposta
        var rispostaEntity = new MessaggioEntityModel
        {
            IdConversazione = idConversazione,
            IdUtente = null, // Messaggio dall'assistente
            TipoMittente = "Assistente",
            TestoMessaggio = testoRisposta,
            TipoContenuto = "Testo",
            IntentRilevato = "General",
            ConfidenzaIntent = 0.85m,
            TokenUtilizzati = 50,
            ModelloAIUtilizzato = "placeholder-model",
            DataInserimento = DateTime.Now,
            UtenteInserimento = "SYSTEM",
            IPInserimento = null
        };

        var idRisposta = await _messaggiRepository.CreateAsync(rispostaEntity);
        rispostaEntity.IdMessaggio = idRisposta;

        // Aggiorna conversazione
        await _conversazioniRepository.IncrementaContatoreMessaggiAsync(idConversazione);

        return new RispostaAssistenteDataModel
        {
            IdMessaggio = rispostaEntity.IdMessaggio,
            IdConversazione = idConversazione,
            TestoRisposta = testoRisposta,
            IntentRilevato = rispostaEntity.IntentRilevato,
            ConfidenzaIntent = rispostaEntity.ConfidenzaIntent,
            TokenUtilizzati = rispostaEntity.TokenUtilizzati,
            ModelloAIUtilizzato = rispostaEntity.ModelloAIUtilizzato,
            DataRisposta = rispostaEntity.DataInserimento
        };
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
