namespace NugoloFamily.Core.Models.Entities;

/// <summary>
/// Modello Entity per la tabella Documenti
/// Rappresenta un documento caricato da una famiglia
/// </summary>
public class DocumentoEntityModel
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
    public bool Indicizzato { get; set; } = false;
    public int? AssistenteAssociato { get; set; }
    public DateTime? DataDocumento { get; set; }
    public DateTime? DataScadenza { get; set; }

    // Campi di tracciamento
    public DateTime DataInserimento { get; set; }
    public string UtenteInserimento { get; set; } = string.Empty;
    public string? IPInserimento { get; set; }
    public DateTime? DataModifica { get; set; }
    public string? UtenteModifica { get; set; }
    public string? IPModifica { get; set; }
}
