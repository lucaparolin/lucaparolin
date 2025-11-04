using NugoloFamily.Core.Models.Entities;

namespace NugoloFamily.Core.Interfaces.Repositories;

/// <summary>
/// Interfaccia repository per la gestione dei Log Attività
/// </summary>
public interface ILogAttivitaRepository
{
    Task<LogAttivitaEntityModel?> GetByIdAsync(long idLog);
    Task<IEnumerable<LogAttivitaEntityModel>> GetByFamigliaAsync(int idFamiglia, int limite);
    Task<IEnumerable<LogAttivitaEntityModel>> GetByUtenteAsync(int idUtente, int limite);
    Task<IEnumerable<LogAttivitaEntityModel>> GetByTipoAttivitaAsync(string tipoAttivita, int limite);
    Task<IEnumerable<LogAttivitaEntityModel>> GetByPeriodoAsync(DateTime dataInizio, DateTime dataFine, int limite);
    Task<long> CreateAsync(LogAttivitaEntityModel logAttivita);
    Task<bool> DeleteOlderThanAsync(DateTime dataLimite);
}
