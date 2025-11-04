using NugoloFamily.Core.Models.Entities;

namespace NugoloFamily.Core.Interfaces.Repositories;

/// <summary>
/// Interfaccia repository per la gestione dei Documenti
/// </summary>
public interface IDocumentiRepository
{
    Task<DocumentoEntityModel?> GetByIdAsync(int idDocumento);
    Task<IEnumerable<DocumentoEntityModel>> GetByFamigliaAsync(int idFamiglia);
    Task<IEnumerable<DocumentoEntityModel>> GetByUtenteAsync(int idUtente);
    Task<IEnumerable<DocumentoEntityModel>> GetByTipoAsync(int idFamiglia, string tipoDocumento);
    Task<IEnumerable<DocumentoEntityModel>> GetByCategoriaAsync(int idFamiglia, string categoria);
    Task<IEnumerable<DocumentoEntityModel>> GetByAssistenteAsync(int idAssistente);
    Task<IEnumerable<DocumentoEntityModel>> GetDaIndicizzareAsync(int limite);
    Task<int> CreateAsync(DocumentoEntityModel documento);
    Task<bool> UpdateAsync(DocumentoEntityModel documento);
    Task<bool> DeleteAsync(int idDocumento);
    Task<bool> UpdateTestoEstrattoAsync(int idDocumento, string testoEstratto, string? metadati);
    Task<bool> SegnaIndicizzatoAsync(int idDocumento);
}
