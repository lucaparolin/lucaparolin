using System.Security.Claims;
using NugoloFamily.Shared.Helpers;

namespace NugoloFamily.API.Middleware;

/// <summary>
/// Middleware per l'autenticazione JWT
/// Estrae e valida il token JWT dall'header Authorization
/// </summary>
public class JwtAuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<JwtAuthenticationMiddleware> _logger;

    public JwtAuthenticationMiddleware(
        RequestDelegate next,
        ILogger<JwtAuthenticationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, JwtHelper jwtHelper)
    {
        var token = ExtractTokenFromHeader(context);

        if (!string.IsNullOrEmpty(token))
        {
            try
            {
                var principal = jwtHelper.ValidateToken(token);

                if (principal != null)
                {
                    context.User = principal;

                    // Estrai informazioni utente e famiglia per logging
                    var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    var familyId = principal.FindFirst("IdFamiglia")?.Value;

                    context.Items["UserId"] = userId;
                    context.Items["FamilyId"] = familyId;

                    _logger.LogDebug("Token JWT validato per utente {UserId}, famiglia {FamilyId}", userId, familyId);
                }
                else
                {
                    _logger.LogWarning("Token JWT non valido");
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Errore durante la validazione del token JWT");
            }
        }

        await _next(context);
    }

    private string? ExtractTokenFromHeader(HttpContext context)
    {
        var authorization = context.Request.Headers["Authorization"].FirstOrDefault();

        if (string.IsNullOrEmpty(authorization))
            return null;

        // Formato: "Bearer {token}"
        if (authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            return authorization.Substring("Bearer ".Length).Trim();
        }

        return null;
    }
}
