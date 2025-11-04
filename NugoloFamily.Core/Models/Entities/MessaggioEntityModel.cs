namespace NugoloFamily.Core.Models.Entities;

/// <summary>
/// Modello Entity per la tabella Messaggi
/// Rappresenta un messaggio scambiato in una conversazione (supporto multimodale)
/// </summary>
public class MessaggioEntityModel
{
    public long IdMessaggio { get; set; }
    public int IdConversazione { get; set; }
    public int? IdUtente { get; set; }
    public string TipoMittente { get; set; } = string.Empty;
    public string? TestoMessaggio { get; set; }
    public string TipoContenuto { get; set; } = "Testo";
    public string? URLContenuto { get; set; }
    public int? DurataContenuto { get; set; }
    public long? DimensioneFile { get; set; }
    public string? MimeType { get; set; }
    public string? MetadatiContenuto { get; set; }
    public string? Sentiment { get; set; }
    public string? IntentRilevato { get; set; }
    public decimal? ConfidenzaIntent { get; set; }
    public int? TokenUtilizzati { get; set; }
    public string? ModelloAIUtilizzato { get; set; }

    // Campi di tracciamento
    public DateTime DataInserimento { get; set; }
    public string UtenteInserimento { get; set; } = string.Empty;
    public string? IPInserimento { get; set; }
}
