using Microsoft.AspNetCore.Mvc;
using NugoloFamily.Core.Interfaces.Repositories;
using NugoloFamily.Core.Models.DTOs;
using NugoloFamily.Core.Models.Entities;

namespace NugoloFamily.API.Controllers;

/// <summary>
/// Controller per la gestione delle Famiglie
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class FamiglieController : ControllerBase
{
    private readonly IFamiglieRepository _famiglieRepository;
    private readonly ILogger<FamiglieController> _logger;

    public FamiglieController(
        IFamiglieRepository famiglieRepository,
        ILogger<FamiglieController> logger)
    {
        _famiglieRepository = famiglieRepository;
        _logger = logger;
    }

    /// <summary>
    /// Ottiene tutte le famiglie
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<FamigliaDataModel>>> GetAll()
    {
        try
        {
            var famiglie = await _famiglieRepository.GetAllAsync();
            var result = famiglie.Select(f => MapToDataModel(f));
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Errore durante il recupero delle famiglie");
            return StatusCode(500, "Errore interno del server");
        }
    }

    /// <summary>
    /// Ottiene una famiglia per ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<FamigliaDataModel>> GetById(int id)
    {
        try
        {
            var famiglia = await _famiglieRepository.GetByIdAsync(id);

            if (famiglia == null)
            {
                return NotFound($"Famiglia con ID {id} non trovata");
            }

            return Ok(MapToDataModel(famiglia));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Errore durante il recupero della famiglia {IdFamiglia}", id);
            return StatusCode(500, "Errore interno del server");
        }
    }

    /// <summary>
    /// Ottiene una famiglia per codice univoco
    /// </summary>
    [HttpGet("codice/{codice}")]
    public async Task<ActionResult<FamigliaDataModel>> GetByCodice(string codice)
    {
        try
        {
            var famiglia = await _famiglieRepository.GetByCodiceUnivocoAsync(codice);

            if (famiglia == null)
            {
                return NotFound($"Famiglia con codice {codice} non trovata");
            }

            return Ok(MapToDataModel(famiglia));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Errore durante il recupero della famiglia con codice {Codice}", codice);
            return StatusCode(500, "Errore interno del server");
        }
    }

    /// <summary>
    /// Crea una nuova famiglia
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<FamigliaDataModel>> Create([FromBody] CreaFamigliaDataModel model)
    {
        try
        {
            // Validazioni
            if (await _famiglieRepository.ExistsByEmailAsync(model.Email))
            {
                return BadRequest("Esiste già una famiglia con questa email");
            }

            // Genera codice univoco
            var codiceUnivoco = GeneraCodiceUnivoco(model.NomeFamiglia);
            while (await _famiglieRepository.ExistsByCodiceUnivocoAsync(codiceUnivoco))
            {
                codiceUnivoco = GeneraCodiceUnivoco(model.NomeFamiglia);
            }

            var entity = new FamigliaEntityModel
            {
                NomeFamiglia = model.NomeFamiglia,
                CodiceUnivoco = codiceUnivoco,
                Email = model.Email,
                Telefono = model.Telefono,
                Indirizzo = model.Indirizzo,
                StatoAttivazione = "Attivo",
                PianoAbbonamento = model.PianoAbbonamento,
                LimiteUtenti = 5,
                DataInserimento = DateTime.Now,
                UtenteInserimento = "SYSTEM", // TODO: Recuperare da contesto autenticazione
                IPInserimento = HttpContext.Connection.RemoteIpAddress?.ToString()
            };

            var idFamiglia = await _famiglieRepository.CreateAsync(entity);
            entity.IdFamiglia = idFamiglia;

            return CreatedAtAction(nameof(GetById), new { id = idFamiglia }, MapToDataModel(entity));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Errore durante la creazione della famiglia");
            return StatusCode(500, "Errore interno del server");
        }
    }

    /// <summary>
    /// Aggiorna una famiglia esistente
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, [FromBody] AggiornaFamigliaDataModel model)
    {
        try
        {
            var famiglia = await _famiglieRepository.GetByIdAsync(id);

            if (famiglia == null)
            {
                return NotFound($"Famiglia con ID {id} non trovata");
            }

            // Aggiorna solo i campi forniti
            if (!string.IsNullOrEmpty(model.NomeFamiglia))
                famiglia.NomeFamiglia = model.NomeFamiglia;

            if (!string.IsNullOrEmpty(model.Email))
                famiglia.Email = model.Email;

            if (model.Telefono != null)
                famiglia.Telefono = model.Telefono;

            if (model.Indirizzo != null)
                famiglia.Indirizzo = model.Indirizzo;

            if (!string.IsNullOrEmpty(model.StatoAttivazione))
                famiglia.StatoAttivazione = model.StatoAttivazione;

            if (model.DataScadenzaAbbonamento.HasValue)
                famiglia.DataScadenzaAbbonamento = model.DataScadenzaAbbonamento;

            if (!string.IsNullOrEmpty(model.PianoAbbonamento))
                famiglia.PianoAbbonamento = model.PianoAbbonamento;

            famiglia.DataModifica = DateTime.Now;
            famiglia.UtenteModifica = "SYSTEM"; // TODO: Recuperare da contesto autenticazione
            famiglia.IPModifica = HttpContext.Connection.RemoteIpAddress?.ToString();

            var result = await _famiglieRepository.UpdateAsync(famiglia);

            if (!result)
            {
                return StatusCode(500, "Errore durante l'aggiornamento della famiglia");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Errore durante l'aggiornamento della famiglia {IdFamiglia}", id);
            return StatusCode(500, "Errore interno del server");
        }
    }

    /// <summary>
    /// Elimina una famiglia
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            var famiglia = await _famiglieRepository.GetByIdAsync(id);

            if (famiglia == null)
            {
                return NotFound($"Famiglia con ID {id} non trovata");
            }

            var result = await _famiglieRepository.DeleteAsync(id);

            if (!result)
            {
                return StatusCode(500, "Errore durante l'eliminazione della famiglia");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Errore durante l'eliminazione della famiglia {IdFamiglia}", id);
            return StatusCode(500, "Errore interno del server");
        }
    }

    // Helper methods

    private FamigliaDataModel MapToDataModel(FamigliaEntityModel entity)
    {
        return new FamigliaDataModel
        {
            IdFamiglia = entity.IdFamiglia,
            NomeFamiglia = entity.NomeFamiglia,
            CodiceUnivoco = entity.CodiceUnivoco,
            Email = entity.Email,
            Telefono = entity.Telefono,
            Indirizzo = entity.Indirizzo,
            StatoAttivazione = entity.StatoAttivazione,
            DataScadenzaAbbonamento = entity.DataScadenzaAbbonamento,
            PianoAbbonamento = entity.PianoAbbonamento,
            LimiteUtenti = entity.LimiteUtenti,
            DataInserimento = entity.DataInserimento
        };
    }

    private string GeneraCodiceUnivoco(string nomeFamiglia)
    {
        var timestamp = DateTime.Now.ToString("yyyyMMdd");
        var random = new Random().Next(1000, 9999);
        var nomeNormalizzato = new string(nomeFamiglia.Take(5).ToArray()).ToUpper();
        return $"FAM-{nomeNormalizzato}-{timestamp}-{random}";
    }
}
