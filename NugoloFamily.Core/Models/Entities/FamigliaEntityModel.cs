namespace NugoloFamily.Core.Models.Entities;

/// <summary>
/// Modello Entity per la tabella Famiglie
/// Rappresenta una famiglia registrata nel sistema (multi-tenant)
/// </summary>
public class FamigliaEntityModel
{
    public int IdFamiglia { get; set; }
    public string NomeFamiglia { get; set; } = string.Empty;
    public string CodiceUnivoco { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public string? Indirizzo { get; set; }
    public string StatoAttivazione { get; set; } = "Attivo";
    public DateTime? DataScadenzaAbbonamento { get; set; }
    public string? PianoAbbonamento { get; set; }
    public int LimiteUtenti { get; set; } = 5;

    // Campi di tracciamento
    public DateTime DataInserimento { get; set; }
    public string UtenteInserimento { get; set; } = string.Empty;
    public string? IPInserimento { get; set; }
    public DateTime? DataModifica { get; set; }
    public string? UtenteModifica { get; set; }
    public string? IPModifica { get; set; }
}
