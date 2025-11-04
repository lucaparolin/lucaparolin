namespace NugoloFamily.Core.Models.DTOs;

/// <summary>
/// DTO per la gestione delle Conversazioni
/// </summary>
public class ConversazioneDataModel
{
    public int IdConversazione { get; set; }
    public int IdFamiglia { get; set; }
    public int IdUtente { get; set; }
    public int IdAssistente { get; set; }
    public string NomeAssistente { get; set; } = string.Empty;
    public string? TitoloConversazione { get; set; }
    public string StatoConversazione { get; set; } = "Aperta";
    public DateTime? UltimoMessaggio { get; set; }
    public int ContatoreMessaggi { get; set; }
    public DateTime DataInserimento { get; set; }
}

/// <summary>
/// DTO per la creazione di una nuova Conversazione
/// </summary>
public class CreaConversazioneDataModel
{
    public int IdFamiglia { get; set; }
    public int IdUtente { get; set; }
    public int IdAssistente { get; set; }
    public string? TitoloConversazione { get; set; }
    public string? PrimoMessaggio { get; set; }
}

/// <summary>
/// DTO per il dettaglio di una Conversazione con messaggi
/// </summary>
public class ConversazioneDettaglioDataModel
{
    public int IdConversazione { get; set; }
    public int IdFamiglia { get; set; }
    public int IdUtente { get; set; }
    public int IdAssistente { get; set; }
    public string NomeAssistente { get; set; } = string.Empty;
    public string? TitoloConversazione { get; set; }
    public string StatoConversazione { get; set; } = "Aperta";
    public DateTime? UltimoMessaggio { get; set; }
    public int ContatoreMessaggi { get; set; }
    public DateTime DataInserimento { get; set; }
    public List<MessaggioDataModel> Messaggi { get; set; } = new();
}
