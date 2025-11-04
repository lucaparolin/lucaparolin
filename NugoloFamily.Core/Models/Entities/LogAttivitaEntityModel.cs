namespace NugoloFamily.Core.Models.Entities;

/// <summary>
/// Modello Entity per la tabella LogAttivita
/// Rappresenta un log di attività del sistema per audit
/// </summary>
public class LogAttivitaEntityModel
{
    public long IdLog { get; set; }
    public int? IdFamiglia { get; set; }
    public int? IdUtente { get; set; }
    public string TipoAttivita { get; set; } = string.Empty;
    public string? Descrizione { get; set; }
    public string? EntitaCoinvolta { get; set; }
    public int? IdEntita { get; set; }
    public string? IPAddress { get; set; }
    public string? UserAgent { get; set; }
    public int? Durata { get; set; }
    public string? Esito { get; set; }
    public string? DettagliErrore { get; set; }
    public DateTime DataInserimento { get; set; }
}
