using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NugoloFamily.Core.Models.DTOs;

namespace NugoloFamily.Core.Services.AI;

/// <summary>
/// Provider per Anthropic Claude (Claude 3 Opus, Sonnet, Haiku)
/// </summary>
public class ClaudeProvider : BaseAIProvider
{
    private readonly string? _apiKey;
    private readonly string _baseUrl;
    private readonly string _defaultModel;
    private const string API_VERSION = "2023-06-01";

    public override string ProviderName => "Claude";

    public override List<string> SupportedModels => new()
    {
        "claude-3-opus-20240229",
        "claude-3-sonnet-20240229",
        "claude-3-haiku-20240307",
        "claude-2.1",
        "claude-2.0"
    };

    public ClaudeProvider(
        HttpClient httpClient,
        ILogger<BaseAIProvider> logger,
        IConfiguration configuration)
        : base(httpClient, logger)
    {
        _apiKey = configuration["AIProviders:Claude:ApiKey"];
        _baseUrl = configuration["AIProviders:Claude:BaseUrl"] ?? "https://api.anthropic.com/v1";
        _defaultModel = configuration["AIProviders:Claude:DefaultModel"] ?? "claude-3-sonnet-20240229";

        // Configura HttpClient
        HttpClient.BaseAddress = new Uri(_baseUrl);
        if (!string.IsNullOrEmpty(_apiKey))
        {
            HttpClient.DefaultRequestHeaders.Add("x-api-key", _apiKey);
            HttpClient.DefaultRequestHeaders.Add("anthropic-version", API_VERSION);
        }
    }

    public override async Task<AIResponseDataModel> SendMessageAsync(AIRequestDataModel request)
    {
        ValidateRequest(request);

        if (string.IsNullOrEmpty(_apiKey))
        {
            return CreateErrorResponse("Claude API key non configurata", request.Model);
        }

        try
        {
            var model = request.Model ?? _defaultModel;
            var messages = BuildClaudeMessages(request);

            var requestBody = new
            {
                model,
                max_tokens = request.MaxTokens ?? 4096,
                messages,
                system = request.SystemPrompt,
                temperature = request.Temperature
            };

            var json = JsonSerializer.Serialize(requestBody, new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            });
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            LogRequest(request, "/messages");

            var response = await HttpClient.PostAsync("/messages", content);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                Logger.LogError("[Claude] Errore HTTP {StatusCode}: {Response}",
                    response.StatusCode, responseBody);
                return CreateErrorResponse($"Errore HTTP {response.StatusCode}: {responseBody}", model);
            }

            var result = JsonSerializer.Deserialize<ClaudeResponse>(responseBody);

            if (result == null || result.Content == null || result.Content.Count == 0)
            {
                return CreateErrorResponse("Risposta Claude vuota o malformata", model);
            }

            // Claude può restituire più blocchi di contenuto, prendiamo il primo text
            var textContent = result.Content.FirstOrDefault(c => c.Type == "text");
            var responseText = textContent?.Text ?? string.Empty;

            var aiResponse = new AIResponseDataModel
            {
                Success = true,
                Content = responseText,
                Provider = ProviderName,
                Model = result.Model ?? model,
                PromptTokens = result.Usage?.InputTokens,
                CompletionTokens = result.Usage?.OutputTokens,
                TotalTokens = (result.Usage?.InputTokens ?? 0) + (result.Usage?.OutputTokens ?? 0),
                FinishReason = result.StopReason,
                Metadata = new Dictionary<string, object>
                {
                    { "id", result.Id ?? string.Empty },
                    { "type", result.Type ?? string.Empty },
                    { "role", result.Role ?? string.Empty }
                }
            };

            LogResponse(aiResponse);
            return aiResponse;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "[Claude] Errore durante l'invio del messaggio");
            return CreateErrorResponse($"Errore: {ex.Message}", request.Model);
        }
    }

    private List<object> BuildClaudeMessages(AIRequestDataModel request)
    {
        var messages = new List<object>();

        // Claude usa alternanza user/assistant, quindi adattiamo lo storico
        foreach (var msg in request.ConversationHistory)
        {
            if (msg.Role == "user" || msg.Role == "assistant")
            {
                messages.Add(new
                {
                    role = msg.Role,
                    content = msg.Content
                });
            }
        }

        // Aggiungi il messaggio corrente
        messages.Add(new
        {
            role = "user",
            content = request.UserMessage
        });

        return messages;
    }

    public override async Task<AIResponseDataModel> SendMessageStreamAsync(
        AIRequestDataModel request,
        Action<string> onChunkReceived)
    {
        // TODO: Implementare streaming con Server-Sent Events
        return await SendMessageAsync(request);
    }

    public override Task<bool> IsConfiguredAsync()
    {
        var isConfigured = !string.IsNullOrEmpty(_apiKey);
        return Task.FromResult(isConfigured);
    }

    public override decimal EstimateCost(int promptTokens, int completionTokens, string model)
    {
        // Prezzi Claude (aggiornati al 2024)
        // https://www.anthropic.com/pricing
        var costs = new Dictionary<string, (decimal input, decimal output)>
        {
            { "claude-3-opus-20240229", (15m / 1_000_000, 75m / 1_000_000) },
            { "claude-3-sonnet-20240229", (3m / 1_000_000, 15m / 1_000_000) },
            { "claude-3-haiku-20240307", (0.25m / 1_000_000, 1.25m / 1_000_000) },
            { "claude-2.1", (8m / 1_000_000, 24m / 1_000_000) },
            { "claude-2.0", (8m / 1_000_000, 24m / 1_000_000) }
        };

        if (!costs.ContainsKey(model))
        {
            model = "claude-3-sonnet-20240229"; // Default
        }

        var (inputCost, outputCost) = costs[model];
        return (promptTokens * inputCost) + (completionTokens * outputCost);
    }

    #region Claude API Models

    private class ClaudeResponse
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("role")]
        public string? Role { get; set; }

        [JsonPropertyName("content")]
        public List<ClaudeContent>? Content { get; set; }

        [JsonPropertyName("model")]
        public string? Model { get; set; }

        [JsonPropertyName("stop_reason")]
        public string? StopReason { get; set; }

        [JsonPropertyName("stop_sequence")]
        public string? StopSequence { get; set; }

        [JsonPropertyName("usage")]
        public ClaudeUsage? Usage { get; set; }
    }

    private class ClaudeContent
    {
        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("text")]
        public string? Text { get; set; }
    }

    private class ClaudeUsage
    {
        [JsonPropertyName("input_tokens")]
        public int InputTokens { get; set; }

        [JsonPropertyName("output_tokens")]
        public int OutputTokens { get; set; }
    }

    #endregion
}
