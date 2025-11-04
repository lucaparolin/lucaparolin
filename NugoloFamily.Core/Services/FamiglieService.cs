using NugoloFamily.Core.Interfaces.Repositories;
using NugoloFamily.Core.Interfaces.Services;
using NugoloFamily.Core.Models.DTOs;
using NugoloFamily.Core.Models.Entities;

namespace NugoloFamily.Core.Services;

/// <summary>
/// Servizio per la gestione delle Famiglie con logica business
/// </summary>
public class FamiglieService : IFamiglieService
{
    private readonly IFamiglieRepository _famiglieRepository;
    private readonly IUtentiRepository _utentiRepository;

    public FamiglieService(
        IFamiglieRepository famiglieRepository,
        IUtentiRepository utentiRepository)
    {
        _famiglieRepository = famiglieRepository;
        _utentiRepository = utentiRepository;
    }

    public async Task<FamigliaDataModel?> GetByIdAsync(int idFamiglia)
    {
        var famiglia = await _famiglieRepository.GetByIdAsync(idFamiglia);
        return famiglia != null ? MapToDataModel(famiglia) : null;
    }

    public async Task<FamigliaDataModel?> GetByCodiceUnivocoAsync(string codiceUnivoco)
    {
        var famiglia = await _famiglieRepository.GetByCodiceUnivocoAsync(codiceUnivoco);
        return famiglia != null ? MapToDataModel(famiglia) : null;
    }

    public async Task<IEnumerable<FamigliaDataModel>> GetAllAsync()
    {
        var famiglie = await _famiglieRepository.GetAllAsync();
        return famiglie.Select(MapToDataModel);
    }

    public async Task<IEnumerable<FamigliaDataModel>> GetAttiviAsync()
    {
        var famiglie = await _famiglieRepository.GetAttiviAsync();
        return famiglie.Select(MapToDataModel);
    }

    public async Task<FamigliaDataModel> CreateAsync(CreaFamigliaDataModel model, string utenteCorrente, string? ipAddress)
    {
        // Validazioni
        if (string.IsNullOrWhiteSpace(model.NomeFamiglia))
            throw new ArgumentException("Il nome della famiglia è obbligatorio");

        if (string.IsNullOrWhiteSpace(model.Email))
            throw new ArgumentException("L'email è obbligatoria");

        if (!IsValidEmail(model.Email))
            throw new ArgumentException("L'email non è valida");

        // Verifica email duplicata
        if (await _famiglieRepository.ExistsByEmailAsync(model.Email))
            throw new InvalidOperationException("Esiste già una famiglia con questa email");

        // Genera codice univoco
        var codiceUnivoco = GeneraCodiceUnivoco(model.NomeFamiglia);
        var tentativi = 0;
        while (await _famiglieRepository.ExistsByCodiceUnivocoAsync(codiceUnivoco) && tentativi < 10)
        {
            codiceUnivoco = GeneraCodiceUnivoco(model.NomeFamiglia);
            tentativi++;
        }

        if (tentativi >= 10)
            throw new InvalidOperationException("Impossibile generare un codice univoco");

        // Determina limite utenti in base al piano
        var limiteUtenti = model.PianoAbbonamento?.ToLower() switch
        {
            "free" => 3,
            "basic" => 5,
            "premium" => 10,
            "enterprise" => 50,
            _ => 5
        };

        var entity = new FamigliaEntityModel
        {
            NomeFamiglia = model.NomeFamiglia.Trim(),
            CodiceUnivoco = codiceUnivoco,
            Email = model.Email.Trim().ToLower(),
            Telefono = model.Telefono?.Trim(),
            Indirizzo = model.Indirizzo?.Trim(),
            StatoAttivazione = "Attivo",
            PianoAbbonamento = model.PianoAbbonamento ?? "Free",
            DataScadenzaAbbonamento = CalculateSubscriptionExpiration(model.PianoAbbonamento),
            LimiteUtenti = limiteUtenti,
            DataInserimento = DateTime.Now,
            UtenteInserimento = utenteCorrente,
            IPInserimento = ipAddress
        };

        var idFamiglia = await _famiglieRepository.CreateAsync(entity);
        entity.IdFamiglia = idFamiglia;

        return MapToDataModel(entity);
    }

    public async Task<bool> UpdateAsync(AggiornaFamigliaDataModel model, string utenteCorrente, string? ipAddress)
    {
        var famiglia = await _famiglieRepository.GetByIdAsync(model.IdFamiglia);
        if (famiglia == null)
            throw new InvalidOperationException($"Famiglia con ID {model.IdFamiglia} non trovata");

        // Aggiorna solo i campi forniti
        if (!string.IsNullOrWhiteSpace(model.NomeFamiglia))
            famiglia.NomeFamiglia = model.NomeFamiglia.Trim();

        if (!string.IsNullOrWhiteSpace(model.Email))
        {
            if (!IsValidEmail(model.Email))
                throw new ArgumentException("L'email non è valida");
            famiglia.Email = model.Email.Trim().ToLower();
        }

        if (model.Telefono != null)
            famiglia.Telefono = model.Telefono.Trim();

        if (model.Indirizzo != null)
            famiglia.Indirizzo = model.Indirizzo.Trim();

        if (!string.IsNullOrWhiteSpace(model.StatoAttivazione))
            famiglia.StatoAttivazione = model.StatoAttivazione;

        if (model.DataScadenzaAbbonamento.HasValue)
            famiglia.DataScadenzaAbbonamento = model.DataScadenzaAbbonamento;

        if (!string.IsNullOrWhiteSpace(model.PianoAbbonamento))
        {
            famiglia.PianoAbbonamento = model.PianoAbbonamento;
            // Aggiorna limite utenti in base al piano
            famiglia.LimiteUtenti = model.PianoAbbonamento.ToLower() switch
            {
                "free" => 3,
                "basic" => 5,
                "premium" => 10,
                "enterprise" => 50,
                _ => famiglia.LimiteUtenti
            };
        }

        famiglia.DataModifica = DateTime.Now;
        famiglia.UtenteModifica = utenteCorrente;
        famiglia.IPModifica = ipAddress;

        return await _famiglieRepository.UpdateAsync(famiglia);
    }

    public async Task<bool> DeleteAsync(int idFamiglia)
    {
        var famiglia = await _famiglieRepository.GetByIdAsync(idFamiglia);
        if (famiglia == null)
            throw new InvalidOperationException($"Famiglia con ID {idFamiglia} non trovata");

        // Verifica che non ci siano utenti attivi
        var contatoreUtenti = await _utentiRepository.CountByFamigliaAsync(idFamiglia);
        if (contatoreUtenti > 0)
            throw new InvalidOperationException("Impossibile eliminare una famiglia con utenti attivi");

        return await _famiglieRepository.DeleteAsync(idFamiglia);
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        return await _famiglieRepository.ExistsByEmailAsync(email.Trim().ToLower());
    }

    public async Task<bool> CanAddUserAsync(int idFamiglia)
    {
        var famiglia = await _famiglieRepository.GetByIdAsync(idFamiglia);
        if (famiglia == null)
            return false;

        var contatoreUtenti = await _utentiRepository.CountByFamigliaAsync(idFamiglia);
        return contatoreUtenti < famiglia.LimiteUtenti;
    }

    // Helper methods

    private FamigliaDataModel MapToDataModel(FamigliaEntityModel entity)
    {
        return new FamigliaDataModel
        {
            IdFamiglia = entity.IdFamiglia,
            NomeFamiglia = entity.NomeFamiglia,
            CodiceUnivoco = entity.CodiceUnivoco,
            Email = entity.Email,
            Telefono = entity.Telefono,
            Indirizzo = entity.Indirizzo,
            StatoAttivazione = entity.StatoAttivazione,
            DataScadenzaAbbonamento = entity.DataScadenzaAbbonamento,
            PianoAbbonamento = entity.PianoAbbonamento,
            LimiteUtenti = entity.LimiteUtenti,
            DataInserimento = entity.DataInserimento
        };
    }

    private string GeneraCodiceUnivoco(string nomeFamiglia)
    {
        var timestamp = DateTime.Now.ToString("yyyyMMdd");
        var random = new Random().Next(1000, 9999);
        var nomeNormalizzato = new string(nomeFamiglia
            .Where(c => char.IsLetterOrDigit(c))
            .Take(5)
            .ToArray())
            .ToUpper();

        if (string.IsNullOrEmpty(nomeNormalizzato))
            nomeNormalizzato = "FAM";

        return $"FAM-{nomeNormalizzato}-{timestamp}-{random}";
    }

    private DateTime? CalculateSubscriptionExpiration(string? piano)
    {
        if (string.IsNullOrEmpty(piano) || piano.ToLower() == "free")
            return null;

        // Abbonamenti a pagamento hanno scadenza a 1 anno
        return DateTime.Now.AddYears(1);
    }

    private bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}
