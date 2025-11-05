namespace NugoloFamily.Core.Models.DTOs;

/// <summary>
/// Richiesta per inviare un messaggio a un provider AI
/// </summary>
public class AIRequestDataModel
{
    /// <summary>
    /// Provider AI da utilizzare (OpenAI, Claude, DeepSeek, Gemini)
    /// </summary>
    public string Provider { get; set; } = string.Empty;

    /// <summary>
    /// Modello specifico da utilizzare (es: gpt-4, claude-3-opus, ecc.)
    /// </summary>
    public string? Model { get; set; }

    /// <summary>
    /// Messaggio dell'utente
    /// </summary>
    public string UserMessage { get; set; } = string.Empty;

    /// <summary>
    /// Contesto della conversazione (messaggi precedenti)
    /// </summary>
    public List<AIMessageDataModel> ConversationHistory { get; set; } = new();

    /// <summary>
    /// System prompt/istruzioni per l'assistente
    /// </summary>
    public string? SystemPrompt { get; set; }

    /// <summary>
    /// Temperatura (creatività) - 0.0 a 1.0
    /// </summary>
    public double Temperature { get; set; } = 0.7;

    /// <summary>
    /// Numero massimo di token nella risposta
    /// </summary>
    public int? MaxTokens { get; set; }

    /// <summary>
    /// Metadati aggiuntivi specifici del provider
    /// </summary>
    public Dictionary<string, object>? Metadata { get; set; }
}

/// <summary>
/// Singolo messaggio in una conversazione AI
/// </summary>
public class AIMessageDataModel
{
    /// <summary>
    /// Ruolo: system, user, assistant
    /// </summary>
    public string Role { get; set; } = string.Empty;

    /// <summary>
    /// Contenuto del messaggio
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Nome opzionale (per identificare l'utente)
    /// </summary>
    public string? Name { get; set; }
}

/// <summary>
/// Risposta da un provider AI
/// </summary>
public class AIResponseDataModel
{
    /// <summary>
    /// Indica se la richiesta ha avuto successo
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Messaggio generato dall'AI
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Provider che ha generato la risposta
    /// </summary>
    public string Provider { get; set; } = string.Empty;

    /// <summary>
    /// Modello utilizzato
    /// </summary>
    public string Model { get; set; } = string.Empty;

    /// <summary>
    /// Token utilizzati nella richiesta
    /// </summary>
    public int? PromptTokens { get; set; }

    /// <summary>
    /// Token utilizzati nella risposta
    /// </summary>
    public int? CompletionTokens { get; set; }

    /// <summary>
    /// Token totali utilizzati
    /// </summary>
    public int? TotalTokens { get; set; }

    /// <summary>
    /// Motivo di terminazione (stop, length, content_filter, ecc.)
    /// </summary>
    public string? FinishReason { get; set; }

    /// <summary>
    /// Messaggio di errore se Success = false
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Metadati aggiuntivi dalla risposta
    /// </summary>
    public Dictionary<string, object>? Metadata { get; set; }
}
