using NugoloFamily.Core.Models.DTOs;

namespace NugoloFamily.Core.Interfaces.Services;

/// <summary>
/// Interfaccia per il servizio di gestione degli Utenti
/// </summary>
public interface IUtentiService
{
    Task<UtenteDataModel?> GetByIdAsync(int idUtente);
    Task<UtenteDataModel?> GetByUsernameAsync(string username);
    Task<IEnumerable<UtenteDataModel>> GetByFamigliaAsync(int idFamiglia);
    Task<IEnumerable<UtenteDataModel>> GetAttiviByFamigliaAsync(int idFamiglia);
    Task<UtenteDataModel> CreateAsync(RegistraUtenteDataModel model, string utenteCorrente, string? ipAddress);
    Task<bool> UpdateAsync(AggiornaUtenteDataModel model, string utenteCorrente, string? ipAddress);
    Task<bool> DeleteAsync(int idUtente);
    Task<LoginRispostaDataModel?> LoginAsync(LoginDataModel model, string? ipAddress);
    Task<bool> UpdatePasswordAsync(int idUtente, string vecchiaPassword, string nuovaPassword);
    Task<bool> ValidatePasswordAsync(string username, string password);
}
