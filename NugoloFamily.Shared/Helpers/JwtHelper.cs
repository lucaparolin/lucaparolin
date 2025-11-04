using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace NugoloFamily.Shared.Helpers;

/// <summary>
/// Helper per la gestione dei JWT Token
/// </summary>
public class JwtHelper
{
    private readonly string _secret;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _expirationMinutes;

    public JwtHelper(string secret, string issuer, string audience, int expirationMinutes = 60)
    {
        if (string.IsNullOrEmpty(secret) || secret.Length < 32)
            throw new ArgumentException("JWT secret deve essere almeno 32 caratteri", nameof(secret));

        _secret = secret;
        _issuer = issuer;
        _audience = audience;
        _expirationMinutes = expirationMinutes;
    }

    /// <summary>
    /// Genera un JWT token per un utente
    /// </summary>
    public string GenerateToken(int idUtente, int idFamiglia, string username, string ruolo)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_secret);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, idUtente.ToString()),
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, ruolo),
            new Claim("IdFamiglia", idFamiglia.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_expirationMinutes),
            Issuer = _issuer,
            Audience = _audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    /// <summary>
    /// Genera un refresh token casuale
    /// </summary>
    public string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomBytes);
        }
        return Convert.ToBase64String(randomBytes);
    }

    /// <summary>
    /// Valida un JWT token
    /// </summary>
    public ClaimsPrincipal? ValidateToken(string token)
    {
        if (string.IsNullOrEmpty(token))
            return null;

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_secret);

        try
        {
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _issuer,
                ValidateAudience = true,
                ValidAudience = _audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
            return principal;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Estrae l'ID utente da un token
    /// </summary>
    public int? GetUserIdFromToken(string token)
    {
        var principal = ValidateToken(token);
        if (principal == null)
            return null;

        var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            return userId;

        return null;
    }

    /// <summary>
    /// Estrae l'ID famiglia da un token
    /// </summary>
    public int? GetFamilyIdFromToken(string token)
    {
        var principal = ValidateToken(token);
        if (principal == null)
            return null;

        var familyIdClaim = principal.FindFirst("IdFamiglia");
        if (familyIdClaim != null && int.TryParse(familyIdClaim.Value, out int familyId))
            return familyId;

        return null;
    }

    /// <summary>
    /// Verifica se un token è scaduto
    /// </summary>
    public bool IsTokenExpired(string token)
    {
        if (string.IsNullOrEmpty(token))
            return true;

        var tokenHandler = new JwtSecurityTokenHandler();

        try
        {
            var jwtToken = tokenHandler.ReadJwtToken(token);
            return jwtToken.ValidTo < DateTime.UtcNow;
        }
        catch
        {
            return true;
        }
    }
}
