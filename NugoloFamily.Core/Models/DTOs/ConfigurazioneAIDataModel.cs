namespace NugoloFamily.Core.Models.DTOs;

/// <summary>
/// DTO per la gestione delle Configurazioni AI
/// </summary>
public class ConfigurazioneAIDataModel
{
    public int IdConfigurazioneAI { get; set; }
    public int IdFamiglia { get; set; }
    public string NomeConfigurazione { get; set; } = string.Empty;
    public string ProviderAI { get; set; } = string.Empty;
    public string Modello { get; set; } = string.Empty;
    public string? Endpoint { get; set; }
    public bool PredefinitaPerFamiglia { get; set; }
    public int? AssistenteSpecifico { get; set; }
    public string? NomeAssistente { get; set; }
    public bool Attiva { get; set; }
    public DateTime DataInserimento { get; set; }
}

/// <summary>
/// DTO per la creazione di una nuova Configurazione AI
/// </summary>
public class CreaConfigurazioneAIDataModel
{
    public int IdFamiglia { get; set; }
    public string NomeConfigurazione { get; set; } = string.Empty;
    public string ProviderAI { get; set; } = string.Empty;
    public string Modello { get; set; } = string.Empty;
    public string? ChiaveAPI { get; set; }
    public string? Endpoint { get; set; }
    public string? ParametriModello { get; set; }
    public bool PredefinitaPerFamiglia { get; set; } = false;
    public int? AssistenteSpecifico { get; set; }
}

/// <summary>
/// DTO per l'aggiornamento di una Configurazione AI
/// </summary>
public class AggiornaConfigurazioneAIDataModel
{
    public int IdConfigurazioneAI { get; set; }
    public string? NomeConfigurazione { get; set; }
    public string? ChiaveAPI { get; set; }
    public string? Endpoint { get; set; }
    public string? ParametriModello { get; set; }
    public bool? PredefinitaPerFamiglia { get; set; }
    public bool? Attiva { get; set; }
}
