using NugoloFamily.Core.Models.DTOs;

namespace NugoloFamily.Core.Interfaces.Services;

/// <summary>
/// Interfaccia per il servizio di gestione delle Famiglie
/// </summary>
public interface IFamiglieService
{
    Task<FamigliaDataModel?> GetByIdAsync(int idFamiglia);
    Task<FamigliaDataModel?> GetByCodiceUnivocoAsync(string codiceUnivoco);
    Task<IEnumerable<FamigliaDataModel>> GetAllAsync();
    Task<IEnumerable<FamigliaDataModel>> GetAttiviAsync();
    Task<FamigliaDataModel> CreateAsync(CreaFamigliaDataModel model, string utenteCorrente, string? ipAddress);
    Task<bool> UpdateAsync(AggiornaFamigliaDataModel model, string utenteCorrente, string? ipAddress);
    Task<bool> DeleteAsync(int idFamiglia);
    Task<bool> ExistsByEmailAsync(string email);
    Task<bool> CanAddUserAsync(int idFamiglia);
}
