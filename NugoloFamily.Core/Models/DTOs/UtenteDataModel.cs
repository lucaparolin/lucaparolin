namespace NugoloFamily.Core.Models.DTOs;

/// <summary>
/// DTO per la gestione degli Utenti
/// </summary>
public class UtenteDataModel
{
    public int IdUtente { get; set; }
    public int IdFamiglia { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string Cognome { get; set; } = string.Empty;
    public DateTime? DataNascita { get; set; }
    public string Ruolo { get; set; } = "Familiare";
    public bool Attivo { get; set; }
    public DateTime? UltimoAccesso { get; set; }
    public DateTime DataInserimento { get; set; }
}

/// <summary>
/// DTO per la registrazione di un nuovo Utente
/// </summary>
public class RegistraUtenteDataModel
{
    public int IdFamiglia { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string Cognome { get; set; } = string.Empty;
    public DateTime? DataNascita { get; set; }
    public string Ruolo { get; set; } = "Familiare";
}

/// <summary>
/// DTO per il login
/// </summary>
public class LoginDataModel
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

/// <summary>
/// DTO per la risposta del login
/// </summary>
public class LoginRispostaDataModel
{
    public int IdUtente { get; set; }
    public int IdFamiglia { get; set; }
    public string Username { get; set; } = string.Empty;
    public string NomeCompleto { get; set; } = string.Empty;
    public string Ruolo { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime DataScadenzaToken { get; set; }
}

/// <summary>
/// DTO per l'aggiornamento di un Utente
/// </summary>
public class AggiornaUtenteDataModel
{
    public int IdUtente { get; set; }
    public string? Email { get; set; }
    public string? Nome { get; set; }
    public string? Cognome { get; set; }
    public DateTime? DataNascita { get; set; }
    public bool? Attivo { get; set; }
}
