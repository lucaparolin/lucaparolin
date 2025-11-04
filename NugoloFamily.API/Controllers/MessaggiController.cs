using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NugoloFamily.Core.Interfaces.Services;
using NugoloFamily.Core.Models.DTOs;

namespace NugoloFamily.API.Controllers;

/// <summary>
/// Controller per la gestione dei Messaggi
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MessaggiController : ControllerBase
{
    private readonly IMessaggiService _messaggiService;
    private readonly ILogger<MessaggiController> _logger;

    public MessaggiController(
        IMessaggiService messaggiService,
        ILogger<MessaggiController> logger)
    {
        _messaggiService = messaggiService;
        _logger = logger;
    }

    /// <summary>
    /// Ottiene un messaggio per ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<MessaggioDataModel>> GetById(long id)
    {
        try
        {
            var messaggio = await _messaggiService.GetByIdAsync(id);

            if (messaggio == null)
            {
                return NotFound(new { Message = $"Messaggio con ID {id} non trovato" });
            }

            return Ok(messaggio);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Errore durante il recupero del messaggio {IdMessaggio}", id);
            return StatusCode(500, new { Message = "Errore interno del server" });
        }
    }

    /// <summary>
    /// Ottiene tutti i messaggi di una conversazione
    /// </summary>
    [HttpGet("conversazione/{idConversazione}")]
    public async Task<ActionResult<IEnumerable<MessaggioDataModel>>> GetByConversazione(int idConversazione)
    {
        try
        {
            var messaggi = await _messaggiService.GetByConversazioneAsync(idConversazione);
            return Ok(messaggi);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Errore durante il recupero dei messaggi della conversazione {IdConversazione}", idConversazione);
            return StatusCode(500, new { Message = "Errore interno del server" });
        }
    }

    /// <summary>
    /// Ottiene i messaggi di una conversazione con paginazione
    /// </summary>
    [HttpGet("conversazione/{idConversazione}/paginati")]
    public async Task<ActionResult<IEnumerable<MessaggioDataModel>>> GetByConversazionePaginati(
        int idConversazione,
        [FromQuery] int pagina = 1,
        [FromQuery] int dimensionePagina = 50)
    {
        try
        {
            var messaggi = await _messaggiService.GetByConversazionePaginatiAsync(idConversazione, pagina, dimensionePagina);
            return Ok(messaggi);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Errore durante il recupero dei messaggi paginati della conversazione {IdConversazione}", idConversazione);
            return StatusCode(500, new { Message = "Errore interno del server" });
        }
    }

    /// <summary>
    /// Invia un nuovo messaggio
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<MessaggioDataModel>> InviaMessaggio([FromBody] InviaMessaggioDataModel model)
    {
        try
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            var utenteCorrente = User.Identity?.Name ?? "UNKNOWN";

            var messaggio = await _messaggiService.InviaMessaggioAsync(model, utenteCorrente, ipAddress);

            _logger.LogInformation("Messaggio {IdMessaggio} inviato nella conversazione {IdConversazione}", messaggio.IdMessaggio, model.IdConversazione);
            return CreatedAtAction(nameof(GetById), new { id = messaggio.IdMessaggio }, messaggio);
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
            _logger.LogError(ex, "Errore durante l'invio del messaggio");
            return StatusCode(500, new { Message = "Errore interno del server" });
        }
    }

    /// <summary>
    /// Elabora la risposta dell'assistente a un messaggio
    /// </summary>
    [HttpPost("{idMessaggio}/elabora-risposta")]
    public async Task<ActionResult<RispostaAssistenteDataModel>> ElaboraRisposta(long idMessaggio, [FromQuery] int idConversazione)
    {
        try
        {
            var risposta = await _messaggiService.ElaboraRispostaAssistenteAsync(idConversazione, idMessaggio);

            _logger.LogInformation("Risposta dell'assistente elaborata per messaggio {IdMessaggio}", idMessaggio);
            return Ok(risposta);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Errore durante l'elaborazione della risposta per messaggio {IdMessaggio}", idMessaggio);
            return StatusCode(500, new { Message = "Errore interno del server" });
        }
    }
}
