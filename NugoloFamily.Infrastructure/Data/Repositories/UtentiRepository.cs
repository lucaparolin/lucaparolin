using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using NugoloFamily.Core.Interfaces.Repositories;
using NugoloFamily.Core.Models.Entities;

namespace NugoloFamily.Infrastructure.Data.Repositories;

/// <summary>
/// Repository per la gestione degli Utenti tramite ADO.NET
/// </summary>
public class UtentiRepository : BaseRepository, IUtentiRepository
{
    public UtentiRepository(IConfiguration configuration) : base(configuration)
    {
    }

    public async Task<UtenteEntityModel?> GetByIdAsync(int idUtente)
    {
        const string query = @"
            SELECT IdUtente, IdFamiglia, Username, Email, PasswordHash, Salt,
                   Nome, Cognome, DataNascita, Ruolo, Attivo, UltimoAccesso,
                   TokenRefresh, DataScadenzaToken,
                   DataInserimento, UtenteInserimento, IPInserimento,
                   DataModifica, UtenteModifica, IPModifica
            FROM Utenti
            WHERE IdUtente = @IdUtente";

        var parameters = new[]
        {
            new SqlParameter("@IdUtente", idUtente)
        };

        return await ExecuteReaderSingleAsync(query, MapUtente, parameters);
    }

    public async Task<UtenteEntityModel?> GetByUsernameAsync(string username)
    {
        const string query = @"
            SELECT IdUtente, IdFamiglia, Username, Email, PasswordHash, Salt,
                   Nome, Cognome, DataNascita, Ruolo, Attivo, UltimoAccesso,
                   TokenRefresh, DataScadenzaToken,
                   DataInserimento, UtenteInserimento, IPInserimento,
                   DataModifica, UtenteModifica, IPModifica
            FROM Utenti
            WHERE Username = @Username";

        var parameters = new[]
        {
            new SqlParameter("@Username", username)
        };

        return await ExecuteReaderSingleAsync(query, MapUtente, parameters);
    }

    public async Task<UtenteEntityModel?> GetByEmailAsync(string email)
    {
        const string query = @"
            SELECT IdUtente, IdFamiglia, Username, Email, PasswordHash, Salt,
                   Nome, Cognome, DataNascita, Ruolo, Attivo, UltimoAccesso,
                   TokenRefresh, DataScadenzaToken,
                   DataInserimento, UtenteInserimento, IPInserimento,
                   DataModifica, UtenteModifica, IPModifica
            FROM Utenti
            WHERE Email = @Email";

        var parameters = new[]
        {
            new SqlParameter("@Email", email)
        };

        return await ExecuteReaderSingleAsync(query, MapUtente, parameters);
    }

    public async Task<IEnumerable<UtenteEntityModel>> GetByFamigliaAsync(int idFamiglia)
    {
        const string query = @"
            SELECT IdUtente, IdFamiglia, Username, Email, PasswordHash, Salt,
                   Nome, Cognome, DataNascita, Ruolo, Attivo, UltimoAccesso,
                   TokenRefresh, DataScadenzaToken,
                   DataInserimento, UtenteInserimento, IPInserimento,
                   DataModifica, UtenteModifica, IPModifica
            FROM Utenti
            WHERE IdFamiglia = @IdFamiglia
            ORDER BY Nome, Cognome";

        var parameters = new[]
        {
            new SqlParameter("@IdFamiglia", idFamiglia)
        };

        return await ExecuteReaderAsync(query, MapUtente, parameters);
    }

    public async Task<IEnumerable<UtenteEntityModel>> GetAttiviByFamigliaAsync(int idFamiglia)
    {
        const string query = @"
            SELECT IdUtente, IdFamiglia, Username, Email, PasswordHash, Salt,
                   Nome, Cognome, DataNascita, Ruolo, Attivo, UltimoAccesso,
                   TokenRefresh, DataScadenzaToken,
                   DataInserimento, UtenteInserimento, IPInserimento,
                   DataModifica, UtenteModifica, IPModifica
            FROM Utenti
            WHERE IdFamiglia = @IdFamiglia AND Attivo = 1
            ORDER BY Nome, Cognome";

        var parameters = new[]
        {
            new SqlParameter("@IdFamiglia", idFamiglia)
        };

        return await ExecuteReaderAsync(query, MapUtente, parameters);
    }

    public async Task<int> CreateAsync(UtenteEntityModel utente)
    {
        const string query = @"
            INSERT INTO Utenti (IdFamiglia, Username, Email, PasswordHash, Salt,
                                Nome, Cognome, DataNascita, Ruolo, Attivo,
                                DataInserimento, UtenteInserimento, IPInserimento)
            VALUES (@IdFamiglia, @Username, @Email, @PasswordHash, @Salt,
                    @Nome, @Cognome, @DataNascita, @Ruolo, @Attivo,
                    @DataInserimento, @UtenteInserimento, @IPInserimento);
            SELECT CAST(SCOPE_IDENTITY() AS INT);";

        var parameters = new[]
        {
            new SqlParameter("@IdFamiglia", utente.IdFamiglia),
            new SqlParameter("@Username", utente.Username),
            new SqlParameter("@Email", utente.Email),
            new SqlParameter("@PasswordHash", utente.PasswordHash),
            new SqlParameter("@Salt", utente.Salt),
            new SqlParameter("@Nome", utente.Nome),
            new SqlParameter("@Cognome", utente.Cognome),
            new SqlParameter("@DataNascita", GetValueOrDBNull(utente.DataNascita)),
            new SqlParameter("@Ruolo", utente.Ruolo),
            new SqlParameter("@Attivo", utente.Attivo),
            new SqlParameter("@DataInserimento", utente.DataInserimento),
            new SqlParameter("@UtenteInserimento", utente.UtenteInserimento),
            new SqlParameter("@IPInserimento", GetValueOrDBNull(utente.IPInserimento))
        };

        return await ExecuteScalarAsync<int>(query, parameters);
    }

    public async Task<bool> UpdateAsync(UtenteEntityModel utente)
    {
        const string query = @"
            UPDATE Utenti
            SET Email = @Email,
                Nome = @Nome,
                Cognome = @Cognome,
                DataNascita = @DataNascita,
                Ruolo = @Ruolo,
                Attivo = @Attivo,
                DataModifica = @DataModifica,
                UtenteModifica = @UtenteModifica,
                IPModifica = @IPModifica
            WHERE IdUtente = @IdUtente";

        var parameters = new[]
        {
            new SqlParameter("@IdUtente", utente.IdUtente),
            new SqlParameter("@Email", utente.Email),
            new SqlParameter("@Nome", utente.Nome),
            new SqlParameter("@Cognome", utente.Cognome),
            new SqlParameter("@DataNascita", GetValueOrDBNull(utente.DataNascita)),
            new SqlParameter("@Ruolo", utente.Ruolo),
            new SqlParameter("@Attivo", utente.Attivo),
            new SqlParameter("@DataModifica", GetValueOrDBNull(utente.DataModifica)),
            new SqlParameter("@UtenteModifica", GetValueOrDBNull(utente.UtenteModifica)),
            new SqlParameter("@IPModifica", GetValueOrDBNull(utente.IPModifica))
        };

        var rowsAffected = await ExecuteNonQueryAsync(query, parameters);
        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(int idUtente)
    {
        const string query = "DELETE FROM Utenti WHERE IdUtente = @IdUtente";

        var parameters = new[]
        {
            new SqlParameter("@IdUtente", idUtente)
        };

        var rowsAffected = await ExecuteNonQueryAsync(query, parameters);
        return rowsAffected > 0;
    }

    public async Task<bool> UpdateUltimoAccessoAsync(int idUtente)
    {
        const string query = @"
            UPDATE Utenti
            SET UltimoAccesso = GETDATE()
            WHERE IdUtente = @IdUtente";

        var parameters = new[]
        {
            new SqlParameter("@IdUtente", idUtente)
        };

        var rowsAffected = await ExecuteNonQueryAsync(query, parameters);
        return rowsAffected > 0;
    }

    public async Task<bool> UpdateTokenRefreshAsync(int idUtente, string tokenRefresh, DateTime dataScadenza)
    {
        const string query = @"
            UPDATE Utenti
            SET TokenRefresh = @TokenRefresh,
                DataScadenzaToken = @DataScadenza
            WHERE IdUtente = @IdUtente";

        var parameters = new[]
        {
            new SqlParameter("@IdUtente", idUtente),
            new SqlParameter("@TokenRefresh", tokenRefresh),
            new SqlParameter("@DataScadenza", dataScadenza)
        };

        var rowsAffected = await ExecuteNonQueryAsync(query, parameters);
        return rowsAffected > 0;
    }

    public async Task<bool> ExistsByUsernameAsync(string username)
    {
        const string query = "SELECT COUNT(1) FROM Utenti WHERE Username = @Username";

        var parameters = new[]
        {
            new SqlParameter("@Username", username)
        };

        var count = await ExecuteScalarAsync<int>(query, parameters);
        return count > 0;
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        const string query = "SELECT COUNT(1) FROM Utenti WHERE Email = @Email";

        var parameters = new[]
        {
            new SqlParameter("@Email", email)
        };

        var count = await ExecuteScalarAsync<int>(query, parameters);
        return count > 0;
    }

    public async Task<int> CountByFamigliaAsync(int idFamiglia)
    {
        const string query = "SELECT COUNT(1) FROM Utenti WHERE IdFamiglia = @IdFamiglia AND Attivo = 1";

        var parameters = new[]
        {
            new SqlParameter("@IdFamiglia", idFamiglia)
        };

        return await ExecuteScalarAsync<int>(query, parameters);
    }

    /// <summary>
    /// Mappa un SqlDataReader su un oggetto UtenteEntityModel
    /// </summary>
    private UtenteEntityModel MapUtente(SqlDataReader reader)
    {
        return new UtenteEntityModel
        {
            IdUtente = reader.GetInt32(reader.GetOrdinal("IdUtente")),
            IdFamiglia = reader.GetInt32(reader.GetOrdinal("IdFamiglia")),
            Username = reader.GetString(reader.GetOrdinal("Username")),
            Email = reader.GetString(reader.GetOrdinal("Email")),
            PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash")),
            Salt = reader.GetString(reader.GetOrdinal("Salt")),
            Nome = reader.GetString(reader.GetOrdinal("Nome")),
            Cognome = reader.GetString(reader.GetOrdinal("Cognome")),
            DataNascita = GetNullableDateTime(reader, "DataNascita"),
            Ruolo = reader.GetString(reader.GetOrdinal("Ruolo")),
            Attivo = reader.GetBoolean(reader.GetOrdinal("Attivo")),
            UltimoAccesso = GetNullableDateTime(reader, "UltimoAccesso"),
            TokenRefresh = GetNullableString(reader, "TokenRefresh"),
            DataScadenzaToken = GetNullableDateTime(reader, "DataScadenzaToken"),
            DataInserimento = reader.GetDateTime(reader.GetOrdinal("DataInserimento")),
            UtenteInserimento = reader.GetString(reader.GetOrdinal("UtenteInserimento")),
            IPInserimento = GetNullableString(reader, "IPInserimento"),
            DataModifica = GetNullableDateTime(reader, "DataModifica"),
            UtenteModifica = GetNullableString(reader, "UtenteModifica"),
            IPModifica = GetNullableString(reader, "IPModifica")
        };
    }
}
