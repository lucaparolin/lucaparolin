using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NugoloFamily.Core.Interfaces.Services;
using NugoloFamily.Core.Models.DTOs;

namespace NugoloFamily.API.Controllers;

/// <summary>
/// Controller per la gestione degli Utenti e Autenticazione
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class UtentiController : ControllerBase
{
    private readonly IUtentiService _utentiService;
    private readonly ILogger<UtentiController> _logger;

    public UtentiController(
        IUtentiService utentiService,
        ILogger<UtentiController> logger)
    {
        _utentiService = utentiService;
        _logger = logger;
    }

    /// <summary>
    /// Login utente
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginRispostaDataModel>> Login([FromBody] LoginDataModel model)
    {
        try
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            var risultato = await _utentiService.LoginAsync(model, ipAddress);

            if (risultato == null)
            {
                return Unauthorized(new { Message = "Username o password non validi" });
            }

            _logger.LogInformation("Login effettuato con successo per utente {Username}", model.Username);
            return Ok(risultato);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Errore durante il login dell'utente {Username}", model.Username);
            return StatusCode(500, new { Message = "Errore interno del server" });
        }
    }

    /// <summary>
    /// Registrazione nuovo utente
    /// </summary>
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<UtenteDataModel>> Register([FromBody] RegistraUtenteDataModel model)
    {
        try
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            var utente = await _utentiService.CreateAsync(model, "SYSTEM", ipAddress);

            _logger.LogInformation("Utente {Username} registrato con successo", model.Username);
            return CreatedAtAction(nameof(GetById), new { id = utente.IdUtente }, utente);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Errore durante la registrazione dell'utente");
            return StatusCode(500, new { Message = "Errore interno del server" });
        }
    }

    /// <summary>
    /// Ottiene un utente per ID
    /// </summary>
    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<UtenteDataModel>> GetById(int id)
    {
        try
        {
            var utente = await _utentiService.GetByIdAsync(id);

            if (utente == null)
            {
                return NotFound(new { Message = $"Utente con ID {id} non trovato" });
            }

            return Ok(utente);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Errore durante il recupero dell'utente {IdUtente}", id);
            return StatusCode(500, new { Message = "Errore interno del server" });
        }
    }

    /// <summary>
    /// Ottiene tutti gli utenti di una famiglia
    /// </summary>
    [HttpGet("famiglia/{idFamiglia}")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<UtenteDataModel>>> GetByFamiglia(int idFamiglia)
    {
        try
        {
            var utenti = await _utentiService.GetByFamigliaAsync(idFamiglia);
            return Ok(utenti);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Errore durante il recupero degli utenti della famiglia {IdFamiglia}", idFamiglia);
            return StatusCode(500, new { Message = "Errore interno del server" });
        }
    }

    /// <summary>
    /// Ottiene gli utenti attivi di una famiglia
    /// </summary>
    [HttpGet("famiglia/{idFamiglia}/attivi")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<UtenteDataModel>>> GetAttiviByFamiglia(int idFamiglia)
    {
        try
        {
            var utenti = await _utentiService.GetAttiviByFamigliaAsync(idFamiglia);
            return Ok(utenti);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Errore durante il recupero degli utenti attivi della famiglia {IdFamiglia}", idFamiglia);
            return StatusCode(500, new { Message = "Errore interno del server" });
        }
    }

    /// <summary>
    /// Aggiorna un utente
    /// </summary>
    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult> Update(int id, [FromBody] AggiornaUtenteDataModel model)
    {
        try
        {
            if (id != model.IdUtente)
            {
                return BadRequest(new { Message = "ID utente non corrispondente" });
            }

            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            var utenteCorrente = User.Identity?.Name ?? "UNKNOWN";

            var risultato = await _utentiService.UpdateAsync(model, utenteCorrente, ipAddress);

            if (!risultato)
            {
                return StatusCode(500, new { Message = "Errore durante l'aggiornamento dell'utente" });
            }

            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Errore durante l'aggiornamento dell'utente {IdUtente}", id);
            return StatusCode(500, new { Message = "Errore interno del server" });
        }
    }

    /// <summary>
    /// Elimina un utente
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Amministratore")]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            var risultato = await _utentiService.DeleteAsync(id);

            if (!risultato)
            {
                return StatusCode(500, new { Message = "Errore durante l'eliminazione dell'utente" });
            }

            _logger.LogInformation("Utente {IdUtente} eliminato con successo", id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Errore durante l'eliminazione dell'utente {IdUtente}", id);
            return StatusCode(500, new { Message = "Errore interno del server" });
        }
    }

    /// <summary>
    /// Aggiorna la password dell'utente
    /// </summary>
    [HttpPost("{id}/cambio-password")]
    [Authorize]
    public async Task<ActionResult> UpdatePassword(int id, [FromBody] CambioPasswordModel model)
    {
        try
        {
            var risultato = await _utentiService.UpdatePasswordAsync(id, model.VecchiaPassword, model.NuovaPassword);

            if (!risultato)
            {
                return StatusCode(500, new { Message = "Errore durante il cambio password" });
            }

            _logger.LogInformation("Password aggiornata con successo per utente {IdUtente}", id);
            return Ok(new { Message = "Password aggiornata con successo" });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Errore durante il cambio password per utente {IdUtente}", id);
            return StatusCode(500, new { Message = "Errore interno del server" });
        }
    }
}

/// <summary>
/// Modello per cambio password
/// </summary>
public class CambioPasswordModel
{
    public string VecchiaPassword { get; set; } = string.Empty;
    public string NuovaPassword { get; set; } = string.Empty;
}
