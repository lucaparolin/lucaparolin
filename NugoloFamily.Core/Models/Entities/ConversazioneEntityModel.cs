namespace NugoloFamily.Core.Models.Entities;

/// <summary>
/// Modello Entity per la tabella Conversazioni
/// Rappresenta una conversazione tra un utente e un assistente
/// </summary>
public class ConversazioneEntityModel
{
    public int IdConversazione { get; set; }
    public int IdFamiglia { get; set; }
    public int IdUtente { get; set; }
    public int IdAssistente { get; set; }
    public string? TitoloConversazione { get; set; }
    public string StatoConversazione { get; set; } = "Aperta";
    public DateTime? UltimoMessaggio { get; set; }
    public int ContatoreMessaggi { get; set; } = 0;
    public string? MetadatiConversazione { get; set; }

    // Campi di tracciamento
    public DateTime DataInserimento { get; set; }
    public string UtenteInserimento { get; set; } = string.Empty;
    public string? IPInserimento { get; set; }
    public DateTime? DataModifica { get; set; }
    public string? UtenteModifica { get; set; }
    public string? IPModifica { get; set; }
}
