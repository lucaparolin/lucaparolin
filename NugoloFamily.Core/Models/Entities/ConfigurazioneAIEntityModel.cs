namespace NugoloFamily.Core.Models.Entities;

/// <summary>
/// Modello Entity per la tabella ConfigurazioniAI
/// Rappresenta la configurazione di un modello AI per una famiglia
/// </summary>
public class ConfigurazioneAIEntityModel
{
    public int IdConfigurazioneAI { get; set; }
    public int IdFamiglia { get; set; }
    public string NomeConfigurazione { get; set; } = string.Empty;
    public string ProviderAI { get; set; } = string.Empty;
    public string Modello { get; set; } = string.Empty;
    public string? ChiaveAPI { get; set; }
    public string? Endpoint { get; set; }
    public string? ParametriModello { get; set; }
    public bool PredefinitaPerFamiglia { get; set; } = false;
    public int? AssistenteSpecifico { get; set; }
    public bool Attiva { get; set; } = true;

    // Campi di tracciamento
    public DateTime DataInserimento { get; set; }
    public string UtenteInserimento { get; set; } = string.Empty;
    public string? IPInserimento { get; set; }
    public DateTime? DataModifica { get; set; }
    public string? UtenteModifica { get; set; }
    public string? IPModifica { get; set; }
}
