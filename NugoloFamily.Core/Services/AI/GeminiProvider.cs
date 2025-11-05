using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NugoloFamily.Core.Models.DTOs;

namespace NugoloFamily.Core.Services.AI;

/// <summary>
/// Provider per Google Gemini (Gemini Pro, Gemini Ultra)
/// </summary>
public class GeminiProvider : BaseAIProvider
{
    private readonly string? _apiKey;
    private readonly string _baseUrl;
    private readonly string _defaultModel;

    public override string ProviderName => "Gemini";

    public override List<string> SupportedModels => new()
    {
        "gemini-pro",
        "gemini-pro-vision",
        "gemini-ultra"
    };

    public GeminiProvider(
        HttpClient httpClient,
        ILogger<BaseAIProvider> logger,
        IConfiguration configuration)
        : base(httpClient, logger)
    {
        _apiKey = configuration["AIProviders:Gemini:ApiKey"];
        _baseUrl = configuration["AIProviders:Gemini:BaseUrl"] ?? "https://generativelanguage.googleapis.com/v1";
        _defaultModel = configuration["AIProviders:Gemini:DefaultModel"] ?? "gemini-pro";

        HttpClient.BaseAddress = new Uri(_baseUrl);
    }

    public override async Task<AIResponseDataModel> SendMessageAsync(AIRequestDataModel request)
    {
        ValidateRequest(request);

        if (string.IsNullOrEmpty(_apiKey))
        {
            return CreateErrorResponse("Gemini API key non configurata", request.Model);
        }

        try
        {
            var model = request.Model ?? _defaultModel;

            // Gemini usa un formato diverso per i messaggi
            var contents = BuildGeminiContents(request);

            var requestBody = new
            {
                contents,
                generationConfig = new
                {
                    temperature = request.Temperature,
                    maxOutputTokens = request.MaxTokens,
                    topP = 1.0,
                    topK = 1
                }
            };

            var json = JsonSerializer.Serialize(requestBody, new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            });
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var endpoint = $"/models/{model}:generateContent?key={_apiKey}";
            LogRequest(request, endpoint);

            var response = await HttpClient.PostAsync(endpoint, content);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                Logger.LogError("[Gemini] Errore HTTP {StatusCode}: {Response}",
                    response.StatusCode, responseBody);
                return CreateErrorResponse($"Errore HTTP {response.StatusCode}", model);
            }

            var result = JsonSerializer.Deserialize<GeminiResponse>(responseBody);

            if (result == null || result.Candidates == null || result.Candidates.Count == 0)
            {
                return CreateErrorResponse("Risposta Gemini vuota", model);
            }

            var candidate = result.Candidates[0];
            var text = candidate.Content?.Parts?.FirstOrDefault()?.Text ?? string.Empty;

            var aiResponse = new AIResponseDataModel
            {
                Success = true,
                Content = text,
                Provider = ProviderName,
                Model = model,
                PromptTokens = result.UsageMetadata?.PromptTokenCount,
                CompletionTokens = result.UsageMetadata?.CandidatesTokenCount,
                TotalTokens = result.UsageMetadata?.TotalTokenCount,
                FinishReason = candidate.FinishReason,
                Metadata = new Dictionary<string, object>
                {
                    { "safetyRatings", candidate.SafetyRatings ?? new List<object>() }
                }
            };

            LogResponse(aiResponse);
            return aiResponse;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "[Gemini] Errore durante l'invio del messaggio");
            return CreateErrorResponse($"Errore: {ex.Message}", request.Model);
        }
    }

    private List<object> BuildGeminiContents(AIRequestDataModel request)
    {
        var contents = new List<object>();

        // Gemini usa "user" e "model" invece di "user" e "assistant"
        foreach (var msg in request.ConversationHistory)
        {
            var role = msg.Role == "assistant" ? "model" : msg.Role;
            if (role == "system")
            {
                // Gemini non ha un ruolo "system" separato, lo includiamo nel primo messaggio user
                continue;
            }

            contents.Add(new
            {
                role,
                parts = new[] { new { text = msg.Content } }
            });
        }

        // Aggiungi system prompt come parte del primo messaggio user se presente
        var userMessage = request.UserMessage;
        if (!string.IsNullOrEmpty(request.SystemPrompt))
        {
            userMessage = $"{request.SystemPrompt}\n\nUser: {userMessage}";
        }

        contents.Add(new
        {
            role = "user",
            parts = new[] { new { text = userMessage } }
        });

        return contents;
    }

    public override async Task<AIResponseDataModel> SendMessageStreamAsync(
        AIRequestDataModel request,
        Action<string> onChunkReceived)
    {
        return await SendMessageAsync(request);
    }

    public override Task<bool> IsConfiguredAsync()
    {
        return Task.FromResult(!string.IsNullOrEmpty(_apiKey));
    }

    public override decimal EstimateCost(int promptTokens, int completionTokens, string model)
    {
        // Prezzi Gemini (da verificare sul sito ufficiale Google AI)
        // Pro è gratuito fino a un certo limite, poi a pagamento
        var inputCost = 0.00025m / 1000;   // $0.00025 per 1K tokens
        var outputCost = 0.0005m / 1000;   // $0.0005 per 1K tokens

        return (promptTokens * inputCost) + (completionTokens * outputCost);
    }

    #region Gemini API Models

    private class GeminiResponse
    {
        [JsonPropertyName("candidates")]
        public List<GeminiCandidate>? Candidates { get; set; }

        [JsonPropertyName("usageMetadata")]
        public GeminiUsageMetadata? UsageMetadata { get; set; }
    }

    private class GeminiCandidate
    {
        [JsonPropertyName("content")]
        public GeminiContent? Content { get; set; }

        [JsonPropertyName("finishReason")]
        public string? FinishReason { get; set; }

        [JsonPropertyName("safetyRatings")]
        public List<object>? SafetyRatings { get; set; }
    }

    private class GeminiContent
    {
        [JsonPropertyName("parts")]
        public List<GeminiPart>? Parts { get; set; }

        [JsonPropertyName("role")]
        public string? Role { get; set; }
    }

    private class GeminiPart
    {
        [JsonPropertyName("text")]
        public string? Text { get; set; }
    }

    private class GeminiUsageMetadata
    {
        [JsonPropertyName("promptTokenCount")]
        public int PromptTokenCount { get; set; }

        [JsonPropertyName("candidatesTokenCount")]
        public int CandidatesTokenCount { get; set; }

        [JsonPropertyName("totalTokenCount")]
        public int TotalTokenCount { get; set; }
    }

    #endregion
}
