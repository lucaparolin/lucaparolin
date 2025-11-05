using Microsoft.Extensions.Logging;
using NugoloFamily.Core.Interfaces.Services;
using NugoloFamily.Core.Models.DTOs;

namespace NugoloFamily.Core.Services.AI;

/// <summary>
/// Classe base astratta per tutti i provider AI
/// Contiene logica comune e metodi helper
/// </summary>
public abstract class BaseAIProvider : IAIProvider
{
    public abstract string ProviderName { get; }
    public abstract List<string> SupportedModels { get; }

    protected readonly HttpClient HttpClient;
    protected readonly ILogger<BaseAIProvider> Logger;

    protected BaseAIProvider(HttpClient httpClient, ILogger<BaseAIProvider> logger)
    {
        HttpClient = httpClient;
        Logger = logger;
    }

    public abstract Task<AIResponseDataModel> SendMessageAsync(AIRequestDataModel request);

    public abstract Task<AIResponseDataModel> SendMessageStreamAsync(
        AIRequestDataModel request,
        Action<string> onChunkReceived);

    public abstract Task<bool> IsConfiguredAsync();

    public abstract decimal EstimateCost(int promptTokens, int completionTokens, string model);

    /// <summary>
    /// Valida la richiesta AI
    /// </summary>
    protected virtual void ValidateRequest(AIRequestDataModel request)
    {
        if (string.IsNullOrEmpty(request.UserMessage))
        {
            throw new ArgumentException("Il messaggio dell'utente non può essere vuoto", nameof(request));
        }

        if (request.Temperature < 0 || request.Temperature > 1)
        {
            throw new ArgumentException("La temperatura deve essere tra 0 e 1", nameof(request));
        }

        if (request.MaxTokens.HasValue && request.MaxTokens.Value <= 0)
        {
            throw new ArgumentException("MaxTokens deve essere maggiore di 0", nameof(request));
        }
    }

    /// <summary>
    /// Crea una risposta di errore
    /// </summary>
    protected AIResponseDataModel CreateErrorResponse(string errorMessage, string? model = null)
    {
        return new AIResponseDataModel
        {
            Success = false,
            ErrorMessage = errorMessage,
            Provider = ProviderName,
            Model = model ?? "unknown",
            Content = string.Empty
        };
    }

    /// <summary>
    /// Prepara gli header HTTP comuni
    /// </summary>
    protected virtual Dictionary<string, string> GetCommonHeaders(string apiKey)
    {
        return new Dictionary<string, string>
        {
            { "User-Agent", "NugoloFamily/1.0" }
        };
    }

    /// <summary>
    /// Conta i token approssimativamente (implementazione semplice)
    /// Per un conteggio preciso, ogni provider dovrebbe usare il proprio tokenizer
    /// </summary>
    protected virtual int EstimateTokenCount(string text)
    {
        // Stima approssimativa: 1 token ~= 4 caratteri
        // Più accurato sarebbe usare il tokenizer specifico del modello
        return (int)Math.Ceiling(text.Length / 4.0);
    }

    /// <summary>
    /// Costruisce lo storico conversazione nel formato del provider
    /// Ogni provider può override questo metodo per il proprio formato
    /// </summary>
    protected virtual List<AIMessageDataModel> BuildConversationHistory(AIRequestDataModel request)
    {
        var messages = new List<AIMessageDataModel>();

        // Aggiungi system prompt se presente
        if (!string.IsNullOrEmpty(request.SystemPrompt))
        {
            messages.Add(new AIMessageDataModel
            {
                Role = "system",
                Content = request.SystemPrompt
            });
        }

        // Aggiungi storico conversazione
        messages.AddRange(request.ConversationHistory);

        // Aggiungi il messaggio corrente dell'utente
        messages.Add(new AIMessageDataModel
        {
            Role = "user",
            Content = request.UserMessage
        });

        return messages;
    }

    /// <summary>
    /// Log della richiesta AI (per debugging e monitoring)
    /// </summary>
    protected void LogRequest(AIRequestDataModel request, string endpoint)
    {
        Logger.LogInformation(
            "[{Provider}] Invio richiesta a {Endpoint} - Model: {Model}, Temp: {Temperature}, MaxTokens: {MaxTokens}",
            ProviderName,
            endpoint,
            request.Model ?? "default",
            request.Temperature,
            request.MaxTokens);
    }

    /// <summary>
    /// Log della risposta AI (per debugging e monitoring)
    /// </summary>
    protected void LogResponse(AIResponseDataModel response)
    {
        if (response.Success)
        {
            Logger.LogInformation(
                "[{Provider}] Risposta ricevuta - Model: {Model}, Tokens: {Total} (Prompt: {Prompt}, Completion: {Completion}), FinishReason: {Reason}",
                response.Provider,
                response.Model,
                response.TotalTokens,
                response.PromptTokens,
                response.CompletionTokens,
                response.FinishReason);
        }
        else
        {
            Logger.LogError(
                "[{Provider}] Errore nella risposta - Model: {Model}, Error: {Error}",
                response.Provider,
                response.Model,
                response.ErrorMessage);
        }
    }
}
