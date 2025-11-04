using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using NugoloFamily.Core.Interfaces.Repositories;
using NugoloFamily.Core.Models.Entities;

namespace NugoloFamily.Infrastructure.Data.Repositories;

/// <summary>
/// Repository per la gestione delle Configurazioni AI tramite ADO.NET
/// </summary>
public class ConfigurazioniAIRepository : BaseRepository, IConfigurazioniAIRepository
{
    public ConfigurazioniAIRepository(IConfiguration configuration) : base(configuration)
    {
    }

    public async Task<ConfigurazioneAIEntityModel?> GetByIdAsync(int idConfigurazioneAI)
    {
        const string query = @"
            SELECT IdConfigurazioneAI, IdFamiglia, NomeConfigurazione,
                   ProviderAI, Modello, ChiaveAPI, Endpoint,
                   ParametriModello, PredefinitaPerFamiglia, AssistenteSpecifico, Attiva,
                   DataInserimento, UtenteInserimento, IPInserimento,
                   DataModifica, UtenteModifica, IPModifica
            FROM ConfigurazioniAI
            WHERE IdConfigurazioneAI = @IdConfigurazioneAI";

        var parameters = new[]
        {
            new SqlParameter("@IdConfigurazioneAI", idConfigurazioneAI)
        };

        return await ExecuteReaderSingleAsync(query, MapConfigurazioneAI, parameters);
    }

    public async Task<IEnumerable<ConfigurazioneAIEntityModel>> GetByFamigliaAsync(int idFamiglia)
    {
        const string query = @"
            SELECT IdConfigurazioneAI, IdFamiglia, NomeConfigurazione,
                   ProviderAI, Modello, ChiaveAPI, Endpoint,
                   ParametriModello, PredefinitaPerFamiglia, AssistenteSpecifico, Attiva,
                   DataInserimento, UtenteInserimento, IPInserimento,
                   DataModifica, UtenteModifica, IPModifica
            FROM ConfigurazioniAI
            WHERE IdFamiglia = @IdFamiglia
            ORDER BY PredefinitaPerFamiglia DESC, NomeConfigurazione";

        var parameters = new[]
        {
            new SqlParameter("@IdFamiglia", idFamiglia)
        };

        return await ExecuteReaderAsync(query, MapConfigurazioneAI, parameters);
    }

    public async Task<ConfigurazioneAIEntityModel?> GetPredefinitaByFamigliaAsync(int idFamiglia)
    {
        const string query = @"
            SELECT IdConfigurazioneAI, IdFamiglia, NomeConfigurazione,
                   ProviderAI, Modello, ChiaveAPI, Endpoint,
                   ParametriModello, PredefinitaPerFamiglia, AssistenteSpecifico, Attiva,
                   DataInserimento, UtenteInserimento, IPInserimento,
                   DataModifica, UtenteModifica, IPModifica
            FROM ConfigurazioniAI
            WHERE IdFamiglia = @IdFamiglia
              AND PredefinitaPerFamiglia = 1
              AND Attiva = 1";

        var parameters = new[]
        {
            new SqlParameter("@IdFamiglia", idFamiglia)
        };

        return await ExecuteReaderSingleAsync(query, MapConfigurazioneAI, parameters);
    }

    public async Task<ConfigurazioneAIEntityModel?> GetByFamigliaAndAssistenteAsync(int idFamiglia, int idAssistente)
    {
        const string query = @"
            SELECT IdConfigurazioneAI, IdFamiglia, NomeConfigurazione,
                   ProviderAI, Modello, ChiaveAPI, Endpoint,
                   ParametriModello, PredefinitaPerFamiglia, AssistenteSpecifico, Attiva,
                   DataInserimento, UtenteInserimento, IPInserimento,
                   DataModifica, UtenteModifica, IPModifica
            FROM ConfigurazioniAI
            WHERE IdFamiglia = @IdFamiglia
              AND AssistenteSpecifico = @IdAssistente
              AND Attiva = 1";

        var parameters = new[]
        {
            new SqlParameter("@IdFamiglia", idFamiglia),
            new SqlParameter("@IdAssistente", idAssistente)
        };

        return await ExecuteReaderSingleAsync(query, MapConfigurazioneAI, parameters);
    }

    public async Task<IEnumerable<ConfigurazioneAIEntityModel>> GetByProviderAsync(string providerAI)
    {
        const string query = @"
            SELECT IdConfigurazioneAI, IdFamiglia, NomeConfigurazione,
                   ProviderAI, Modello, ChiaveAPI, Endpoint,
                   ParametriModello, PredefinitaPerFamiglia, AssistenteSpecifico, Attiva,
                   DataInserimento, UtenteInserimento, IPInserimento,
                   DataModifica, UtenteModifica, IPModifica
            FROM ConfigurazioniAI
            WHERE ProviderAI = @ProviderAI
            ORDER BY IdFamiglia, NomeConfigurazione";

        var parameters = new[]
        {
            new SqlParameter("@ProviderAI", providerAI)
        };

        return await ExecuteReaderAsync(query, MapConfigurazioneAI, parameters);
    }

    public async Task<int> CreateAsync(ConfigurazioneAIEntityModel configurazioneAI)
    {
        const string query = @"
            INSERT INTO ConfigurazioniAI (IdFamiglia, NomeConfigurazione, ProviderAI,
                                          Modello, ChiaveAPI, Endpoint, ParametriModello,
                                          PredefinitaPerFamiglia, AssistenteSpecifico, Attiva,
                                          DataInserimento, UtenteInserimento, IPInserimento)
            VALUES (@IdFamiglia, @NomeConfigurazione, @ProviderAI,
                    @Modello, @ChiaveAPI, @Endpoint, @ParametriModello,
                    @PredefinitaPerFamiglia, @AssistenteSpecifico, @Attiva,
                    @DataInserimento, @UtenteInserimento, @IPInserimento);
            SELECT CAST(SCOPE_IDENTITY() AS INT);";

        var parameters = new[]
        {
            new SqlParameter("@IdFamiglia", configurazioneAI.IdFamiglia),
            new SqlParameter("@NomeConfigurazione", configurazioneAI.NomeConfigurazione),
            new SqlParameter("@ProviderAI", configurazioneAI.ProviderAI),
            new SqlParameter("@Modello", configurazioneAI.Modello),
            new SqlParameter("@ChiaveAPI", GetValueOrDBNull(configurazioneAI.ChiaveAPI)),
            new SqlParameter("@Endpoint", GetValueOrDBNull(configurazioneAI.Endpoint)),
            new SqlParameter("@ParametriModello", GetValueOrDBNull(configurazioneAI.ParametriModello)),
            new SqlParameter("@PredefinitaPerFamiglia", configurazioneAI.PredefinitaPerFamiglia),
            new SqlParameter("@AssistenteSpecifico", GetValueOrDBNull(configurazioneAI.AssistenteSpecifico)),
            new SqlParameter("@Attiva", configurazioneAI.Attiva),
            new SqlParameter("@DataInserimento", configurazioneAI.DataInserimento),
            new SqlParameter("@UtenteInserimento", configurazioneAI.UtenteInserimento),
            new SqlParameter("@IPInserimento", GetValueOrDBNull(configurazioneAI.IPInserimento))
        };

        return await ExecuteScalarAsync<int>(query, parameters);
    }

    public async Task<bool> UpdateAsync(ConfigurazioneAIEntityModel configurazioneAI)
    {
        const string query = @"
            UPDATE ConfigurazioniAI
            SET NomeConfigurazione = @NomeConfigurazione,
                ChiaveAPI = @ChiaveAPI,
                Endpoint = @Endpoint,
                ParametriModello = @ParametriModello,
                PredefinitaPerFamiglia = @PredefinitaPerFamiglia,
                Attiva = @Attiva,
                DataModifica = @DataModifica,
                UtenteModifica = @UtenteModifica,
                IPModifica = @IPModifica
            WHERE IdConfigurazioneAI = @IdConfigurazioneAI";

        var parameters = new[]
        {
            new SqlParameter("@IdConfigurazioneAI", configurazioneAI.IdConfigurazioneAI),
            new SqlParameter("@NomeConfigurazione", configurazioneAI.NomeConfigurazione),
            new SqlParameter("@ChiaveAPI", GetValueOrDBNull(configurazioneAI.ChiaveAPI)),
            new SqlParameter("@Endpoint", GetValueOrDBNull(configurazioneAI.Endpoint)),
            new SqlParameter("@ParametriModello", GetValueOrDBNull(configurazioneAI.ParametriModello)),
            new SqlParameter("@PredefinitaPerFamiglia", configurazioneAI.PredefinitaPerFamiglia),
            new SqlParameter("@Attiva", configurazioneAI.Attiva),
            new SqlParameter("@DataModifica", GetValueOrDBNull(configurazioneAI.DataModifica)),
            new SqlParameter("@UtenteModifica", GetValueOrDBNull(configurazioneAI.UtenteModifica)),
            new SqlParameter("@IPModifica", GetValueOrDBNull(configurazioneAI.IPModifica))
        };

        var rowsAffected = await ExecuteNonQueryAsync(query, parameters);
        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(int idConfigurazioneAI)
    {
        const string query = "DELETE FROM ConfigurazioniAI WHERE IdConfigurazioneAI = @IdConfigurazioneAI";

        var parameters = new[]
        {
            new SqlParameter("@IdConfigurazioneAI", idConfigurazioneAI)
        };

        var rowsAffected = await ExecuteNonQueryAsync(query, parameters);
        return rowsAffected > 0;
    }

    public async Task<bool> ImpostaPredefinitaAsync(int idFamiglia, int idConfigurazioneAI)
    {
        // Prima rimuove il flag predefinito da tutte le altre configurazioni della famiglia
        const string queryReset = @"
            UPDATE ConfigurazioniAI
            SET PredefinitaPerFamiglia = 0
            WHERE IdFamiglia = @IdFamiglia";

        var parametersReset = new[]
        {
            new SqlParameter("@IdFamiglia", idFamiglia)
        };

        await ExecuteNonQueryAsync(queryReset, parametersReset);

        // Poi imposta la nuova configurazione come predefinita
        const string querySet = @"
            UPDATE ConfigurazioniAI
            SET PredefinitaPerFamiglia = 1
            WHERE IdConfigurazioneAI = @IdConfigurazioneAI
              AND IdFamiglia = @IdFamiglia";

        var parametersSet = new[]
        {
            new SqlParameter("@IdConfigurazioneAI", idConfigurazioneAI),
            new SqlParameter("@IdFamiglia", idFamiglia)
        };

        var rowsAffected = await ExecuteNonQueryAsync(querySet, parametersSet);
        return rowsAffected > 0;
    }

    /// <summary>
    /// Mappa un SqlDataReader su un oggetto ConfigurazioneAIEntityModel
    /// </summary>
    private ConfigurazioneAIEntityModel MapConfigurazioneAI(SqlDataReader reader)
    {
        return new ConfigurazioneAIEntityModel
        {
            IdConfigurazioneAI = reader.GetInt32(reader.GetOrdinal("IdConfigurazioneAI")),
            IdFamiglia = reader.GetInt32(reader.GetOrdinal("IdFamiglia")),
            NomeConfigurazione = reader.GetString(reader.GetOrdinal("NomeConfigurazione")),
            ProviderAI = reader.GetString(reader.GetOrdinal("ProviderAI")),
            Modello = reader.GetString(reader.GetOrdinal("Modello")),
            ChiaveAPI = GetNullableString(reader, "ChiaveAPI"),
            Endpoint = GetNullableString(reader, "Endpoint"),
            ParametriModello = GetNullableString(reader, "ParametriModello"),
            PredefinitaPerFamiglia = reader.GetBoolean(reader.GetOrdinal("PredefinitaPerFamiglia")),
            AssistenteSpecifico = GetNullableValue<int>(reader, "AssistenteSpecifico"),
            Attiva = reader.GetBoolean(reader.GetOrdinal("Attiva")),
            DataInserimento = reader.GetDateTime(reader.GetOrdinal("DataInserimento")),
            UtenteInserimento = reader.GetString(reader.GetOrdinal("UtenteInserimento")),
            IPInserimento = GetNullableString(reader, "IPInserimento"),
            DataModifica = GetNullableDateTime(reader, "DataModifica"),
            UtenteModifica = GetNullableString(reader, "UtenteModifica"),
            IPModifica = GetNullableString(reader, "IPModifica")
        };
    }
}
