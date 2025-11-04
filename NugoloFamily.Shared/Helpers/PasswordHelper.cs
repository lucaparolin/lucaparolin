using System.Security.Cryptography;
using System.Text;

namespace NugoloFamily.Shared.Helpers;

/// <summary>
/// Helper per la gestione sicura delle password con PBKDF2
/// </summary>
public static class PasswordHelper
{
    private const int SaltSize = 32; // 256 bit
    private const int HashSize = 32; // 256 bit
    private const int Iterations = 100000; // Numero di iterazioni PBKDF2

    /// <summary>
    /// Genera un salt casuale
    /// </summary>
    public static string GenerateSalt()
    {
        var saltBytes = new byte[SaltSize];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(saltBytes);
        }
        return Convert.ToBase64String(saltBytes);
    }

    /// <summary>
    /// Genera l'hash della password con PBKDF2
    /// </summary>
    public static string HashPassword(string password, string salt)
    {
        if (string.IsNullOrEmpty(password))
            throw new ArgumentNullException(nameof(password));

        if (string.IsNullOrEmpty(salt))
            throw new ArgumentNullException(nameof(salt));

        var saltBytes = Convert.FromBase64String(salt);

        using (var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, Iterations, HashAlgorithmName.SHA256))
        {
            var hashBytes = pbkdf2.GetBytes(HashSize);
            return Convert.ToBase64String(hashBytes);
        }
    }

    /// <summary>
    /// Verifica se una password corrisponde all'hash
    /// </summary>
    public static bool VerifyPassword(string password, string salt, string hash)
    {
        if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(salt) || string.IsNullOrEmpty(hash))
            return false;

        var computedHash = HashPassword(password, salt);
        return hash == computedHash;
    }

    /// <summary>
    /// Valida la forza di una password
    /// </summary>
    public static bool IsPasswordStrong(string password)
    {
        if (string.IsNullOrEmpty(password) || password.Length < 8)
            return false;

        bool hasUpper = false;
        bool hasLower = false;
        bool hasDigit = false;
        bool hasSpecial = false;

        foreach (char c in password)
        {
            if (char.IsUpper(c)) hasUpper = true;
            else if (char.IsLower(c)) hasLower = true;
            else if (char.IsDigit(c)) hasDigit = true;
            else if (!char.IsLetterOrDigit(c)) hasSpecial = true;
        }

        return hasUpper && hasLower && hasDigit && hasSpecial;
    }

    /// <summary>
    /// Genera una password casuale sicura
    /// </summary>
    public static string GenerateRandomPassword(int length = 16)
    {
        const string upperChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string lowerChars = "abcdefghijklmnopqrstuvwxyz";
        const string digitChars = "0123456789";
        const string specialChars = "!@#$%^&*()_-+=<>?";
        const string allChars = upperChars + lowerChars + digitChars + specialChars;

        var password = new StringBuilder();
        using (var rng = RandomNumberGenerator.Create())
        {
            // Garantisce almeno un carattere di ogni tipo
            password.Append(GetRandomChar(upperChars, rng));
            password.Append(GetRandomChar(lowerChars, rng));
            password.Append(GetRandomChar(digitChars, rng));
            password.Append(GetRandomChar(specialChars, rng));

            // Riempie il resto con caratteri casuali
            for (int i = 4; i < length; i++)
            {
                password.Append(GetRandomChar(allChars, rng));
            }
        }

        // Mescola i caratteri
        return new string(password.ToString().OrderBy(x => Guid.NewGuid()).ToArray());
    }

    private static char GetRandomChar(string chars, RandomNumberGenerator rng)
    {
        var randomBytes = new byte[4];
        rng.GetBytes(randomBytes);
        var randomValue = BitConverter.ToUInt32(randomBytes, 0);
        return chars[(int)(randomValue % (uint)chars.Length)];
    }
}
