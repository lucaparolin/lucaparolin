using NugoloFamily.Core.Models.DTOs;

namespace NugoloFamily.Core.Interfaces.Services;

/// <summary>
/// Interfaccia per il servizio di gestione dei Messaggi
/// </summary>
public interface IMessaggiService
{
    Task<MessaggioDataModel?> GetByIdAsync(long idMessaggio);
    Task<IEnumerable<MessaggioDataModel>> GetByConversazioneAsync(int idConversazione);
    Task<IEnumerable<MessaggioDataModel>> GetByConversazionePaginatiAsync(int idConversazione, int pagina, int dimensionePagina);
    Task<MessaggioDataModel> InviaMessaggioAsync(InviaMessaggioDataModel model, string utenteCorrente, string? ipAddress);
    Task<RispostaAssistenteDataModel> ElaboraRispostaAssistenteAsync(int idConversazione, long idMessaggioUtente);
}
