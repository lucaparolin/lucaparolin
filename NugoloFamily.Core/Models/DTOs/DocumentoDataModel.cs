namespace NugoloFamily.Core.Models.DTOs;

/// <summary>
/// DTO per la gestione dei Documenti
/// </summary>
public class DocumentoDataModel
{
    public int IdDocumento { get; set; }
    public int IdFamiglia { get; set; }
    public int IdUtente { get; set; }
    public string NomeFile { get; set; } = string.Empty;
    public string? TipoDocumento { get; set; }
    public string? Categoria { get; set; }
    public long? DimensioneFile { get; set; }
    public string? MimeType { get; set; }
    public bool Indicizzato { get; set; }
    public int? AssistenteAssociato { get; set; }
    public string? NomeAssistente { get; set; }
    public DateTime? DataDocumento { get; set; }
    public DateTime? DataScadenza { get; set; }
    public DateTime DataInserimento { get; set; }
}

/// <summary>
/// DTO per il caricamento di un nuovo Documento
/// </summary>
public class CaricaDocumentoDataModel
{
    public int IdFamiglia { get; set; }
    public int IdUtente { get; set; }
    public string NomeFile { get; set; } = string.Empty;
    public string PercorsoFile { get; set; } = string.Empty;
    public long DimensioneFile { get; set; }
    public string MimeType { get; set; } = string.Empty;
    public string? TipoDocumento { get; set; }
    public string? Categoria { get; set; }
}

/// <summary>
/// DTO per il dettaglio completo di un Documento
/// </summary>
public class DocumentoDettaglioDataModel
{
    public int IdDocumento { get; set; }
    public int IdFamiglia { get; set; }
    public int IdUtente { get; set; }
    public string NomeFile { get; set; } = string.Empty;
    public string PercorsoFile { get; set; } = string.Empty;
    public string? TipoDocumento { get; set; }
    public string? Categoria { get; set; }
    public long? DimensioneFile { get; set; }
    public string? MimeType { get; set; }
    public string? TestoEstratto { get; set; }
    public string? MetadatiDocumento { get; set; }
    public bool Indicizzato { get; set; }
    public int? AssistenteAssociato { get; set; }
    public string? NomeAssistente { get; set; }
    public DateTime? DataDocumento { get; set; }
    public DateTime? DataScadenza { get; set; }
    public DateTime DataInserimento { get; set; }
}
