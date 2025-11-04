namespace NugoloFamily.Core.Models.Entities;

/// <summary>
/// Modello Entity per la tabella Assistenti
/// Rappresenta un assistente specializzato disponibile nel sistema
/// </summary>
public class AssistenteEntityModel
{
    public int IdAssistente { get; set; }
    public string NomeAssistente { get; set; } = string.Empty;
    public string CodiceAssistente { get; set; } = string.Empty;
    public string? Descrizione { get; set; }
    public string? Icona { get; set; }
    public string? Categoria { get; set; }
    public bool Attivo { get; set; } = true;
    public int Ordinamento { get; set; } = 0;
    public bool RichiedeConfigurazione { get; set; } = false;
    public string? ParametriConfigurazione { get; set; }

    // Campi di tracciamento
    public DateTime DataInserimento { get; set; }
    public string UtenteInserimento { get; set; } = string.Empty;
    public string? IPInserimento { get; set; }
    public DateTime? DataModifica { get; set; }
    public string? UtenteModifica { get; set; }
    public string? IPModifica { get; set; }
}
