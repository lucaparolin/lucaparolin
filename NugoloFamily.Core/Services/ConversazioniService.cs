using NugoloFamily.Core.Interfaces.Repositories;
using NugoloFamily.Core.Interfaces.Services;
using NugoloFamily.Core.Models.DTOs;
using NugoloFamily.Core.Models.Entities;

namespace NugoloFamily.Core.Services;

/// <summary>
/// Servizio per la gestione delle Conversazioni
/// </summary>
public class ConversazioniService : IConversazioniService
{
    private readonly IConversazioniRepository _conversazioniRepository;
    private readonly IMessaggiRepository _messaggiRepository;
    private readonly IAssistentiRepository _assistentiRepository;
    private readonly IUtentiRepository _utentiRepository;

    public ConversazioniService(
        IConversazioniRepository conversazioniRepository,
        IMessaggiRepository messaggiRepository,
        IAssistentiRepository assistentiRepository,
        IUtentiRepository utentiRepository)
    {
        _conversazioniRepository = conversazioniRepository;
        _messaggiRepository = messaggiRepository;
        _assistentiRepository = assistentiRepository;
        _utentiRepository = utentiRepository;
    }

    public async Task<ConversazioneDataModel?> GetByIdAsync(int idConversazione)
    {
        var conversazione = await _conversazioniRepository.GetByIdAsync(idConversazione);
        if (conversazione == null)
            return null;

        var assistente = await _assistentiRepository.GetByIdAsync(conversazione.IdAssistente);

        return new ConversazioneDataModel
        {
            IdConversazione = conversazione.IdConversazione,
            IdFamiglia = conversazione.IdFamiglia,
            IdUtente = conversazione.IdUtente,
            IdAssistente = conversazione.IdAssistente,
            NomeAssistente = assistente?.NomeAssistente ?? "Sconosciuto",
            TitoloConversazione = conversazione.TitoloConversazione,
            StatoConversazione = conversazione.StatoConversazione,
            UltimoMessaggio = conversazione.UltimoMessaggio,
            ContatoreMessaggi = conversazione.ContatoreMessaggi,
            DataInserimento = conversazione.DataInserimento
        };
    }

    public async Task<ConversazioneDettaglioDataModel?> GetDettaglioByIdAsync(int idConversazione)
    {
        var conversazione = await GetByIdAsync(idConversazione);
        if (conversazione == null)
            return null;

        var messaggi = await _messaggiRepository.GetByConversazioneAsync(idConversazione);

        return new ConversazioneDettaglioDataModel
        {
            IdConversazione = conversazione.IdConversazione,
            IdFamiglia = conversazione.IdFamiglia,
            IdUtente = conversazione.IdUtente,
            IdAssistente = conversazione.IdAssistente,
            NomeAssistente = conversazione.NomeAssistente,
            TitoloConversazione = conversazione.TitoloConversazione,
            StatoConversazione = conversazione.StatoConversazione,
            UltimoMessaggio = conversazione.UltimoMessaggio,
            ContatoreMessaggi = conversazione.ContatoreMessaggi,
            DataInserimento = conversazione.DataInserimento,
            Messaggi = messaggi.Select(m => new MessaggioDataModel
            {
                IdMessaggio = m.IdMessaggio,
                IdConversazione = m.IdConversazione,
                IdUtente = m.IdUtente,
                TipoMittente = m.TipoMittente,
                TestoMessaggio = m.TestoMessaggio,
                TipoContenuto = m.TipoContenuto,
                URLContenuto = m.URLContenuto,
                DurataContenuto = m.DurataContenuto,
                DimensioneFile = m.DimensioneFile,
                MimeType = m.MimeType,
                Sentiment = m.Sentiment,
                IntentRilevato = m.IntentRilevato,
                DataInserimento = m.DataInserimento
            }).ToList()
        };
    }

    public async Task<IEnumerable<ConversazioneDataModel>> GetByUtenteAsync(int idUtente)
    {
        var conversazioni = await _conversazioniRepository.GetByUtenteAsync(idUtente);
        var result = new List<ConversazioneDataModel>();

        foreach (var conv in conversazioni)
        {
            var assistente = await _assistentiRepository.GetByIdAsync(conv.IdAssistente);
            result.Add(new ConversazioneDataModel
            {
                IdConversazione = conv.IdConversazione,
                IdFamiglia = conv.IdFamiglia,
                IdUtente = conv.IdUtente,
                IdAssistente = conv.IdAssistente,
                NomeAssistente = assistente?.NomeAssistente ?? "Sconosciuto",
                TitoloConversazione = conv.TitoloConversazione,
                StatoConversazione = conv.StatoConversazione,
                UltimoMessaggio = conv.UltimoMessaggio,
                ContatoreMessaggi = conv.ContatoreMessaggi,
                DataInserimento = conv.DataInserimento
            });
        }

        return result;
    }

    public async Task<IEnumerable<ConversazioneDataModel>> GetByUtenteAndAssistenteAsync(int idUtente, int idAssistente)
    {
        var conversazioni = await _conversazioniRepository.GetByUtenteAndAssistenteAsync(idUtente, idAssistente);
        var result = new List<ConversazioneDataModel>();
        var assistente = await _assistentiRepository.GetByIdAsync(idAssistente);

        foreach (var conv in conversazioni)
        {
            result.Add(new ConversazioneDataModel
            {
                IdConversazione = conv.IdConversazione,
                IdFamiglia = conv.IdFamiglia,
                IdUtente = conv.IdUtente,
                IdAssistente = conv.IdAssistente,
                NomeAssistente = assistente?.NomeAssistente ?? "Sconosciuto",
                TitoloConversazione = conv.TitoloConversazione,
                StatoConversazione = conv.StatoConversazione,
                UltimoMessaggio = conv.UltimoMessaggio,
                ContatoreMessaggi = conv.ContatoreMessaggi,
                DataInserimento = conv.DataInserimento
            });
        }

        return result;
    }

    public async Task<ConversazioneDataModel> CreateAsync(CreaConversazioneDataModel model, string utenteCorrente, string? ipAddress)
    {
        // Validazioni
        var utente = await _utentiRepository.GetByIdAsync(model.IdUtente);
        if (utente == null)
            throw new InvalidOperationException("Utente non trovato");

        var assistente = await _assistentiRepository.GetByIdAsync(model.IdAssistente);
        if (assistente == null || !assistente.Attivo)
            throw new InvalidOperationException("Assistente non trovato o non attivo");

        // Crea conversazione
        var entity = new ConversazioneEntityModel
        {
            IdFamiglia = model.IdFamiglia,
            IdUtente = model.IdUtente,
            IdAssistente = model.IdAssistente,
            TitoloConversazione = model.TitoloConversazione ?? $"Conversazione con {assistente.NomeAssistente}",
            StatoConversazione = "Aperta",
            UltimoMessaggio = DateTime.Now,
            ContatoreMessaggi = 0,
            DataInserimento = DateTime.Now,
            UtenteInserimento = utenteCorrente,
            IPInserimento = ipAddress
        };

        var idConversazione = await _conversazioniRepository.CreateAsync(entity);
        entity.IdConversazione = idConversazione;

        // Se c'è un primo messaggio, crealo
        if (!string.IsNullOrWhiteSpace(model.PrimoMessaggio))
        {
            var messaggio = new MessaggioEntityModel
            {
                IdConversazione = idConversazione,
                IdUtente = model.IdUtente,
                TipoMittente = "Utente",
                TestoMessaggio = model.PrimoMessaggio,
                TipoContenuto = "Testo",
                DataInserimento = DateTime.Now,
                UtenteInserimento = utenteCorrente,
                IPInserimento = ipAddress
            };

            await _messaggiRepository.CreateAsync(messaggio);
            await _conversazioniRepository.IncrementaContatoreMessaggiAsync(idConversazione);
        }

        return new ConversazioneDataModel
        {
            IdConversazione = entity.IdConversazione,
            IdFamiglia = entity.IdFamiglia,
            IdUtente = entity.IdUtente,
            IdAssistente = entity.IdAssistente,
            NomeAssistente = assistente.NomeAssistente,
            TitoloConversazione = entity.TitoloConversazione,
            StatoConversazione = entity.StatoConversazione,
            UltimoMessaggio = entity.UltimoMessaggio,
            ContatoreMessaggi = entity.ContatoreMessaggi,
            DataInserimento = entity.DataInserimento
        };
    }

    public async Task<bool> ChiudiConversazioneAsync(int idConversazione)
    {
        return await _conversazioniRepository.UpdateStatoAsync(idConversazione, "Chiusa");
    }

    public async Task<bool> ArchivaConversazioneAsync(int idConversazione)
    {
        return await _conversazioniRepository.UpdateStatoAsync(idConversazione, "Archiviata");
    }
}
