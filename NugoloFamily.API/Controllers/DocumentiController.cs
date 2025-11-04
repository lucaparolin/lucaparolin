using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NugoloFamily.Core.Interfaces.Services;
using NugoloFamily.Core.Models.DTOs;

namespace NugoloFamily.API.Controllers;

/// <summary>
/// Controller per la gestione dei Documenti
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DocumentiController : ControllerBase
{
    private readonly IDocumentiService _documentiService;
    private readonly ILogger<DocumentiController> _logger;

    public DocumentiController(
        IDocumentiService documentiService,
        ILogger<DocumentiController> logger)
    {
        _documentiService = documentiService;
        _logger = logger;
    }

    /// <summary>
    /// Ottiene un documento per ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<DocumentoDataModel>> GetById(int id)
    {
        try
        {
            var documento = await _documentiService.GetByIdAsync(id);

            if (documento == null)
            {
                return NotFound(new { Message = $"Documento con ID {id} non trovato" });
            }

            return Ok(documento);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Errore durante il recupero del documento {IdDocumento}", id);
            return StatusCode(500, new { Message = "Errore interno del server" });
        }
    }

    /// <summary>
    /// Ottiene il dettaglio completo di un documento
    /// </summary>
    [HttpGet("{id}/dettaglio")]
    public async Task<ActionResult<DocumentoDettaglioDataModel>> GetDettaglioById(int id)
    {
        try
        {
            var documento = await _documentiService.GetDettaglioByIdAsync(id);

            if (documento == null)
            {
                return NotFound(new { Message = $"Documento con ID {id} non trovato" });
            }

            return Ok(documento);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Errore durante il recupero del dettaglio del documento {IdDocumento}", id);
            return StatusCode(500, new { Message = "Errore interno del server" });
        }
    }

    /// <summary>
    /// Ottiene tutti i documenti di una famiglia
    /// </summary>
    [HttpGet("famiglia/{idFamiglia}")]
    public async Task<ActionResult<IEnumerable<DocumentoDataModel>>> GetByFamiglia(int idFamiglia)
    {
        try
        {
            var documenti = await _documentiService.GetByFamigliaAsync(idFamiglia);
            return Ok(documenti);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Errore durante il recupero dei documenti della famiglia {IdFamiglia}", idFamiglia);
            return StatusCode(500, new { Message = "Errore interno del server" });
        }
    }

    /// <summary>
    /// Ottiene i documenti per tipo
    /// </summary>
    [HttpGet("famiglia/{idFamiglia}/tipo/{tipoDocumento}")]
    public async Task<ActionResult<IEnumerable<DocumentoDataModel>>> GetByTipo(int idFamiglia, string tipoDocumento)
    {
        try
        {
            var documenti = await _documentiService.GetByTipoAsync(idFamiglia, tipoDocumento);
            return Ok(documenti);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Errore durante il recupero dei documenti per tipo {TipoDocumento}", tipoDocumento);
            return StatusCode(500, new { Message = "Errore interno del server" });
        }
    }

    /// <summary>
    /// Ottiene i documenti per categoria
    /// </summary>
    [HttpGet("famiglia/{idFamiglia}/categoria/{categoria}")]
    public async Task<ActionResult<IEnumerable<DocumentoDataModel>>> GetByCategoria(int idFamiglia, string categoria)
    {
        try
        {
            var documenti = await _documentiService.GetByCategoriaAsync(idFamiglia, categoria);
            return Ok(documenti);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Errore durante il recupero dei documenti per categoria {Categoria}", categoria);
            return StatusCode(500, new { Message = "Errore interno del server" });
        }
    }

    /// <summary>
    /// Upload di un nuovo documento
    /// </summary>
    [HttpPost("upload")]
    public async Task<ActionResult<DocumentoDataModel>> Upload([FromBody] CaricaDocumentoDataModel model)
    {
        try
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            var utenteCorrente = User.Identity?.Name ?? "UNKNOWN";

            var documento = await _documentiService.UploadAsync(model, utenteCorrente, ipAddress);

            _logger.LogInformation("Documento {IdDocumento} caricato con successo", documento.IdDocumento);
            return CreatedAtAction(nameof(GetById), new { id = documento.IdDocumento }, documento);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Errore durante l'upload del documento");
            return StatusCode(500, new { Message = "Errore interno del server" });
        }
    }

    /// <summary>
    /// Elimina un documento
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            var risultato = await _documentiService.DeleteAsync(id);

            if (!risultato)
            {
                return StatusCode(500, new { Message = "Errore durante l'eliminazione del documento" });
            }

            _logger.LogInformation("Documento {IdDocumento} eliminato con successo", id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Errore durante l'eliminazione del documento {IdDocumento}", id);
            return StatusCode(500, new { Message = "Errore interno del server" });
        }
    }

    /// <summary>
    /// Elabora un documento (estrazione testo con Textract)
    /// </summary>
    [HttpPost("{id}/elabora")]
    public async Task<ActionResult> Elabora(int id)
    {
        try
        {
            var risultato = await _documentiService.ElaboraDocumentoAsync(id);

            if (!risultato)
            {
                return StatusCode(500, new { Message = "Errore durante l'elaborazione del documento" });
            }

            _logger.LogInformation("Documento {IdDocumento} elaborato con successo", id);
            return Ok(new { Message = "Documento elaborato con successo" });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Errore durante l'elaborazione del documento {IdDocumento}", id);
            return StatusCode(500, new { Message = "Errore interno del server" });
        }
    }
}
