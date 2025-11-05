using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NugoloFamily.Core.Models.DTOs;

namespace NugoloFamily.Core.Services.AI;

/// <summary>
/// Provider per OpenAI (ChatGPT, GPT-4, GPT-3.5)
/// </summary>
public class OpenAIProvider : BaseAIProvider
{
    private readonly string? _apiKey;
    private readonly string _baseUrl;
    private readonly string _defaultModel;

    public override string ProviderName => "OpenAI";

    public override List<string> SupportedModels => new()
    {
        "gpt-4",
        "gpt-4-turbo",
        "gpt-4-turbo-preview",
        "gpt-3.5-turbo",
        "gpt-3.5-turbo-16k"
    };

    public OpenAIProvider(
        HttpClient httpClient,
        ILogger<BaseAIProvider> logger,
        IConfiguration configuration)
        : base(httpClient, logger)
    {
        _apiKey = configuration["AIProviders:OpenAI:ApiKey"];
        _baseUrl = configuration["AIProviders:OpenAI:BaseUrl"] ?? "https://api.openai.com/v1";
        _defaultModel = configuration["AIProviders:OpenAI:DefaultModel"] ?? "gpt-3.5-turbo";

        // Configura HttpClient
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
            return CreateErrorResponse("OpenAI API key non configurata", request.Model);
        }

        try
        {
            var model = request.Model ?? _defaultModel;
            var messages = BuildConversationHistory(request);

            var requestBody = new
            {
                model,
                messages = messages.Select(m => new { role = m.Role, content = m.Content }),
                temperature = request.Temperature,
                max_tokens = request.MaxTokens,
                top_p = 1.0,
                frequency_penalty = 0.0,
                presence_penalty = 0.0
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            LogRequest(request, "/chat/completions");

            var response = await HttpClient.PostAsync("/chat/completions", content);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                Logger.LogError("[OpenAI] Errore HTTP {StatusCode}: {Response}",
                    response.StatusCode, responseBody);
                return CreateErrorResponse($"Errore HTTP {response.StatusCode}: {responseBody}", model);
            }

            var result = JsonSerializer.Deserialize<OpenAIResponse>(responseBody);

            if (result == null || result.Choices == null || result.Choices.Count == 0)
            {
                return CreateErrorResponse("Risposta OpenAI vuota o malformata", model);
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
                FinishReason = result.Choices[0].FinishReason,
                Metadata = new Dictionary<string, object>
                {
                    { "id", result.Id ?? string.Empty },
                    { "created", result.Created }
                }
            };

            LogResponse(aiResponse);
            return aiResponse;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "[OpenAI] Errore durante l'invio del messaggio");
            return CreateErrorResponse($"Errore: {ex.Message}", request.Model);
        }
    }

    public override async Task<AIResponseDataModel> SendMessageStreamAsync(
        AIRequestDataModel request,
        Action<string> onChunkReceived)
    {
        // TODO: Implementare streaming con Server-Sent Events
        // Per ora usiamo la versione non-streaming
        return await SendMessageAsync(request);
    }

    public override Task<bool> IsConfiguredAsync()
    {
        var isConfigured = !string.IsNullOrEmpty(_apiKey);
        return Task.FromResult(isConfigured);
    }

    public override decimal EstimateCost(int promptTokens, int completionTokens, string model)
    {
        // Prezzi OpenAI (aggiornati al 2024)
        // https://openai.com/pricing
        var costs = new Dictionary<string, (decimal input, decimal output)>
        {
            { "gpt-4", (0.03m / 1000, 0.06m / 1000) },
            { "gpt-4-turbo", (0.01m / 1000, 0.03m / 1000) },
            { "gpt-4-turbo-preview", (0.01m / 1000, 0.03m / 1000) },
            { "gpt-3.5-turbo", (0.0005m / 1000, 0.0015m / 1000) },
            { "gpt-3.5-turbo-16k", (0.003m / 1000, 0.004m / 1000) }
        };

        if (!costs.ContainsKey(model))
        {
            // Default a gpt-3.5-turbo
            model = "gpt-3.5-turbo";
        }

        var (inputCost, outputCost) = costs[model];
        return (promptTokens * inputCost) + (completionTokens * outputCost);
    }

    #region OpenAI API Models

    private class OpenAIResponse
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("object")]
        public string? Object { get; set; }

        [JsonPropertyName("created")]
        public long Created { get; set; }

        [JsonPropertyName("model")]
        public string? Model { get; set; }

        [JsonPropertyName("choices")]
        public List<OpenAIChoice>? Choices { get; set; }

        [JsonPropertyName("usage")]
        public OpenAIUsage? Usage { get; set; }
    }

    private class OpenAIChoice
    {
        [JsonPropertyName("index")]
        public int Index { get; set; }

        [JsonPropertyName("message")]
        public OpenAIMessage? Message { get; set; }

        [JsonPropertyName("finish_reason")]
        public string? FinishReason { get; set; }
    }

    private class OpenAIMessage
    {
        [JsonPropertyName("role")]
        public string? Role { get; set; }

        [JsonPropertyName("content")]
        public string? Content { get; set; }
    }

    private class OpenAIUsage
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
