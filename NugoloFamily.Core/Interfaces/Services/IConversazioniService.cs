using NugoloFamily.Core.Models.DTOs;

namespace NugoloFamily.Core.Interfaces.Services;

/// <summary>
/// Interfaccia per il servizio di gestione delle Conversazioni
/// </summary>
public interface IConversazioniService
{
    Task<ConversazioneDataModel?> GetByIdAsync(int idConversazione);
    Task<ConversazioneDettaglioDataModel?> GetDettaglioByIdAsync(int idConversazione);
    Task<IEnumerable<ConversazioneDataModel>> GetByUtenteAsync(int idUtente);
    Task<IEnumerable<ConversazioneDataModel>> GetByUtenteAndAssistenteAsync(int idUtente, int idAssistente);
    Task<ConversazioneDataModel> CreateAsync(CreaConversazioneDataModel model, string utenteCorrente, string? ipAddress);
    Task<bool> ChiudiConversazioneAsync(int idConversazione);
    Task<bool> ArchivaConversazioneAsync(int idConversazione);
}
