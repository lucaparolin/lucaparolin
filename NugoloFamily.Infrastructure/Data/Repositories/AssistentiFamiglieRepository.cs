using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using NugoloFamily.Core.Interfaces.Repositories;
using NugoloFamily.Core.Models.Entities;

namespace NugoloFamily.Infrastructure.Data.Repositories;

/// <summary>
/// Repository per la gestione degli Assistenti attivati per le Famiglie tramite ADO.NET
/// </summary>
public class AssistentiFamiglieRepository : BaseRepository, IAssistentiFamiglieRepository
{
    public AssistentiFamiglieRepository(IConfiguration configuration) : base(configuration)
    {
    }

    public async Task<AssistenteFamigliaEntityModel?> GetByIdAsync(int idAssistenteFamiglia)
    {
        const string query = @"
            SELECT IdAssistenteFamiglia, IdFamiglia, IdAssistente, Attivo,
                   ConfigurazionePersonalizzata, DataAttivazione,
                   DataInserimento, UtenteInserimento, IPInserimento,
                   DataModifica, UtenteModifica, IPModifica
            FROM AssistentiFamiglie
            WHERE IdAssistenteFamiglia = @IdAssistenteFamiglia";

        var parameters = new[]
        {
            new SqlParameter("@IdAssistenteFamiglia", idAssistenteFamiglia)
        };

        return await ExecuteReaderSingleAsync(query, MapAssistenteFamiglia, parameters);
    }

    public async Task<IEnumerable<AssistenteFamigliaEntityModel>> GetByFamigliaAsync(int idFamiglia)
    {
        const string query = @"
            SELECT IdAssistenteFamiglia, IdFamiglia, IdAssistente, Attivo,
                   ConfigurazionePersonalizzata, DataAttivazione,
                   DataInserimento, UtenteInserimento, IPInserimento,
                   DataModifica, UtenteModifica, IPModifica
            FROM AssistentiFamiglie
            WHERE IdFamiglia = @IdFamiglia
            ORDER BY DataAttivazione DESC";

        var parameters = new[]
        {
            new SqlParameter("@IdFamiglia", idFamiglia)
        };

        return await ExecuteReaderAsync(query, MapAssistenteFamiglia, parameters);
    }

    public async Task<IEnumerable<AssistenteFamigliaEntityModel>> GetAttiviByFamigliaAsync(int idFamiglia)
    {
        const string query = @"
            SELECT IdAssistenteFamiglia, IdFamiglia, IdAssistente, Attivo,
                   ConfigurazionePersonalizzata, DataAttivazione,
                   DataInserimento, UtenteInserimento, IPInserimento,
                   DataModifica, UtenteModifica, IPModifica
            FROM AssistentiFamiglie
            WHERE IdFamiglia = @IdFamiglia AND Attivo = 1
            ORDER BY DataAttivazione DESC";

        var parameters = new[]
        {
            new SqlParameter("@IdFamiglia", idFamiglia)
        };

        return await ExecuteReaderAsync(query, MapAssistenteFamiglia, parameters);
    }

    public async Task<AssistenteFamigliaEntityModel?> GetByFamigliaAndAssistenteAsync(int idFamiglia, int idAssistente)
    {
        const string query = @"
            SELECT IdAssistenteFamiglia, IdFamiglia, IdAssistente, Attivo,
                   ConfigurazionePersonalizzata, DataAttivazione,
                   DataInserimento, UtenteInserimento, IPInserimento,
                   DataModifica, UtenteModifica, IPModifica
            FROM AssistentiFamiglie
            WHERE IdFamiglia = @IdFamiglia AND IdAssistente = @IdAssistente";

        var parameters = new[]
        {
            new SqlParameter("@IdFamiglia", idFamiglia),
            new SqlParameter("@IdAssistente", idAssistente)
        };

        return await ExecuteReaderSingleAsync(query, MapAssistenteFamiglia, parameters);
    }

    public async Task<int> CreateAsync(AssistenteFamigliaEntityModel assistenteFamiglia)
    {
        const string query = @"
            INSERT INTO AssistentiFamiglie (IdFamiglia, IdAssistente, Attivo,
                                            ConfigurazionePersonalizzata, DataAttivazione,
                                            DataInserimento, UtenteInserimento, IPInserimento)
            VALUES (@IdFamiglia, @IdAssistente, @Attivo,
                    @ConfigurazionePersonalizzata, @DataAttivazione,
                    @DataInserimento, @UtenteInserimento, @IPInserimento);
            SELECT CAST(SCOPE_IDENTITY() AS INT);";

        var parameters = new[]
        {
            new SqlParameter("@IdFamiglia", assistenteFamiglia.IdFamiglia),
            new SqlParameter("@IdAssistente", assistenteFamiglia.IdAssistente),
            new SqlParameter("@Attivo", assistenteFamiglia.Attivo),
            new SqlParameter("@ConfigurazionePersonalizzata", GetValueOrDBNull(assistenteFamiglia.ConfigurazionePersonalizzata)),
            new SqlParameter("@DataAttivazione", assistenteFamiglia.DataAttivazione),
            new SqlParameter("@DataInserimento", assistenteFamiglia.DataInserimento),
            new SqlParameter("@UtenteInserimento", assistenteFamiglia.UtenteInserimento),
            new SqlParameter("@IPInserimento", GetValueOrDBNull(assistenteFamiglia.IPInserimento))
        };

        return await ExecuteScalarAsync<int>(query, parameters);
    }

    public async Task<bool> UpdateAsync(AssistenteFamigliaEntityModel assistenteFamiglia)
    {
        const string query = @"
            UPDATE AssistentiFamiglie
            SET Attivo = @Attivo,
                ConfigurazionePersonalizzata = @ConfigurazionePersonalizzata,
                DataModifica = @DataModifica,
                UtenteModifica = @UtenteModifica,
                IPModifica = @IPModifica
            WHERE IdAssistenteFamiglia = @IdAssistenteFamiglia";

        var parameters = new[]
        {
            new SqlParameter("@IdAssistenteFamiglia", assistenteFamiglia.IdAssistenteFamiglia),
            new SqlParameter("@Attivo", assistenteFamiglia.Attivo),
            new SqlParameter("@ConfigurazionePersonalizzata", GetValueOrDBNull(assistenteFamiglia.ConfigurazionePersonalizzata)),
            new SqlParameter("@DataModifica", GetValueOrDBNull(assistenteFamiglia.DataModifica)),
            new SqlParameter("@UtenteModifica", GetValueOrDBNull(assistenteFamiglia.UtenteModifica)),
            new SqlParameter("@IPModifica", GetValueOrDBNull(assistenteFamiglia.IPModifica))
        };

        var rowsAffected = await ExecuteNonQueryAsync(query, parameters);
        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(int idAssistenteFamiglia)
    {
        const string query = "DELETE FROM AssistentiFamiglie WHERE IdAssistenteFamiglia = @IdAssistenteFamiglia";

        var parameters = new[]
        {
            new SqlParameter("@IdAssistenteFamiglia", idAssistenteFamiglia)
        };

        var rowsAffected = await ExecuteNonQueryAsync(query, parameters);
        return rowsAffected > 0;
    }

    public async Task<bool> AttivaDisattivaAsync(int idAssistenteFamiglia, bool attivo)
    {
        const string query = @"
            UPDATE AssistentiFamiglie
            SET Attivo = @Attivo
            WHERE IdAssistenteFamiglia = @IdAssistenteFamiglia";

        var parameters = new[]
        {
            new SqlParameter("@IdAssistenteFamiglia", idAssistenteFamiglia),
            new SqlParameter("@Attivo", attivo)
        };

        var rowsAffected = await ExecuteNonQueryAsync(query, parameters);
        return rowsAffected > 0;
    }

    public async Task<bool> ExistsAsync(int idFamiglia, int idAssistente)
    {
        const string query = @"
            SELECT COUNT(1)
            FROM AssistentiFamiglie
            WHERE IdFamiglia = @IdFamiglia AND IdAssistente = @IdAssistente";

        var parameters = new[]
        {
            new SqlParameter("@IdFamiglia", idFamiglia),
            new SqlParameter("@IdAssistente", idAssistente)
        };

        var count = await ExecuteScalarAsync<int>(query, parameters);
        return count > 0;
    }

    /// <summary>
    /// Mappa un SqlDataReader su un oggetto AssistenteFamigliaEntityModel
    /// </summary>
    private AssistenteFamigliaEntityModel MapAssistenteFamiglia(SqlDataReader reader)
    {
        return new AssistenteFamigliaEntityModel
        {
            IdAssistenteFamiglia = reader.GetInt32(reader.GetOrdinal("IdAssistenteFamiglia")),
            IdFamiglia = reader.GetInt32(reader.GetOrdinal("IdFamiglia")),
            IdAssistente = reader.GetInt32(reader.GetOrdinal("IdAssistente")),
            Attivo = reader.GetBoolean(reader.GetOrdinal("Attivo")),
            ConfigurazionePersonalizzata = GetNullableString(reader, "ConfigurazionePersonalizzata"),
            DataAttivazione = reader.GetDateTime(reader.GetOrdinal("DataAttivazione")),
            DataInserimento = reader.GetDateTime(reader.GetOrdinal("DataInserimento")),
            UtenteInserimento = reader.GetString(reader.GetOrdinal("UtenteInserimento")),
            IPInserimento = GetNullableString(reader, "IPInserimento"),
            DataModifica = GetNullableDateTime(reader, "DataModifica"),
            UtenteModifica = GetNullableString(reader, "UtenteModifica"),
            IPModifica = GetNullableString(reader, "IPModifica")
        };
    }
}
