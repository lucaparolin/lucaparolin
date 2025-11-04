using System.Security.Claims;

namespace NugoloFamily.API.Middleware;

/// <summary>
/// Middleware per gestire il multi-tenancy
/// Verifica che l'utente possa accedere solo ai dati della propria famiglia
/// </summary>
public class MultiTenantMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<MultiTenantMiddleware> _logger;

    public MultiTenantMiddleware(
        RequestDelegate next,
        ILogger<MultiTenantMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Verifica solo per richieste autenticate
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var familyId = context.User.FindFirst("IdFamiglia")?.Value;

            if (!string.IsNullOrEmpty(familyId))
            {
                // Salva l'ID famiglia nel contesto per uso successivo
                context.Items["TenantId"] = familyId;

                _logger.LogDebug("Richiesta autenticata per famiglia {FamilyId}", familyId);

                // TODO: Implementare validazione route parameters
                // Esempio: Se la route contiene "famiglia/{idFamiglia}",
                // verificare che corrisponda all'ID famiglia dell'utente
                // a meno che l'utente non sia un amministratore di sistema
            }
            else
            {
                _logger.LogWarning("Utente autenticato senza ID famiglia associato");
            }
        }

        await _next(context);
    }
}
