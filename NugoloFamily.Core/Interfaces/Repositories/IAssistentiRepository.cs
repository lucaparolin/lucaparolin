using NugoloFamily.Core.Models.Entities;

namespace NugoloFamily.Core.Interfaces.Repositories;

/// <summary>
/// Interfaccia repository per la gestione degli Assistenti
/// </summary>
public interface IAssistentiRepository
{
    Task<AssistenteEntityModel?> GetByIdAsync(int idAssistente);
    Task<AssistenteEntityModel?> GetByCodiceAsync(string codiceAssistente);
    Task<IEnumerable<AssistenteEntityModel>> GetAllAsync();
    Task<IEnumerable<AssistenteEntityModel>> GetAttiviAsync();
    Task<IEnumerable<AssistenteEntityModel>> GetByCategoriaAsync(string categoria);
    Task<int> CreateAsync(AssistenteEntityModel assistente);
    Task<bool> UpdateAsync(AssistenteEntityModel assistente);
    Task<bool> DeleteAsync(int idAssistente);
}
