namespace NugoloFamily.Core.Models.DTOs;

/// <summary>
/// DTO per la gestione dei Messaggi
/// </summary>
public class MessaggioDataModel
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
    public string? Sentiment { get; set; }
    public string? IntentRilevato { get; set; }
    public DateTime DataInserimento { get; set; }
}

/// <summary>
/// DTO per l'invio di un nuovo Messaggio
/// </summary>
public class InviaMessaggioDataModel
{
    public int IdConversazione { get; set; }
    public int IdUtente { get; set; }
    public string? TestoMessaggio { get; set; }
    public string TipoContenuto { get; set; } = "Testo";
    public string? URLContenuto { get; set; }
    public int? DurataContenuto { get; set; }
    public long? DimensioneFile { get; set; }
    public string? MimeType { get; set; }
}

/// <summary>
/// DTO per la risposta dell'assistente
/// </summary>
public class RispostaAssistenteDataModel
{
    public long IdMessaggio { get; set; }
    public int IdConversazione { get; set; }
    public string TestoRisposta { get; set; } = string.Empty;
    public string? IntentRilevato { get; set; }
    public decimal? ConfidenzaIntent { get; set; }
    public int? TokenUtilizzati { get; set; }
    public string? ModelloAIUtilizzato { get; set; }
    public DateTime DataRisposta { get; set; }
}
