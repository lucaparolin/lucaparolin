using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using NugoloFamily.Core.Interfaces.Repositories;
using NugoloFamily.Core.Models.Entities;

namespace NugoloFamily.Infrastructure.Data.Repositories;

/// <summary>
/// Repository per la gestione dei Log Attività tramite ADO.NET
/// </summary>
public class LogAttivitaRepository : BaseRepository, ILogAttivitaRepository
{
    public LogAttivitaRepository(IConfiguration configuration) : base(configuration)
    {
    }

    public async Task<LogAttivitaEntityModel?> GetByIdAsync(long idLog)
    {
        const string query = @"
            SELECT IdLog, IdFamiglia, IdUtente, TipoAttivita, Descrizione,
                   EntitaCoinvolta, IdEntita, IPAddress, UserAgent,
                   Durata, Esito, DettagliErrore, DataInserimento
            FROM LogAttivita
            WHERE IdLog = @IdLog";

        var parameters = new[]
        {
            new SqlParameter("@IdLog", idLog)
        };

        return await ExecuteReaderSingleAsync(query, MapLogAttivita, parameters);
    }

    public async Task<IEnumerable<LogAttivitaEntityModel>> GetByFamigliaAsync(int idFamiglia, int limite)
    {
        const string query = @"
            SELECT TOP(@Limite)
                   IdLog, IdFamiglia, IdUtente, TipoAttivita, Descrizione,
                   EntitaCoinvolta, IdEntita, IPAddress, UserAgent,
                   Durata, Esito, DettagliErrore, DataInserimento
            FROM LogAttivita
            WHERE IdFamiglia = @IdFamiglia
            ORDER BY DataInserimento DESC";

        var parameters = new[]
        {
            new SqlParameter("@IdFamiglia", idFamiglia),
            new SqlParameter("@Limite", limite)
        };

        return await ExecuteReaderAsync(query, MapLogAttivita, parameters);
    }

    public async Task<IEnumerable<LogAttivitaEntityModel>> GetByUtenteAsync(int idUtente, int limite)
    {
        const string query = @"
            SELECT TOP(@Limite)
                   IdLog, IdFamiglia, IdUtente, TipoAttivita, Descrizione,
                   EntitaCoinvolta, IdEntita, IPAddress, UserAgent,
                   Durata, Esito, DettagliErrore, DataInserimento
            FROM LogAttivita
            WHERE IdUtente = @IdUtente
            ORDER BY DataInserimento DESC";

        var parameters = new[]
        {
            new SqlParameter("@IdUtente", idUtente),
            new SqlParameter("@Limite", limite)
        };

        return await ExecuteReaderAsync(query, MapLogAttivita, parameters);
    }

    public async Task<IEnumerable<LogAttivitaEntityModel>> GetByTipoAttivitaAsync(string tipoAttivita, int limite)
    {
        const string query = @"
            SELECT TOP(@Limite)
                   IdLog, IdFamiglia, IdUtente, TipoAttivita, Descrizione,
                   EntitaCoinvolta, IdEntita, IPAddress, UserAgent,
                   Durata, Esito, DettagliErrore, DataInserimento
            FROM LogAttivita
            WHERE TipoAttivita = @TipoAttivita
            ORDER BY DataInserimento DESC";

        var parameters = new[]
        {
            new SqlParameter("@TipoAttivita", tipoAttivita),
            new SqlParameter("@Limite", limite)
        };

        return await ExecuteReaderAsync(query, MapLogAttivita, parameters);
    }

    public async Task<IEnumerable<LogAttivitaEntityModel>> GetByPeriodoAsync(DateTime dataInizio, DateTime dataFine, int limite)
    {
        const string query = @"
            SELECT TOP(@Limite)
                   IdLog, IdFamiglia, IdUtente, TipoAttivita, Descrizione,
                   EntitaCoinvolta, IdEntita, IPAddress, UserAgent,
                   Durata, Esito, DettagliErrore, DataInserimento
            FROM LogAttivita
            WHERE DataInserimento BETWEEN @DataInizio AND @DataFine
            ORDER BY DataInserimento DESC";

        var parameters = new[]
        {
            new SqlParameter("@DataInizio", dataInizio),
            new SqlParameter("@DataFine", dataFine),
            new SqlParameter("@Limite", limite)
        };

        return await ExecuteReaderAsync(query, MapLogAttivita, parameters);
    }

    public async Task<long> CreateAsync(LogAttivitaEntityModel logAttivita)
    {
        const string query = @"
            INSERT INTO LogAttivita (IdFamiglia, IdUtente, TipoAttivita, Descrizione,
                                     EntitaCoinvolta, IdEntita, IPAddress, UserAgent,
                                     Durata, Esito, DettagliErrore, DataInserimento)
            VALUES (@IdFamiglia, @IdUtente, @TipoAttivita, @Descrizione,
                    @EntitaCoinvolta, @IdEntita, @IPAddress, @UserAgent,
                    @Durata, @Esito, @DettagliErrore, @DataInserimento);
            SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

        var parameters = new[]
        {
            new SqlParameter("@IdFamiglia", GetValueOrDBNull(logAttivita.IdFamiglia)),
            new SqlParameter("@IdUtente", GetValueOrDBNull(logAttivita.IdUtente)),
            new SqlParameter("@TipoAttivita", logAttivita.TipoAttivita),
            new SqlParameter("@Descrizione", GetValueOrDBNull(logAttivita.Descrizione)),
            new SqlParameter("@EntitaCoinvolta", GetValueOrDBNull(logAttivita.EntitaCoinvolta)),
            new SqlParameter("@IdEntita", GetValueOrDBNull(logAttivita.IdEntita)),
            new SqlParameter("@IPAddress", GetValueOrDBNull(logAttivita.IPAddress)),
            new SqlParameter("@UserAgent", GetValueOrDBNull(logAttivita.UserAgent)),
            new SqlParameter("@Durata", GetValueOrDBNull(logAttivita.Durata)),
            new SqlParameter("@Esito", GetValueOrDBNull(logAttivita.Esito)),
            new SqlParameter("@DettagliErrore", GetValueOrDBNull(logAttivita.DettagliErrore)),
            new SqlParameter("@DataInserimento", logAttivita.DataInserimento)
        };

        return await ExecuteScalarAsync<long>(query, parameters);
    }

    public async Task<bool> DeleteOlderThanAsync(DateTime dataLimite)
    {
        const string query = "DELETE FROM LogAttivita WHERE DataInserimento < @DataLimite";

        var parameters = new[]
        {
            new SqlParameter("@DataLimite", dataLimite)
        };

        var rowsAffected = await ExecuteNonQueryAsync(query, parameters);
        return rowsAffected > 0;
    }

    /// <summary>
    /// Mappa un SqlDataReader su un oggetto LogAttivitaEntityModel
    /// </summary>
    private LogAttivitaEntityModel MapLogAttivita(SqlDataReader reader)
    {
        return new LogAttivitaEntityModel
        {
            IdLog = reader.GetInt64(reader.GetOrdinal("IdLog")),
            IdFamiglia = GetNullableValue<int>(reader, "IdFamiglia"),
            IdUtente = GetNullableValue<int>(reader, "IdUtente"),
            TipoAttivita = reader.GetString(reader.GetOrdinal("TipoAttivita")),
            Descrizione = GetNullableString(reader, "Descrizione"),
            EntitaCoinvolta = GetNullableString(reader, "EntitaCoinvolta"),
            IdEntita = GetNullableValue<int>(reader, "IdEntita"),
            IPAddress = GetNullableString(reader, "IPAddress"),
            UserAgent = GetNullableString(reader, "UserAgent"),
            Durata = GetNullableValue<int>(reader, "Durata"),
            Esito = GetNullableString(reader, "Esito"),
            DettagliErrore = GetNullableString(reader, "DettagliErrore"),
            DataInserimento = reader.GetDateTime(reader.GetOrdinal("DataInserimento"))
        };
    }
}
