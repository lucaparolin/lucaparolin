using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NugoloFamily.Core.Interfaces.Services;

namespace NugoloFamily.Core.Services.AI;

/// <summary>
/// Factory per creare istanze dei provider AI
/// </summary>
public class AIProviderFactory
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<BaseAIProvider> _logger;
    private readonly IConfiguration _configuration;
    private readonly Dictionary<string, IAIProvider> _providers;

    public AIProviderFactory(
        IHttpClientFactory httpClientFactory,
        ILogger<BaseAIProvider> logger,
        IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _configuration = configuration;
        _providers = new Dictionary<string, IAIProvider>(StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Ottieni un provider AI specifico
    /// </summary>
    /// <param name="providerName">Nome del provider (OpenAI, Claude, DeepSeek, Gemini)</param>
    /// <returns>Istanza del provider richiesto</returns>
    public IAIProvider GetProvider(string providerName)
    {
        if (string.IsNullOrWhiteSpace(providerName))
        {
            throw new ArgumentException("Nome provider non può essere vuoto", nameof(providerName));
        }

        // Controlla cache
        if (_providers.TryGetValue(providerName, out var cachedProvider))
        {
            return cachedProvider;
        }

        // Crea nuovo provider
        var provider = CreateProvider(providerName);

        // Aggiungi a cache
        _providers[providerName] = provider;

        return provider;
    }

    /// <summary>
    /// Ottieni il provider predefinito dal configuration
    /// </summary>
    public IAIProvider GetDefaultProvider()
    {
        var defaultProvider = _configuration["AIProviders:Default"] ?? "OpenAI";
        return GetProvider(defaultProvider);
    }

    /// <summary>
    /// Ottieni il provider per un assistente specifico
    /// </summary>
    /// <param name="codiceAssistente">Codice dell'assistente (HUB, AGENDA, etc.)</param>
    /// <returns>Provider configurato per quell'assistente</returns>
    public IAIProvider GetProviderForAssistant(string codiceAssistente)
    {
        // Mapping assistente -> provider (configurabile)
        var assistantProviders = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "HUB", "OpenAI" },
            { "AGENDA", "Claude" },
            { "FINANZE", "OpenAI" },
            { "BANCA", "Claude" },
            { "ASSICURAZIONI", "OpenAI" },
            { "CUCINA", "Gemini" },
            { "SALUTE", "Claude" },
            { "DOCUMENTI", "OpenAI" },
            { "EMAIL", "OpenAI" },
            { "CASA", "Gemini" },
            { "VIAGGI", "Claude" },
            { "COMPITI", "DeepSeek" }
        };

        var providerName = assistantProviders.GetValueOrDefault(codiceAssistente, "OpenAI");
        return GetProvider(providerName);
    }

    /// <summary>
    /// Ottieni tutti i provider disponibili e configurati
    /// </summary>
    public async Task<List<(string Name, bool IsConfigured)>> GetAvailableProvidersAsync()
    {
        var providers = new List<(string Name, bool IsConfigured)>();

        var providerNames = new[] { "OpenAI", "Claude", "DeepSeek", "Gemini" };

        foreach (var name in providerNames)
        {
            try
            {
                var provider = GetProvider(name);
                var isConfigured = await provider.IsConfiguredAsync();
                providers.Add((name, isConfigured));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[AIProviderFactory] Errore verificando provider {Provider}", name);
                providers.Add((name, false));
            }
        }

        return providers;
    }

    /// <summary>
    /// Ottieni il miglior provider disponibile per un task
    /// Controlla quali sono configurati e seleziona il migliore
    /// </summary>
    public async Task<IAIProvider> GetBestAvailableProviderAsync()
    {
        var available = await GetAvailableProvidersAsync();
        var configured = available.Where(p => p.IsConfigured).ToList();

        if (configured.Count == 0)
        {
            throw new InvalidOperationException("Nessun provider AI configurato. Configura almeno un provider in appsettings.json");
        }

        // Ordine di preferenza: Claude > OpenAI > DeepSeek > Gemini
        var preferenceOrder = new[] { "Claude", "OpenAI", "DeepSeek", "Gemini" };

        foreach (var preferred in preferenceOrder)
        {
            if (configured.Any(p => p.Name.Equals(preferred, StringComparison.OrdinalIgnoreCase)))
            {
                _logger.LogInformation("[AIProviderFactory] Selezionato provider {Provider}", preferred);
                return GetProvider(preferred);
            }
        }

        // Fallback al primo configurato
        var first = configured.First();
        _logger.LogInformation("[AIProviderFactory] Selezionato provider {Provider} (fallback)", first.Name);
        return GetProvider(first.Name);
    }

    private IAIProvider CreateProvider(string providerName)
    {
        var httpClient = _httpClientFactory.CreateClient($"AI_{providerName}");

        return providerName.ToUpperInvariant() switch
        {
            "OPENAI" => new OpenAIProvider(httpClient, _logger, _configuration),
            "CLAUDE" or "ANTHROPIC" => new ClaudeProvider(httpClient, _logger, _configuration),
            "DEEPSEEK" => new DeepSeekProvider(httpClient, _logger, _configuration),
            "GEMINI" or "GOOGLE" => new GeminiProvider(httpClient, _logger, _configuration),
            _ => throw new ArgumentException($"Provider '{providerName}' non supportato. Provider disponibili: OpenAI, Claude, DeepSeek, Gemini", nameof(providerName))
        };
    }
}
