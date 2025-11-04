using NugoloFamily.Core.Models.Entities;

namespace NugoloFamily.Core.Interfaces.Repositories;

/// <summary>
/// Interfaccia repository per la gestione delle Conversazioni
/// </summary>
public interface IConversazioniRepository
{
    Task<ConversazioneEntityModel?> GetByIdAsync(int idConversazione);
    Task<IEnumerable<ConversazioneEntityModel>> GetByFamigliaAsync(int idFamiglia);
    Task<IEnumerable<ConversazioneEntityModel>> GetByUtenteAsync(int idUtente);
    Task<IEnumerable<ConversazioneEntityModel>> GetByAssistenteAsync(int idAssistente);
    Task<IEnumerable<ConversazioneEntityModel>> GetByUtenteAndAssistenteAsync(int idUtente, int idAssistente);
    Task<int> CreateAsync(ConversazioneEntityModel conversazione);
    Task<bool> UpdateAsync(ConversazioneEntityModel conversazione);
    Task<bool> DeleteAsync(int idConversazione);
    Task<bool> UpdateUltimoMessaggioAsync(int idConversazione);
    Task<bool> UpdateStatoAsync(int idConversazione, string stato);
    Task<bool> IncrementaContatoreMessaggiAsync(int idConversazione);
}
