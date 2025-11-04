namespace NugoloFamily.Core.Models.DTOs;

/// <summary>
/// DTO per la gestione degli Assistenti
/// </summary>
public class AssistenteDataModel
{
    public int IdAssistente { get; set; }
    public string NomeAssistente { get; set; } = string.Empty;
    public string CodiceAssistente { get; set; } = string.Empty;
    public string? Descrizione { get; set; }
    public string? Icona { get; set; }
    public string? Categoria { get; set; }
    public bool Attivo { get; set; }
    public int Ordinamento { get; set; }
    public bool RichiedeConfigurazione { get; set; }
}

/// <summary>
/// DTO per gli assistenti attivati per una famiglia
/// </summary>
public class AssistenteFamigliaDataModel
{
    public int IdAssistenteFamiglia { get; set; }
    public int IdFamiglia { get; set; }
    public int IdAssistente { get; set; }
    public string NomeAssistente { get; set; } = string.Empty;
    public string CodiceAssistente { get; set; } = string.Empty;
    public string? Descrizione { get; set; }
    public string? Categoria { get; set; }
    public bool Attivo { get; set; }
    public string? ConfigurazionePersonalizzata { get; set; }
    public DateTime DataAttivazione { get; set; }
}

/// <summary>
/// DTO per l'attivazione di un assistente per una famiglia
/// </summary>
public class AttivaAssistenteDataModel
{
    public int IdFamiglia { get; set; }
    public int IdAssistente { get; set; }
    public string? ConfigurazionePersonalizzata { get; set; }
}
