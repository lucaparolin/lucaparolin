using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NugoloFamily.Core.Models.DTOs;

namespace NugoloFamily.Core.Services.AI;

/// <summary>
/// Provider per DeepSeek AI
/// API compatibile con OpenAI
/// </summary>
public class DeepSeekProvider : BaseAIProvider
{
    private readonly string? _apiKey;
    private readonly string _baseUrl;
    private readonly string _defaultModel;

    public override string ProviderName => "DeepSeek";

    public override List<string> SupportedModels => new()
    {
        "deepseek-chat",
        "deepseek-coder"
    };

    public DeepSeekProvider(
        HttpClient httpClient,
        ILogger<BaseAIProvider> logger,
        IConfiguration configuration)
        : base(httpClient, logger)
    {
        _apiKey = configuration["AIProviders:DeepSeek:ApiKey"];
        _baseUrl = configuration["AIProviders:DeepSeek:BaseUrl"] ?? "https://api.deepseek.com/v1";
        _defaultModel = configuration["AIProviders:DeepSeek:DefaultModel"] ?? "deepseek-chat";

        HttpClient.BaseAddress = new Uri(_baseUrl);
        if (!string.IsNullOrEmpty(_apiKey))
        {
            HttpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
        }
    }

    public override async Task<AIResponseDataModel> SendMessageAsync(AIRequestDataModel request)
    {
        ValidateRequest(request);

        if (string.IsNullOrEmpty(_apiKey))
        {
            return CreateErrorResponse("DeepSeek API key non configurata", request.Model);
        }

        try
        {
            var model = request.Model ?? _defaultModel;
            var messages = BuildConversationHistory(request);

            // DeepSeek usa API compatibile con OpenAI
            var requestBody = new
            {
                model,
                messages = messages.Select(m => new { role = m.Role, content = m.Content }),
                temperature = request.Temperature,
                max_tokens = request.MaxTokens
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            LogRequest(request, "/chat/completions");

            var response = await HttpClient.PostAsync("/chat/completions", content);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                Logger.LogError("[DeepSeek] Errore HTTP {StatusCode}: {Response}",
                    response.StatusCode, responseBody);
                return CreateErrorResponse($"Errore HTTP {response.StatusCode}", model);
            }

            var result = JsonSerializer.Deserialize<DeepSeekResponse>(responseBody);

            if (result == null || result.Choices == null || result.Choices.Count == 0)
            {
                return CreateErrorResponse("Risposta DeepSeek vuota", model);
            }

            var aiResponse = new AIResponseDataModel
            {
                Success = true,
                Content = result.Choices[0].Message?.Content ?? string.Empty,
                Provider = ProviderName,
                Model = result.Model ?? model,
                PromptTokens = result.Usage?.PromptTokens,
                CompletionTokens = result.Usage?.CompletionTokens,
                TotalTokens = result.Usage?.TotalTokens,
                FinishReason = result.Choices[0].FinishReason
            };

            LogResponse(aiResponse);
            return aiResponse;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "[DeepSeek] Errore durante l'invio del messaggio");
            return CreateErrorResponse($"Errore: {ex.Message}", request.Model);
        }
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
        // Prezzi indicativi DeepSeek (da verificare sul sito ufficiale)
        var inputCost = 0.0001m / 1000;  // $0.0001 per 1K tokens
        var outputCost = 0.0002m / 1000; // $0.0002 per 1K tokens

        return (promptTokens * inputCost) + (completionTokens * outputCost);
    }

    #region DeepSeek API Models (compatibile con OpenAI)

    private class DeepSeekResponse
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("model")]
        public string? Model { get; set; }

        [JsonPropertyName("choices")]
        public List<DeepSeekChoice>? Choices { get; set; }

        [JsonPropertyName("usage")]
        public DeepSeekUsage? Usage { get; set; }
    }

    private class DeepSeekChoice
    {
        [JsonPropertyName("index")]
        public int Index { get; set; }

        [JsonPropertyName("message")]
        public DeepSeekMessage? Message { get; set; }

        [JsonPropertyName("finish_reason")]
        public string? FinishReason { get; set; }
    }

    private class DeepSeekMessage
    {
        [JsonPropertyName("role")]
        public string? Role { get; set; }

        [JsonPropertyName("content")]
        public string? Content { get; set; }
    }

    private class DeepSeekUsage
    {
        [JsonPropertyName("prompt_tokens")]
        public int PromptTokens { get; set; }

        [JsonPropertyName("completion_tokens")]
        public int CompletionTokens { get; set; }

        [JsonPropertyName("total_tokens")]
        public int TotalTokens { get; set; }
    }

    #endregion
}
