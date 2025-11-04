using NugoloFamily.Core.Models.Entities;

namespace NugoloFamily.Core.Interfaces.Repositories;

/// <summary>
/// Interfaccia repository per la gestione degli Assistenti attivati per le Famiglie
/// </summary>
public interface IAssistentiFamiglieRepository
{
    Task<AssistenteFamigliaEntityModel?> GetByIdAsync(int idAssistenteFamiglia);
    Task<IEnumerable<AssistenteFamigliaEntityModel>> GetByFamigliaAsync(int idFamiglia);
    Task<IEnumerable<AssistenteFamigliaEntityModel>> GetAttiviByFamigliaAsync(int idFamiglia);
    Task<AssistenteFamigliaEntityModel?> GetByFamigliaAndAssistenteAsync(int idFamiglia, int idAssistente);
    Task<int> CreateAsync(AssistenteFamigliaEntityModel assistenteFamiglia);
    Task<bool> UpdateAsync(AssistenteFamigliaEntityModel assistenteFamiglia);
    Task<bool> DeleteAsync(int idAssistenteFamiglia);
    Task<bool> AttivaDisattivaAsync(int idAssistenteFamiglia, bool attivo);
    Task<bool> ExistsAsync(int idFamiglia, int idAssistente);
}
