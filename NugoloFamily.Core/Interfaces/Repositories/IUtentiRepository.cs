using NugoloFamily.Core.Models.Entities;

namespace NugoloFamily.Core.Interfaces.Repositories;

/// <summary>
/// Interfaccia repository per la gestione degli Utenti
/// </summary>
public interface IUtentiRepository
{
    Task<UtenteEntityModel?> GetByIdAsync(int idUtente);
    Task<UtenteEntityModel?> GetByUsernameAsync(string username);
    Task<UtenteEntityModel?> GetByEmailAsync(string email);
    Task<IEnumerable<UtenteEntityModel>> GetByFamigliaAsync(int idFamiglia);
    Task<IEnumerable<UtenteEntityModel>> GetAttiviByFamigliaAsync(int idFamiglia);
    Task<int> CreateAsync(UtenteEntityModel utente);
    Task<bool> UpdateAsync(UtenteEntityModel utente);
    Task<bool> DeleteAsync(int idUtente);
    Task<bool> UpdateUltimoAccessoAsync(int idUtente);
    Task<bool> UpdateTokenRefreshAsync(int idUtente, string tokenRefresh, DateTime dataScadenza);
    Task<bool> ExistsByUsernameAsync(string username);
    Task<bool> ExistsByEmailAsync(string email);
    Task<int> CountByFamigliaAsync(int idFamiglia);
}
