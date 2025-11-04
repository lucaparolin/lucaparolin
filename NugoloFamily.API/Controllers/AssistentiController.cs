using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NugoloFamily.Core.Interfaces.Services;
using NugoloFamily.Core.Models.DTOs;

namespace NugoloFamily.API.Controllers;

/// <summary>
/// Controller per la gestione degli Assistenti
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AssistentiController : ControllerBase
{
    private readonly IAssistentiService _assistentiService;
    private readonly ILogger<AssistentiController> _logger;

    public AssistentiController(
        IAssistentiService assistentiService,
        ILogger<AssistentiController> logger)
    {
        _assistentiService = assistentiService;
        _logger = logger;
    }

    /// <summary>
    /// Ottiene tutti gli assistenti disponibili
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AssistenteDataModel>>> GetAll()
    {
        try
        {
            var assistenti = await _assistentiService.GetAllAsync();
            return Ok(assistenti);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Errore durante il recupero degli assistenti");
            return StatusCode(500, new { Message = "Errore interno del server" });
        }
    }

    /// <summary>
    /// Ottiene tutti gli assistenti attivi
    /// </summary>
    [HttpGet("attivi")]
    public async Task<ActionResult<IEnumerable<AssistenteDataModel>>> GetAttivi()
    {
        try
        {
            var assistenti = await _assistentiService.GetAttiviAsync();
            return Ok(assistenti);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Errore durante il recupero degli assistenti attivi");
            return StatusCode(500, new { Message = "Errore interno del server" });
        }
    }

    /// <summary>
    /// Ottiene un assistente per ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<AssistenteDataModel>> GetById(int id)
    {
        try
        {
            var assistente = await _assistentiService.GetByIdAsync(id);

            if (assistente == null)
            {
                return NotFound(new { Message = $"Assistente con ID {id} non trovato" });
            }

            return Ok(assistente);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Errore durante il recupero dell'assistente {IdAssistente}", id);
            return StatusCode(500, new { Message = "Errore interno del server" });
        }
    }

    /// <summary>
    /// Ottiene un assistente per codice
    /// </summary>
    [HttpGet("codice/{codice}")]
    public async Task<ActionResult<AssistenteDataModel>> GetByCodice(string codice)
    {
        try
        {
            var assistente = await _assistentiService.GetByCodiceAsync(codice);

            if (assistente == null)
            {
                return NotFound(new { Message = $"Assistente con codice {codice} non trovato" });
            }

            return Ok(assistente);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Errore durante il recupero dell'assistente con codice {Codice}", codice);
            return StatusCode(500, new { Message = "Errore interno del server" });
        }
    }

    /// <summary>
    /// Ottiene gli assistenti per categoria
    /// </summary>
    [HttpGet("categoria/{categoria}")]
    public async Task<ActionResult<IEnumerable<AssistenteDataModel>>> GetByCategoria(string categoria)
    {
        try
        {
            var assistenti = await _assistentiService.GetByCategoriaAsync(categoria);
            return Ok(assistenti);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Errore durante il recupero degli assistenti per categoria {Categoria}", categoria);
            return StatusCode(500, new { Message = "Errore interno del server" });
        }
    }

    /// <summary>
    /// Ottiene gli assistenti attivi per una famiglia
    /// </summary>
    [HttpGet("famiglia/{idFamiglia}")]
    public async Task<ActionResult<IEnumerable<AssistenteFamigliaDataModel>>> GetAttiviPerFamiglia(int idFamiglia)
    {
        try
        {
            var assistenti = await _assistentiService.GetAssistentiAttiviPerFamigliaAsync(idFamiglia);
            return Ok(assistenti);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Errore durante il recupero degli assistenti attivi per famiglia {IdFamiglia}", idFamiglia);
            return StatusCode(500, new { Message = "Errore interno del server" });
        }
    }

    /// <summary>
    /// Attiva un assistente per una famiglia
    /// </summary>
    [HttpPost("famiglia/{idFamiglia}/attiva")]
    [Authorize(Roles = "Amministratore")]
    public async Task<ActionResult> AttivaPerFamiglia(int idFamiglia, [FromBody] AttivaAssistenteRequest request)
    {
        try
        {
            var model = new AttivaAssistenteDataModel
            {
                IdFamiglia = idFamiglia,
                IdAssistente = request.IdAssistente,
                ConfigurazionePersonalizzata = request.ConfigurazionePersonalizzata
            };

            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            var utenteCorrente = User.Identity?.Name ?? "UNKNOWN";

            var risultato = await _assistentiService.AttivaAssistentePerFamigliaAsync(model, utenteCorrente, ipAddress);

            if (!risultato)
            {
                return StatusCode(500, new { Message = "Errore durante l'attivazione dell'assistente" });
            }

            _logger.LogInformation("Assistente {IdAssistente} attivato per famiglia {IdFamiglia}", request.IdAssistente, idFamiglia);
            return Ok(new { Message = "Assistente attivato con successo" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Errore durante l'attivazione dell'assistente per famiglia {IdFamiglia}", idFamiglia);
            return StatusCode(500, new { Message = "Errore interno del server" });
        }
    }

    /// <summary>
    /// Disattiva un assistente per una famiglia
    /// </summary>
    [HttpPost("famiglia/{idFamiglia}/disattiva/{idAssistente}")]
    [Authorize(Roles = "Amministratore")]
    public async Task<ActionResult> DisattivaPerFamiglia(int idFamiglia, int idAssistente)
    {
        try
        {
            var risultato = await _assistentiService.DisattivaAssistentePerFamigliaAsync(idFamiglia, idAssistente);

            if (!risultato)
            {
                return StatusCode(500, new { Message = "Errore durante la disattivazione dell'assistente" });
            }

            _logger.LogInformation("Assistente {IdAssistente} disattivato per famiglia {IdFamiglia}", idAssistente, idFamiglia);
            return Ok(new { Message = "Assistente disattivato con successo" });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Errore durante la disattivazione dell'assistente per famiglia {IdFamiglia}", idFamiglia);
            return StatusCode(500, new { Message = "Errore interno del server" });
        }
    }
}

/// <summary>
/// Request per attivazione assistente
/// </summary>
public class AttivaAssistenteRequest
{
    public int IdAssistente { get; set; }
    public string? ConfigurazionePersonalizzata { get; set; }
}
