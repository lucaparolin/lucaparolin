namespace NugoloFamily.Core.Models.Entities;

/// <summary>
/// Modello Entity per la tabella PermessiUtenti
/// Rappresenta un permesso granulare assegnato a un utente
/// </summary>
public class PermessoUtenteEntityModel
{
    public int IdPermesso { get; set; }
    public int IdUtente { get; set; }
    public int? IdAssistente { get; set; }
    public string TipoPermesso { get; set; } = string.Empty;
    public bool Concesso { get; set; } = true;

    // Campi di tracciamento
    public DateTime DataInserimento { get; set; }
    public string UtenteInserimento { get; set; } = string.Empty;
    public string? IPInserimento { get; set; }
    public DateTime? DataModifica { get; set; }
    public string? UtenteModifica { get; set; }
    public string? IPModifica { get; set; }
}
