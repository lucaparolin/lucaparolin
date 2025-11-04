using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NugoloFamily.Core.Interfaces.Services;
using NugoloFamily.Core.Models.DTOs;

namespace NugoloFamily.API.Controllers;

/// <summary>
/// Controller per la gestione delle Conversazioni
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ConversazioniController : ControllerBase
{
    private readonly IConversazioniService _conversazioniService;
    private readonly ILogger<ConversazioniController> _logger;

    public ConversazioniController(
        IConversazioniService conversazioniService,
        ILogger<ConversazioniController> logger)
    {
        _conversazioniService = conversazioniService;
        _logger = logger;
    }

    /// <summary>
    /// Ottiene una conversazione per ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ConversazioneDataModel>> GetById(int id)
    {
        try
        {
            var conversazione = await _conversazioniService.GetByIdAsync(id);

            if (conversazione == null)
            {
                return NotFound(new { Message = $"Conversazione con ID {id} non trovata" });
            }

            return Ok(conversazione);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Errore durante il recupero della conversazione {IdConversazione}", id);
            return StatusCode(500, new { Message = "Errore interno del server" });
        }
    }

    /// <summary>
    /// Ottiene il dettaglio completo di una conversazione con messaggi
    /// </summary>
    [HttpGet("{id}/dettaglio")]
    public async Task<ActionResult<ConversazioneDettaglioDataModel>> GetDettaglioById(int id)
    {
        try
        {
            var conversazione = await _conversazioniService.GetDettaglioByIdAsync(id);

            if (conversazione == null)
            {
                return NotFound(new { Message = $"Conversazione con ID {id} non trovata" });
            }

            return Ok(conversazione);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Errore durante il recupero del dettaglio della conversazione {IdConversazione}", id);
            return StatusCode(500, new { Message = "Errore interno del server" });
        }
    }

    /// <summary>
    /// Ottiene tutte le conversazioni di un utente
    /// </summary>
    [HttpGet("utente/{idUtente}")]
    public async Task<ActionResult<IEnumerable<ConversazioneDataModel>>> GetByUtente(int idUtente)
    {
        try
        {
            var conversazioni = await _conversazioniService.GetByUtenteAsync(idUtente);
            return Ok(conversazioni);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Errore durante il recupero delle conversazioni dell'utente {IdUtente}", idUtente);
            return StatusCode(500, new { Message = "Errore interno del server" });
        }
    }

    /// <summary>
    /// Ottiene le conversazioni di un utente con un assistente specifico
    /// </summary>
    [HttpGet("utente/{idUtente}/assistente/{idAssistente}")]
    public async Task<ActionResult<IEnumerable<ConversazioneDataModel>>> GetByUtenteAndAssistente(int idUtente, int idAssistente)
    {
        try
        {
            var conversazioni = await _conversazioniService.GetByUtenteAndAssistenteAsync(idUtente, idAssistente);
            return Ok(conversazioni);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Errore durante il recupero delle conversazioni utente {IdUtente} con assistente {IdAssistente}", idUtente, idAssistente);
            return StatusCode(500, new { Message = "Errore interno del server" });
        }
    }

    /// <summary>
    /// Crea una nuova conversazione
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ConversazioneDataModel>> Create([FromBody] CreaConversazioneDataModel model)
    {
        try
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            var utenteCorrente = User.Identity?.Name ?? "UNKNOWN";

            var conversazione = await _conversazioniService.CreateAsync(model, utenteCorrente, ipAddress);

            _logger.LogInformation("Conversazione {IdConversazione} creata con successo", conversazione.IdConversazione);
            return CreatedAtAction(nameof(GetById), new { id = conversazione.IdConversazione }, conversazione);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Errore durante la creazione della conversazione");
            return StatusCode(500, new { Message = "Errore interno del server" });
        }
    }

    /// <summary>
    /// Chiude una conversazione
    /// </summary>
    [HttpPost("{id}/chiudi")]
    public async Task<ActionResult> Chiudi(int id)
    {
        try
        {
            var risultato = await _conversazioniService.ChiudiConversazioneAsync(id);

            if (!risultato)
            {
                return StatusCode(500, new { Message = "Errore durante la chiusura della conversazione" });
            }

            _logger.LogInformation("Conversazione {IdConversazione} chiusa", id);
            return Ok(new { Message = "Conversazione chiusa con successo" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Errore durante la chiusura della conversazione {IdConversazione}", id);
            return StatusCode(500, new { Message = "Errore interno del server" });
        }
    }

    /// <summary>
    /// Archivia una conversazione
    /// </summary>
    [HttpPost("{id}/archivia")]
    public async Task<ActionResult> Archivia(int id)
    {
        try
        {
            var risultato = await _conversazioniService.ArchivaConversazioneAsync(id);

            if (!risultato)
            {
                return StatusCode(500, new { Message = "Errore durante l'archiviazione della conversazione" });
            }

            _logger.LogInformation("Conversazione {IdConversazione} archiviata", id);
            return Ok(new { Message = "Conversazione archiviata con successo" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Errore durante l'archiviazione della conversazione {IdConversazione}", id);
            return StatusCode(500, new { Message = "Errore interno del server" });
        }
    }
}
