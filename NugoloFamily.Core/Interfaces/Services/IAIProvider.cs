using NugoloFamily.Core.Models.DTOs;

namespace NugoloFamily.Core.Interfaces.Services;

/// <summary>
/// Interfaccia generica per tutti i provider AI
/// Implementata da OpenAI, Claude, DeepSeek, Gemini
/// </summary>
public interface IAIProvider
{
    /// <summary>
    /// Nome del provider (es: "OpenAI", "Claude", "DeepSeek", "Gemini")
    /// </summary>
    string ProviderName { get; }

    /// <summary>
    /// Modelli supportati da questo provider
    /// </summary>
    List<string> SupportedModels { get; }

    /// <summary>
    /// Invia un messaggio all'AI e ottieni una risposta
    /// </summary>
    /// <param name="request">Richiesta con messaggio e contesto</param>
    /// <returns>Risposta generata dall'AI</returns>
    Task<AIResponseDataModel> SendMessageAsync(AIRequestDataModel request);

    /// <summary>
    /// Invia un messaggio in streaming (per risposte progressive)
    /// </summary>
    /// <param name="request">Richiesta con messaggio e contesto</param>
    /// <param name="onChunkReceived">Callback chiamata per ogni chunk ricevuto</param>
    /// <returns>Risposta completa finale</returns>
    Task<AIResponseDataModel> SendMessageStreamAsync(
        AIRequestDataModel request,
        Action<string> onChunkReceived);

    /// <summary>
    /// Verifica se il provider è configurato correttamente
    /// </summary>
    /// <returns>True se configurato e pronto all'uso</returns>
    Task<bool> IsConfiguredAsync();

    /// <summary>
    /// Ottieni informazioni sui costi stimati per una richiesta
    /// </summary>
    /// <param name="promptTokens">Token nel prompt</param>
    /// <param name="completionTokens">Token nella completion</param>
    /// <param name="model">Modello utilizzato</param>
    /// <returns>Costo stimato in USD</returns>
    decimal EstimateCost(int promptTokens, int completionTokens, string model);
}
