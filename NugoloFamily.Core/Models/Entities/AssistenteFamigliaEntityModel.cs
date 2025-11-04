namespace NugoloFamily.Core.Models.Entities;

/// <summary>
/// Modello Entity per la tabella AssistentiFamiglie
/// Rappresenta l'associazione tra una famiglia e un assistente attivato
/// </summary>
public class AssistenteFamigliaEntityModel
{
    public int IdAssistenteFamiglia { get; set; }
    public int IdFamiglia { get; set; }
    public int IdAssistente { get; set; }
    public bool Attivo { get; set; } = true;
    public string? ConfigurazionePersonalizzata { get; set; }
    public DateTime DataAttivazione { get; set; }

    // Campi di tracciamento
    public DateTime DataInserimento { get; set; }
    public string UtenteInserimento { get; set; } = string.Empty;
    public string? IPInserimento { get; set; }
    public DateTime? DataModifica { get; set; }
    public string? UtenteModifica { get; set; }
    public string? IPModifica { get; set; }
}
