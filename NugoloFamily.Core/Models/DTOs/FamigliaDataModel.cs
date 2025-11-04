namespace NugoloFamily.Core.Models.DTOs;

/// <summary>
/// DTO per la gestione delle Famiglie
/// </summary>
public class FamigliaDataModel
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
    public int LimiteUtenti { get; set; }
    public DateTime DataInserimento { get; set; }
}

/// <summary>
/// DTO per la creazione di una nuova Famiglia
/// </summary>
public class CreaFamigliaDataModel
{
    public string NomeFamiglia { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public string? Indirizzo { get; set; }
    public string PianoAbbonamento { get; set; } = "Free";
}

/// <summary>
/// DTO per l'aggiornamento di una Famiglia
/// </summary>
public class AggiornaFamigliaDataModel
{
    public int IdFamiglia { get; set; }
    public string? NomeFamiglia { get; set; }
    public string? Email { get; set; }
    public string? Telefono { get; set; }
    public string? Indirizzo { get; set; }
    public string? StatoAttivazione { get; set; }
    public DateTime? DataScadenzaAbbonamento { get; set; }
    public string? PianoAbbonamento { get; set; }
}
