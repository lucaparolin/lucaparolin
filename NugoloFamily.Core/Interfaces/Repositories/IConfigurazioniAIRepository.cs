using NugoloFamily.Core.Models.Entities;

namespace NugoloFamily.Core.Interfaces.Repositories;

/// <summary>
/// Interfaccia repository per la gestione delle Configurazioni AI
/// </summary>
public interface IConfigurazioniAIRepository
{
    Task<ConfigurazioneAIEntityModel?> GetByIdAsync(int idConfigurazioneAI);
    Task<IEnumerable<ConfigurazioneAIEntityModel>> GetByFamigliaAsync(int idFamiglia);
    Task<ConfigurazioneAIEntityModel?> GetPredefinitaByFamigliaAsync(int idFamiglia);
    Task<ConfigurazioneAIEntityModel?> GetByFamigliaAndAssistenteAsync(int idFamiglia, int idAssistente);
    Task<IEnumerable<ConfigurazioneAIEntityModel>> GetByProviderAsync(string providerAI);
    Task<int> CreateAsync(ConfigurazioneAIEntityModel configurazioneAI);
    Task<bool> UpdateAsync(ConfigurazioneAIEntityModel configurazioneAI);
    Task<bool> DeleteAsync(int idConfigurazioneAI);
    Task<bool> ImpostaPredefinitaAsync(int idFamiglia, int idConfigurazioneAI);
}
