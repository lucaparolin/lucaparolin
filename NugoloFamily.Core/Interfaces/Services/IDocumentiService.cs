using NugoloFamily.Core.Models.DTOs;

namespace NugoloFamily.Core.Interfaces.Services;

/// <summary>
/// Interfaccia per il servizio di gestione dei Documenti
/// </summary>
public interface IDocumentiService
{
    Task<DocumentoDataModel?> GetByIdAsync(int idDocumento);
    Task<DocumentoDettaglioDataModel?> GetDettaglioByIdAsync(int idDocumento);
    Task<IEnumerable<DocumentoDataModel>> GetByFamigliaAsync(int idFamiglia);
    Task<IEnumerable<DocumentoDataModel>> GetByTipoAsync(int idFamiglia, string tipoDocumento);
    Task<IEnumerable<DocumentoDataModel>> GetByCategoriaAsync(int idFamiglia, string categoria);
    Task<DocumentoDataModel> UploadAsync(CaricaDocumentoDataModel model, string utenteCorrente, string? ipAddress);
    Task<bool> DeleteAsync(int idDocumento);
    Task<bool> ElaboraDocumentoAsync(int idDocumento);
}
