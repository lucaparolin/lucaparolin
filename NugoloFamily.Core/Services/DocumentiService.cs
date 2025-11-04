using NugoloFamily.Core.Interfaces.Repositories;
using NugoloFamily.Core.Interfaces.Services;
using NugoloFamily.Core.Models.DTOs;
using NugoloFamily.Core.Models.Entities;

namespace NugoloFamily.Core.Services;

/// <summary>
/// Servizio per la gestione dei Documenti
/// </summary>
public class DocumentiService : IDocumentiService
{
    private readonly IDocumentiRepository _documentiRepository;
    private readonly IAssistentiRepository _assistentiRepository;

    public DocumentiService(
        IDocumentiRepository documentiRepository,
        IAssistentiRepository assistentiRepository)
    {
        _documentiRepository = documentiRepository;
        _assistentiRepository = assistentiRepository;
    }

    public async Task<DocumentoDataModel?> GetByIdAsync(int idDocumento)
    {
        var documento = await _documentiRepository.GetByIdAsync(idDocumento);
        if (documento == null)
            return null;

        string? nomeAssistente = null;
        if (documento.AssistenteAssociato.HasValue)
        {
            var assistente = await _assistentiRepository.GetByIdAsync(documento.AssistenteAssociato.Value);
            nomeAssistente = assistente?.NomeAssistente;
        }

        return new DocumentoDataModel
        {
            IdDocumento = documento.IdDocumento,
            IdFamiglia = documento.IdFamiglia,
            IdUtente = documento.IdUtente,
            NomeFile = documento.NomeFile,
            TipoDocumento = documento.TipoDocumento,
            Categoria = documento.Categoria,
            DimensioneFile = documento.DimensioneFile,
            MimeType = documento.MimeType,
            Indicizzato = documento.Indicizzato,
            AssistenteAssociato = documento.AssistenteAssociato,
            NomeAssistente = nomeAssistente,
            DataDocumento = documento.DataDocumento,
            DataScadenza = documento.DataScadenza,
            DataInserimento = documento.DataInserimento
        };
    }

    public async Task<DocumentoDettaglioDataModel?> GetDettaglioByIdAsync(int idDocumento)
    {
        var documento = await _documentiRepository.GetByIdAsync(idDocumento);
        if (documento == null)
            return null;

        string? nomeAssistente = null;
        if (documento.AssistenteAssociato.HasValue)
        {
            var assistente = await _assistentiRepository.GetByIdAsync(documento.AssistenteAssociato.Value);
            nomeAssistente = assistente?.NomeAssistente;
        }

        return new DocumentoDettaglioDataModel
        {
            IdDocumento = documento.IdDocumento,
            IdFamiglia = documento.IdFamiglia,
            IdUtente = documento.IdUtente,
            NomeFile = documento.NomeFile,
            PercorsoFile = documento.PercorsoFile,
            TipoDocumento = documento.TipoDocumento,
            Categoria = documento.Categoria,
            DimensioneFile = documento.DimensioneFile,
            MimeType = documento.MimeType,
            TestoEstratto = documento.TestoEstratto,
            MetadatiDocumento = documento.MetadatiDocumento,
            Indicizzato = documento.Indicizzato,
            AssistenteAssociato = documento.AssistenteAssociato,
            NomeAssistente = nomeAssistente,
            DataDocumento = documento.DataDocumento,
            DataScadenza = documento.DataScadenza,
            DataInserimento = documento.DataInserimento
        };
    }

    public async Task<IEnumerable<DocumentoDataModel>> GetByFamigliaAsync(int idFamiglia)
    {
        var documenti = await _documentiRepository.GetByFamigliaAsync(idFamiglia);
        var result = new List<DocumentoDataModel>();

        foreach (var doc in documenti)
        {
            string? nomeAssistente = null;
            if (doc.AssistenteAssociato.HasValue)
            {
                var assistente = await _assistentiRepository.GetByIdAsync(doc.AssistenteAssociato.Value);
                nomeAssistente = assistente?.NomeAssistente;
            }

            result.Add(new DocumentoDataModel
            {
                IdDocumento = doc.IdDocumento,
                IdFamiglia = doc.IdFamiglia,
                IdUtente = doc.IdUtente,
                NomeFile = doc.NomeFile,
                TipoDocumento = doc.TipoDocumento,
                Categoria = doc.Categoria,
                DimensioneFile = doc.DimensioneFile,
                MimeType = doc.MimeType,
                Indicizzato = doc.Indicizzato,
                AssistenteAssociato = doc.AssistenteAssociato,
                NomeAssistente = nomeAssistente,
                DataDocumento = doc.DataDocumento,
                DataScadenza = doc.DataScadenza,
                DataInserimento = doc.DataInserimento
            });
        }

        return result;
    }

    public async Task<IEnumerable<DocumentoDataModel>> GetByTipoAsync(int idFamiglia, string tipoDocumento)
    {
        var documenti = await _documentiRepository.GetByTipoAsync(idFamiglia, tipoDocumento);
        return await MapDocumentsToDataModels(documenti);
    }

    public async Task<IEnumerable<DocumentoDataModel>> GetByCategoriaAsync(int idFamiglia, string categoria)
    {
        var documenti = await _documentiRepository.GetByCategoriaAsync(idFamiglia, categoria);
        return await MapDocumentsToDataModels(documenti);
    }

    public async Task<DocumentoDataModel> UploadAsync(CaricaDocumentoDataModel model, string utenteCorrente, string? ipAddress)
    {
        // Validazioni
        if (string.IsNullOrWhiteSpace(model.NomeFile))
            throw new ArgumentException("Il nome del file è obbligatorio");

        if (string.IsNullOrWhiteSpace(model.PercorsoFile))
            throw new ArgumentException("Il percorso del file è obbligatorio");

        if (model.DimensioneFile <= 0)
            throw new ArgumentException("La dimensione del file deve essere maggiore di zero");

        // Validazione dimensione massima (es: 50MB)
        const long maxSize = 50 * 1024 * 1024;
        if (model.DimensioneFile > maxSize)
            throw new ArgumentException($"Il file supera la dimensione massima consentita di {maxSize / (1024 * 1024)}MB");

        // Determina assistente associato in base al tipo o categoria
        int? assistenteAssociato = null;
        if (!string.IsNullOrEmpty(model.TipoDocumento))
        {
            assistenteAssociato = DeterminaAssistentePerTipo(model.TipoDocumento);
        }
        else if (!string.IsNullOrEmpty(model.Categoria))
        {
            assistenteAssociato = await DeterminaAssistentePerCategoriaAsync(model.Categoria);
        }

        var entity = new DocumentoEntityModel
        {
            IdFamiglia = model.IdFamiglia,
            IdUtente = model.IdUtente,
            NomeFile = model.NomeFile.Trim(),
            PercorsoFile = model.PercorsoFile,
            TipoDocumento = model.TipoDocumento?.Trim(),
            Categoria = model.Categoria?.Trim(),
            DimensioneFile = model.DimensioneFile,
            MimeType = model.MimeType,
            Indicizzato = false,
            AssistenteAssociato = assistenteAssociato,
            DataInserimento = DateTime.Now,
            UtenteInserimento = utenteCorrente,
            IPInserimento = ipAddress
        };

        var idDocumento = await _documentiRepository.CreateAsync(entity);
        entity.IdDocumento = idDocumento;

        string? nomeAssistente = null;
        if (assistenteAssociato.HasValue)
        {
            var assistente = await _assistentiRepository.GetByIdAsync(assistenteAssociato.Value);
            nomeAssistente = assistente?.NomeAssistente;
        }

        return new DocumentoDataModel
        {
            IdDocumento = entity.IdDocumento,
            IdFamiglia = entity.IdFamiglia,
            IdUtente = entity.IdUtente,
            NomeFile = entity.NomeFile,
            TipoDocumento = entity.TipoDocumento,
            Categoria = entity.Categoria,
            DimensioneFile = entity.DimensioneFile,
            MimeType = entity.MimeType,
            Indicizzato = entity.Indicizzato,
            AssistenteAssociato = entity.AssistenteAssociato,
            NomeAssistente = nomeAssistente,
            DataDocumento = entity.DataDocumento,
            DataScadenza = entity.DataScadenza,
            DataInserimento = entity.DataInserimento
        };
    }

    public async Task<bool> DeleteAsync(int idDocumento)
    {
        var documento = await _documentiRepository.GetByIdAsync(idDocumento);
        if (documento == null)
            throw new InvalidOperationException($"Documento con ID {idDocumento} non trovato");

        // TODO: Eliminare anche il file fisico dal filesystem
        return await _documentiRepository.DeleteAsync(idDocumento);
    }

    public async Task<bool> ElaboraDocumentoAsync(int idDocumento)
    {
        // Questa è una implementazione placeholder
        // In una implementazione reale, questo metodo dovrebbe:
        // 1. Recuperare il documento dal filesystem
        // 2. Chiamare AWS Textract per estrarre il testo
        // 3. Salvare il testo estratto e i metadati
        // 4. Segnare il documento come indicizzato

        var documento = await _documentiRepository.GetByIdAsync(idDocumento);
        if (documento == null)
            throw new InvalidOperationException($"Documento con ID {idDocumento} non trovato");

        // Placeholder: simula estrazione testo
        var testoEstratto = "Testo estratto placeholder. " +
                           "L'integrazione con AWS Textract verrà implementata successivamente.";

        var metadati = System.Text.Json.JsonSerializer.Serialize(new
        {
            ProcessedAt = DateTime.Now,
            Service = "Placeholder",
            Pages = 1
        });

        await _documentiRepository.UpdateTestoEstrattoAsync(idDocumento, testoEstratto, metadati);
        await _documentiRepository.SegnaIndicizzatoAsync(idDocumento);

        return true;
    }

    // Helper methods

    private async Task<IEnumerable<DocumentoDataModel>> MapDocumentsToDataModels(IEnumerable<DocumentoEntityModel> documenti)
    {
        var result = new List<DocumentoDataModel>();

        foreach (var doc in documenti)
        {
            string? nomeAssistente = null;
            if (doc.AssistenteAssociato.HasValue)
            {
                var assistente = await _assistentiRepository.GetByIdAsync(doc.AssistenteAssociato.Value);
                nomeAssistente = assistente?.NomeAssistente;
            }

            result.Add(new DocumentoDataModel
            {
                IdDocumento = doc.IdDocumento,
                IdFamiglia = doc.IdFamiglia,
                IdUtente = doc.IdUtente,
                NomeFile = doc.NomeFile,
                TipoDocumento = doc.TipoDocumento,
                Categoria = doc.Categoria,
                DimensioneFile = doc.DimensioneFile,
                MimeType = doc.MimeType,
                Indicizzato = doc.Indicizzato,
                AssistenteAssociato = doc.AssistenteAssociato,
                NomeAssistente = nomeAssistente,
                DataDocumento = doc.DataDocumento,
                DataScadenza = doc.DataScadenza,
                DataInserimento = doc.DataInserimento
            });
        }

        return result;
    }

    private int? DeterminaAssistentePerTipo(string tipoDocumento)
    {
        // Mapping tipo documento -> ID assistente (basato sui seed data)
        return tipoDocumento.ToLower() switch
        {
            "fattura" => 3, // Finanze
            "contratto" => 3, // Finanze
            "ricetta" => 7, // Salute
            "referto" => 7, // Salute
            "polizza" => 5, // Assicurazioni
            _ => 8 // Documenti (default)
        };
    }

    private async Task<int?> DeterminaAssistentePerCategoriaAsync(string categoria)
    {
        // Mapping categoria -> Codice assistente
        var codiceAssistente = categoria.ToLower() switch
        {
            "salute" => "SALUTE",
            "finanza" => "FINANZE",
            "casa" => "CASA",
            "legale" => "DOCUMENTI",
            _ => "DOCUMENTI"
        };

        var assistente = await _assistentiRepository.GetByCodiceAsync(codiceAssistente);
        return assistente?.IdAssistente;
    }
}
