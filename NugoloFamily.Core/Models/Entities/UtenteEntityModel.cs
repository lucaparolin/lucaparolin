namespace NugoloFamily.Core.Models.Entities;

/// <summary>
/// Modello Entity per la tabella Utenti
/// Rappresenta un utente appartenente a una famiglia
/// </summary>
public class UtenteEntityModel
{
    public int IdUtente { get; set; }
    public int IdFamiglia { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Salt { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string Cognome { get; set; } = string.Empty;
    public DateTime? DataNascita { get; set; }
    public string Ruolo { get; set; } = "Familiare";
    public bool Attivo { get; set; } = true;
    public DateTime? UltimoAccesso { get; set; }
    public string? TokenRefresh { get; set; }
    public DateTime? DataScadenzaToken { get; set; }

    // Campi di tracciamento
    public DateTime DataInserimento { get; set; }
    public string UtenteInserimento { get; set; } = string.Empty;
    public string? IPInserimento { get; set; }
    public DateTime? DataModifica { get; set; }
    public string? UtenteModifica { get; set; }
    public string? IPModifica { get; set; }
}
