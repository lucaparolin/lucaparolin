namespace NugoloFamily.Core.Models.Entities;

/// <summary>
/// Modello Entity per la tabella ConsumiAI
/// Rappresenta il tracciamento dei consumi AI per billing
/// </summary>
public class ConsumoAIEntityModel
{
    public long IdConsumo { get; set; }
    public int IdFamiglia { get; set; }
    public int? IdUtente { get; set; }
    public int? IdConfigurazioneAI { get; set; }
    public long? IdMessaggio { get; set; }
    public string ProviderAI { get; set; } = string.Empty;
    public string? Modello { get; set; }
    public int TokenInput { get; set; } = 0;
    public int TokenOutput { get; set; } = 0;
    public int TokenTotali { get; set; } = 0;
    public decimal? CostoStimato { get; set; }
    public int? DurataElaborazione { get; set; }
    public string? TipoRichiesta { get; set; }
    public DateTime DataInserimento { get; set; }
}
