using NugoloFamily.Core.Interfaces.Repositories;
using NugoloFamily.Core.Interfaces.Services;
using NugoloFamily.Core.Models.DTOs;
using NugoloFamily.Core.Models.Entities;
using NugoloFamily.Shared.Helpers;

namespace NugoloFamily.Core.Services;

/// <summary>
/// Servizio per la gestione degli Utenti con autenticazione
/// </summary>
public class UtentiService : IUtentiService
{
    private readonly IUtentiRepository _utentiRepository;
    private readonly IFamiglieRepository _famiglieRepository;
    private readonly JwtHelper _jwtHelper;

    public UtentiService(
        IUtentiRepository utentiRepository,
        IFamiglieRepository famiglieRepository,
        JwtHelper jwtHelper)
    {
        _utentiRepository = utentiRepository;
        _famiglieRepository = famiglieRepository;
        _jwtHelper = jwtHelper;
    }

    public async Task<UtenteDataModel?> GetByIdAsync(int idUtente)
    {
        var utente = await _utentiRepository.GetByIdAsync(idUtente);
        return utente != null ? MapToDataModel(utente) : null;
    }

    public async Task<UtenteDataModel?> GetByUsernameAsync(string username)
    {
        var utente = await _utentiRepository.GetByUsernameAsync(username);
        return utente != null ? MapToDataModel(utente) : null;
    }

    public async Task<IEnumerable<UtenteDataModel>> GetByFamigliaAsync(int idFamiglia)
    {
        var utenti = await _utentiRepository.GetByFamigliaAsync(idFamiglia);
        return utenti.Select(MapToDataModel);
    }

    public async Task<IEnumerable<UtenteDataModel>> GetAttiviByFamigliaAsync(int idFamiglia)
    {
        var utenti = await _utentiRepository.GetAttiviByFamigliaAsync(idFamiglia);
        return utenti.Select(MapToDataModel);
    }

    public async Task<UtenteDataModel> CreateAsync(RegistraUtenteDataModel model, string utenteCorrente, string? ipAddress)
    {
        // Validazioni
        if (string.IsNullOrWhiteSpace(model.Username))
            throw new ArgumentException("L'username è obbligatorio");

        if (string.IsNullOrWhiteSpace(model.Email))
            throw new ArgumentException("L'email è obbligatoria");

        if (string.IsNullOrWhiteSpace(model.Password))
            throw new ArgumentException("La password è obbligatoria");

        if (!PasswordHelper.IsPasswordStrong(model.Password))
            throw new ArgumentException("La password deve contenere almeno 8 caratteri, una maiuscola, una minuscola, un numero e un carattere speciale");

        // Verifica famiglia esistente
        var famiglia = await _famiglieRepository.GetByIdAsync(model.IdFamiglia);
        if (famiglia == null)
            throw new InvalidOperationException($"Famiglia con ID {model.IdFamiglia} non trovata");

        // Verifica limite utenti
        var contatoreUtenti = await _utentiRepository.CountByFamigliaAsync(model.IdFamiglia);
        if (contatoreUtenti >= famiglia.LimiteUtenti)
            throw new InvalidOperationException($"Limite massimo di {famiglia.LimiteUtenti} utenti raggiunto per questa famiglia");

        // Verifica username duplicato
        if (await _utentiRepository.ExistsByUsernameAsync(model.Username))
            throw new InvalidOperationException("Username già in uso");

        // Verifica email duplicata
        if (await _utentiRepository.ExistsByEmailAsync(model.Email))
            throw new InvalidOperationException("Email già in uso");

        // Genera salt e hash password
        var salt = PasswordHelper.GenerateSalt();
        var passwordHash = PasswordHelper.HashPassword(model.Password, salt);

        var entity = new UtenteEntityModel
        {
            IdFamiglia = model.IdFamiglia,
            Username = model.Username.Trim().ToLower(),
            Email = model.Email.Trim().ToLower(),
            PasswordHash = passwordHash,
            Salt = salt,
            Nome = model.Nome.Trim(),
            Cognome = model.Cognome.Trim(),
            DataNascita = model.DataNascita,
            Ruolo = model.Ruolo,
            Attivo = true,
            DataInserimento = DateTime.Now,
            UtenteInserimento = utenteCorrente,
            IPInserimento = ipAddress
        };

        var idUtente = await _utentiRepository.CreateAsync(entity);
        entity.IdUtente = idUtente;

        return MapToDataModel(entity);
    }

    public async Task<bool> UpdateAsync(AggiornaUtenteDataModel model, string utenteCorrente, string? ipAddress)
    {
        var utente = await _utentiRepository.GetByIdAsync(model.IdUtente);
        if (utente == null)
            throw new InvalidOperationException($"Utente con ID {model.IdUtente} non trovato");

        // Aggiorna solo i campi forniti
        if (!string.IsNullOrWhiteSpace(model.Email))
        {
            // Verifica se email già usata da altro utente
            var utenteConEmail = await _utentiRepository.GetByEmailAsync(model.Email);
            if (utenteConEmail != null && utenteConEmail.IdUtente != model.IdUtente)
                throw new InvalidOperationException("Email già in uso");

            utente.Email = model.Email.Trim().ToLower();
        }

        if (!string.IsNullOrWhiteSpace(model.Nome))
            utente.Nome = model.Nome.Trim();

        if (!string.IsNullOrWhiteSpace(model.Cognome))
            utente.Cognome = model.Cognome.Trim();

        if (model.DataNascita.HasValue)
            utente.DataNascita = model.DataNascita;

        if (model.Attivo.HasValue)
            utente.Attivo = model.Attivo.Value;

        utente.DataModifica = DateTime.Now;
        utente.UtenteModifica = utenteCorrente;
        utente.IPModifica = ipAddress;

        return await _utentiRepository.UpdateAsync(utente);
    }

    public async Task<bool> DeleteAsync(int idUtente)
    {
        var utente = await _utentiRepository.GetByIdAsync(idUtente);
        if (utente == null)
            throw new InvalidOperationException($"Utente con ID {idUtente} non trovato");

        return await _utentiRepository.DeleteAsync(idUtente);
    }

    public async Task<LoginRispostaDataModel?> LoginAsync(LoginDataModel model, string? ipAddress)
    {
        if (string.IsNullOrWhiteSpace(model.Username) || string.IsNullOrWhiteSpace(model.Password))
            return null;

        // Cerca utente
        var utente = await _utentiRepository.GetByUsernameAsync(model.Username.Trim().ToLower());
        if (utente == null || !utente.Attivo)
            return null;

        // Verifica password
        if (!PasswordHelper.VerifyPassword(model.Password, utente.Salt, utente.PasswordHash))
            return null;

        // Genera token
        var token = _jwtHelper.GenerateToken(utente.IdUtente, utente.IdFamiglia, utente.Username, utente.Ruolo);
        var refreshToken = _jwtHelper.GenerateRefreshToken();
        var dataScadenzaToken = DateTime.Now.AddDays(7); // Refresh token valido 7 giorni

        // Salva refresh token
        await _utentiRepository.UpdateTokenRefreshAsync(utente.IdUtente, refreshToken, dataScadenzaToken);

        // Aggiorna ultimo accesso
        await _utentiRepository.UpdateUltimoAccessoAsync(utente.IdUtente);

        return new LoginRispostaDataModel
        {
            IdUtente = utente.IdUtente,
            IdFamiglia = utente.IdFamiglia,
            Username = utente.Username,
            NomeCompleto = $"{utente.Nome} {utente.Cognome}",
            Ruolo = utente.Ruolo,
            Token = token,
            RefreshToken = refreshToken,
            DataScadenzaToken = dataScadenzaToken
        };
    }

    public async Task<bool> UpdatePasswordAsync(int idUtente, string vecchiaPassword, string nuovaPassword)
    {
        var utente = await _utentiRepository.GetByIdAsync(idUtente);
        if (utente == null)
            throw new InvalidOperationException($"Utente con ID {idUtente} non trovato");

        // Verifica vecchia password
        if (!PasswordHelper.VerifyPassword(vecchiaPassword, utente.Salt, utente.PasswordHash))
            throw new InvalidOperationException("La vecchia password non è corretta");

        // Valida nuova password
        if (!PasswordHelper.IsPasswordStrong(nuovaPassword))
            throw new ArgumentException("La nuova password deve contenere almeno 8 caratteri, una maiuscola, una minuscola, un numero e un carattere speciale");

        // Genera nuovo salt e hash
        var nuovoSalt = PasswordHelper.GenerateSalt();
        var nuovoHash = PasswordHelper.HashPassword(nuovaPassword, nuovoSalt);

        utente.Salt = nuovoSalt;
        utente.PasswordHash = nuovoHash;
        utente.DataModifica = DateTime.Now;

        return await _utentiRepository.UpdateAsync(utente);
    }

    public async Task<bool> ValidatePasswordAsync(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            return false;

        var utente = await _utentiRepository.GetByUsernameAsync(username.Trim().ToLower());
        if (utente == null || !utente.Attivo)
            return false;

        return PasswordHelper.VerifyPassword(password, utente.Salt, utente.PasswordHash);
    }

    // Helper methods

    private UtenteDataModel MapToDataModel(UtenteEntityModel entity)
    {
        return new UtenteDataModel
        {
            IdUtente = entity.IdUtente,
            IdFamiglia = entity.IdFamiglia,
            Username = entity.Username,
            Email = entity.Email,
            Nome = entity.Nome,
            Cognome = entity.Cognome,
            DataNascita = entity.DataNascita,
            Ruolo = entity.Ruolo,
            Attivo = entity.Attivo,
            UltimoAccesso = entity.UltimoAccesso,
            DataInserimento = entity.DataInserimento
        };
    }
}
