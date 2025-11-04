using NugoloFamily.Core.Models.Entities;

namespace NugoloFamily.Core.Interfaces.Repositories;

/// <summary>
/// Interfaccia repository per la gestione delle Famiglie
/// </summary>
public interface IFamiglieRepository
{
    Task<FamigliaEntityModel?> GetByIdAsync(int idFamiglia);
    Task<FamigliaEntityModel?> GetByCodiceUnivocoAsync(string codiceUnivoco);
    Task<IEnumerable<FamigliaEntityModel>> GetAllAsync();
    Task<IEnumerable<FamigliaEntityModel>> GetAttiviAsync();
    Task<int> CreateAsync(FamigliaEntityModel famiglia);
    Task<bool> UpdateAsync(FamigliaEntityModel famiglia);
    Task<bool> DeleteAsync(int idFamiglia);
    Task<bool> ExistsByEmailAsync(string email);
    Task<bool> ExistsByCodiceUnivocoAsync(string codiceUnivoco);
}
