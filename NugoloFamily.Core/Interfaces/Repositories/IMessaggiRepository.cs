using NugoloFamily.Core.Models.Entities;

namespace NugoloFamily.Core.Interfaces.Repositories;

/// <summary>
/// Interfaccia repository per la gestione dei Messaggi
/// </summary>
public interface IMessaggiRepository
{
    Task<MessaggioEntityModel?> GetByIdAsync(long idMessaggio);
    Task<IEnumerable<MessaggioEntityModel>> GetByConversazioneAsync(int idConversazione);
    Task<IEnumerable<MessaggioEntityModel>> GetByConversazionePaginatiAsync(int idConversazione, int pagina, int dimensionePagina);
    Task<IEnumerable<MessaggioEntityModel>> GetByUtenteAsync(int idUtente);
    Task<IEnumerable<MessaggioEntityModel>> GetByTipoContenutoAsync(string tipoContenuto);
    Task<long> CreateAsync(MessaggioEntityModel messaggio);
    Task<bool> UpdateAsync(MessaggioEntityModel messaggio);
    Task<bool> DeleteAsync(long idMessaggio);
    Task<int> CountByConversazioneAsync(int idConversazione);
}
