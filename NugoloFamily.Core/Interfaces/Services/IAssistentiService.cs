using NugoloFamily.Core.Models.DTOs;

namespace NugoloFamily.Core.Interfaces.Services;

/// <summary>
/// Interfaccia per il servizio di gestione degli Assistenti
/// </summary>
public interface IAssistentiService
{
    Task<IEnumerable<AssistenteDataModel>> GetAllAsync();
    Task<IEnumerable<AssistenteDataModel>> GetAttiviAsync();
    Task<AssistenteDataModel?> GetByIdAsync(int idAssistente);
    Task<AssistenteDataModel?> GetByCodiceAsync(string codiceAssistente);
    Task<IEnumerable<AssistenteDataModel>> GetByCategoriaAsync(string categoria);
    Task<IEnumerable<AssistenteFamigliaDataModel>> GetAssistentiAttiviPerFamigliaAsync(int idFamiglia);
    Task<bool> AttivaAssistentePerFamigliaAsync(AttivaAssistenteDataModel model, string utenteCorrente, string? ipAddress);
    Task<bool> DisattivaAssistentePerFamigliaAsync(int idFamiglia, int idAssistente);
}
