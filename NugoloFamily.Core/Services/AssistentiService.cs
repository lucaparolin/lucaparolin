using NugoloFamily.Core.Interfaces.Repositories;
using NugoloFamily.Core.Interfaces.Services;
using NugoloFamily.Core.Models.DTOs;
using NugoloFamily.Core.Models.Entities;

namespace NugoloFamily.Core.Services;

/// <summary>
/// Servizio per la gestione degli Assistenti
/// </summary>
public class AssistentiService : IAssistentiService
{
    private readonly IAssistentiRepository _assistentiRepository;
    private readonly IAssistentiFamiglieRepository _assistentiFamiglieRepository;

    public AssistentiService(
        IAssistentiRepository assistentiRepository,
        IAssistentiFamiglieRepository assistentiFamiglieRepository)
    {
        _assistentiRepository = assistentiRepository;
        _assistentiFamiglieRepository = assistentiFamiglieRepository;
    }

    public async Task<IEnumerable<AssistenteDataModel>> GetAllAsync()
    {
        var assistenti = await _assistentiRepository.GetAllAsync();
        return assistenti.Select(MapToDataModel);
    }

    public async Task<IEnumerable<AssistenteDataModel>> GetAttiviAsync()
    {
        var assistenti = await _assistentiRepository.GetAttiviAsync();
        return assistenti.Select(MapToDataModel);
    }

    public async Task<AssistenteDataModel?> GetByIdAsync(int idAssistente)
    {
        var assistente = await _assistentiRepository.GetByIdAsync(idAssistente);
        return assistente != null ? MapToDataModel(assistente) : null;
    }

    public async Task<AssistenteDataModel?> GetByCodiceAsync(string codiceAssistente)
    {
        var assistente = await _assistentiRepository.GetByCodiceAsync(codiceAssistente);
        return assistente != null ? MapToDataModel(assistente) : null;
    }

    public async Task<IEnumerable<AssistenteDataModel>> GetByCategoriaAsync(string categoria)
    {
        var assistenti = await _assistentiRepository.GetByCategoriaAsync(categoria);
        return assistenti.Select(MapToDataModel);
    }

    public async Task<IEnumerable<AssistenteFamigliaDataModel>> GetAssistentiAttiviPerFamigliaAsync(int idFamiglia)
    {
        var assistentiFamiglia = await _assistentiFamiglieRepository.GetAttiviByFamigliaAsync(idFamiglia);
        var result = new List<AssistenteFamigliaDataModel>();

        foreach (var af in assistentiFamiglia)
        {
            var assistente = await _assistentiRepository.GetByIdAsync(af.IdAssistente);
            if (assistente != null)
            {
                result.Add(new AssistenteFamigliaDataModel
                {
                    IdAssistenteFamiglia = af.IdAssistenteFamiglia,
                    IdFamiglia = af.IdFamiglia,
                    IdAssistente = af.IdAssistente,
                    NomeAssistente = assistente.NomeAssistente,
                    CodiceAssistente = assistente.CodiceAssistente,
                    Descrizione = assistente.Descrizione,
                    Categoria = assistente.Categoria,
                    Attivo = af.Attivo,
                    ConfigurazionePersonalizzata = af.ConfigurazionePersonalizzata,
                    DataAttivazione = af.DataAttivazione
                });
            }
        }

        return result;
    }

    public async Task<bool> AttivaAssistentePerFamigliaAsync(AttivaAssistenteDataModel model, string utenteCorrente, string? ipAddress)
    {
        // Verifica che l'assistente esista ed sia attivo
        var assistente = await _assistentiRepository.GetByIdAsync(model.IdAssistente);
        if (assistente == null || !assistente.Attivo)
            throw new InvalidOperationException("Assistente non trovato o non attivo");

        // Verifica se già attivato
        var esistente = await _assistentiFamiglieRepository.GetByFamigliaAndAssistenteAsync(model.IdFamiglia, model.IdAssistente);
        if (esistente != null)
        {
            // Se già esiste ma è disattivato, riattivalo
            if (!esistente.Attivo)
            {
                return await _assistentiFamiglieRepository.AttivaDisattivaAsync(esistente.IdAssistenteFamiglia, true);
            }
            throw new InvalidOperationException("Assistente già attivato per questa famiglia");
        }

        // Crea nuova associazione
        var entity = new AssistenteFamigliaEntityModel
        {
            IdFamiglia = model.IdFamiglia,
            IdAssistente = model.IdAssistente,
            Attivo = true,
            ConfigurazionePersonalizzata = model.ConfigurazionePersonalizzata,
            DataAttivazione = DateTime.Now,
            DataInserimento = DateTime.Now,
            UtenteInserimento = utenteCorrente,
            IPInserimento = ipAddress
        };

        await _assistentiFamiglieRepository.CreateAsync(entity);
        return true;
    }

    public async Task<bool> DisattivaAssistentePerFamigliaAsync(int idFamiglia, int idAssistente)
    {
        var associazione = await _assistentiFamiglieRepository.GetByFamigliaAndAssistenteAsync(idFamiglia, idAssistente);
        if (associazione == null)
            throw new InvalidOperationException("Associazione non trovata");

        return await _assistentiFamiglieRepository.AttivaDisattivaAsync(associazione.IdAssistenteFamiglia, false);
    }

    // Helper methods

    private AssistenteDataModel MapToDataModel(AssistenteEntityModel entity)
    {
        return new AssistenteDataModel
        {
            IdAssistente = entity.IdAssistente,
            NomeAssistente = entity.NomeAssistente,
            CodiceAssistente = entity.CodiceAssistente,
            Descrizione = entity.Descrizione,
            Icona = entity.Icona,
            Categoria = entity.Categoria,
            Attivo = entity.Attivo,
            Ordinamento = entity.Ordinamento,
            RichiedeConfigurazione = entity.RichiedeConfigurazione
        };
    }
}
