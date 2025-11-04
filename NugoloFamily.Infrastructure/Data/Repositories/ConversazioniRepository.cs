using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using NugoloFamily.Core.Interfaces.Repositories;
using NugoloFamily.Core.Models.Entities;

namespace NugoloFamily.Infrastructure.Data.Repositories;

/// <summary>
/// Repository per la gestione delle Conversazioni tramite ADO.NET
/// </summary>
public class ConversazioniRepository : BaseRepository, IConversazioniRepository
{
    public ConversazioniRepository(IConfiguration configuration) : base(configuration)
    {
    }

    public async Task<ConversazioneEntityModel?> GetByIdAsync(int idConversazione)
    {
        const string query = @"
            SELECT IdConversazione, IdFamiglia, IdUtente, IdAssistente,
                   TitoloConversazione, StatoConversazione, UltimoMessaggio,
                   ContatoreMessaggi, MetadatiConversazione,
                   DataInserimento, UtenteInserimento, IPInserimento,
                   DataModifica, UtenteModifica, IPModifica
            FROM Conversazioni
            WHERE IdConversazione = @IdConversazione";

        var parameters = new[]
        {
            new SqlParameter("@IdConversazione", idConversazione)
        };

        return await ExecuteReaderSingleAsync(query, MapConversazione, parameters);
    }

    public async Task<IEnumerable<ConversazioneEntityModel>> GetByFamigliaAsync(int idFamiglia)
    {
        const string query = @"
            SELECT IdConversazione, IdFamiglia, IdUtente, IdAssistente,
                   TitoloConversazione, StatoConversazione, UltimoMessaggio,
                   ContatoreMessaggi, MetadatiConversazione,
                   DataInserimento, UtenteInserimento, IPInserimento,
                   DataModifica, UtenteModifica, IPModifica
            FROM Conversazioni
            WHERE IdFamiglia = @IdFamiglia
            ORDER BY UltimoMessaggio DESC, DataInserimento DESC";

        var parameters = new[]
        {
            new SqlParameter("@IdFamiglia", idFamiglia)
        };

        return await ExecuteReaderAsync(query, MapConversazione, parameters);
    }

    public async Task<IEnumerable<ConversazioneEntityModel>> GetByUtenteAsync(int idUtente)
    {
        const string query = @"
            SELECT IdConversazione, IdFamiglia, IdUtente, IdAssistente,
                   TitoloConversazione, StatoConversazione, UltimoMessaggio,
                   ContatoreMessaggi, MetadatiConversazione,
                   DataInserimento, UtenteInserimento, IPInserimento,
                   DataModifica, UtenteModifica, IPModifica
            FROM Conversazioni
            WHERE IdUtente = @IdUtente
            ORDER BY UltimoMessaggio DESC, DataInserimento DESC";

        var parameters = new[]
        {
            new SqlParameter("@IdUtente", idUtente)
        };

        return await ExecuteReaderAsync(query, MapConversazione, parameters);
    }

    public async Task<IEnumerable<ConversazioneEntityModel>> GetByAssistenteAsync(int idAssistente)
    {
        const string query = @"
            SELECT IdConversazione, IdFamiglia, IdUtente, IdAssistente,
                   TitoloConversazione, StatoConversazione, UltimoMessaggio,
                   ContatoreMessaggi, MetadatiConversazione,
                   DataInserimento, UtenteInserimento, IPInserimento,
                   DataModifica, UtenteModifica, IPModifica
            FROM Conversazioni
            WHERE IdAssistente = @IdAssistente
            ORDER BY UltimoMessaggio DESC, DataInserimento DESC";

        var parameters = new[]
        {
            new SqlParameter("@IdAssistente", idAssistente)
        };

        return await ExecuteReaderAsync(query, MapConversazione, parameters);
    }

    public async Task<IEnumerable<ConversazioneEntityModel>> GetByUtenteAndAssistenteAsync(int idUtente, int idAssistente)
    {
        const string query = @"
            SELECT IdConversazione, IdFamiglia, IdUtente, IdAssistente,
                   TitoloConversazione, StatoConversazione, UltimoMessaggio,
                   ContatoreMessaggi, MetadatiConversazione,
                   DataInserimento, UtenteInserimento, IPInserimento,
                   DataModifica, UtenteModifica, IPModifica
            FROM Conversazioni
            WHERE IdUtente = @IdUtente AND IdAssistente = @IdAssistente
            ORDER BY UltimoMessaggio DESC, DataInserimento DESC";

        var parameters = new[]
        {
            new SqlParameter("@IdUtente", idUtente),
            new SqlParameter("@IdAssistente", idAssistente)
        };

        return await ExecuteReaderAsync(query, MapConversazione, parameters);
    }

    public async Task<int> CreateAsync(ConversazioneEntityModel conversazione)
    {
        const string query = @"
            INSERT INTO Conversazioni (IdFamiglia, IdUtente, IdAssistente,
                                       TitoloConversazione, StatoConversazione,
                                       UltimoMessaggio, ContatoreMessaggi,
                                       MetadatiConversazione,
                                       DataInserimento, UtenteInserimento, IPInserimento)
            VALUES (@IdFamiglia, @IdUtente, @IdAssistente,
                    @TitoloConversazione, @StatoConversazione,
                    @UltimoMessaggio, @ContatoreMessaggi,
                    @MetadatiConversazione,
                    @DataInserimento, @UtenteInserimento, @IPInserimento);
            SELECT CAST(SCOPE_IDENTITY() AS INT);";

        var parameters = new[]
        {
            new SqlParameter("@IdFamiglia", conversazione.IdFamiglia),
            new SqlParameter("@IdUtente", conversazione.IdUtente),
            new SqlParameter("@IdAssistente", conversazione.IdAssistente),
            new SqlParameter("@TitoloConversazione", GetValueOrDBNull(conversazione.TitoloConversazione)),
            new SqlParameter("@StatoConversazione", conversazione.StatoConversazione),
            new SqlParameter("@UltimoMessaggio", GetValueOrDBNull(conversazione.UltimoMessaggio)),
            new SqlParameter("@ContatoreMessaggi", conversazione.ContatoreMessaggi),
            new SqlParameter("@MetadatiConversazione", GetValueOrDBNull(conversazione.MetadatiConversazione)),
            new SqlParameter("@DataInserimento", conversazione.DataInserimento),
            new SqlParameter("@UtenteInserimento", conversazione.UtenteInserimento),
            new SqlParameter("@IPInserimento", GetValueOrDBNull(conversazione.IPInserimento))
        };

        return await ExecuteScalarAsync<int>(query, parameters);
    }

    public async Task<bool> UpdateAsync(ConversazioneEntityModel conversazione)
    {
        const string query = @"
            UPDATE Conversazioni
            SET TitoloConversazione = @TitoloConversazione,
                StatoConversazione = @StatoConversazione,
                UltimoMessaggio = @UltimoMessaggio,
                ContatoreMessaggi = @ContatoreMessaggi,
                MetadatiConversazione = @MetadatiConversazione,
                DataModifica = @DataModifica,
                UtenteModifica = @UtenteModifica,
                IPModifica = @IPModifica
            WHERE IdConversazione = @IdConversazione";

        var parameters = new[]
        {
            new SqlParameter("@IdConversazione", conversazione.IdConversazione),
            new SqlParameter("@TitoloConversazione", GetValueOrDBNull(conversazione.TitoloConversazione)),
            new SqlParameter("@StatoConversazione", conversazione.StatoConversazione),
            new SqlParameter("@UltimoMessaggio", GetValueOrDBNull(conversazione.UltimoMessaggio)),
            new SqlParameter("@ContatoreMessaggi", conversazione.ContatoreMessaggi),
            new SqlParameter("@MetadatiConversazione", GetValueOrDBNull(conversazione.MetadatiConversazione)),
            new SqlParameter("@DataModifica", GetValueOrDBNull(conversazione.DataModifica)),
            new SqlParameter("@UtenteModifica", GetValueOrDBNull(conversazione.UtenteModifica)),
            new SqlParameter("@IPModifica", GetValueOrDBNull(conversazione.IPModifica))
        };

        var rowsAffected = await ExecuteNonQueryAsync(query, parameters);
        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(int idConversazione)
    {
        const string query = "DELETE FROM Conversazioni WHERE IdConversazione = @IdConversazione";

        var parameters = new[]
        {
            new SqlParameter("@IdConversazione", idConversazione)
        };

        var rowsAffected = await ExecuteNonQueryAsync(query, parameters);
        return rowsAffected > 0;
    }

    public async Task<bool> UpdateUltimoMessaggioAsync(int idConversazione)
    {
        const string query = @"
            UPDATE Conversazioni
            SET UltimoMessaggio = GETDATE()
            WHERE IdConversazione = @IdConversazione";

        var parameters = new[]
        {
            new SqlParameter("@IdConversazione", idConversazione)
        };

        var rowsAffected = await ExecuteNonQueryAsync(query, parameters);
        return rowsAffected > 0;
    }

    public async Task<bool> UpdateStatoAsync(int idConversazione, string stato)
    {
        const string query = @"
            UPDATE Conversazioni
            SET StatoConversazione = @Stato
            WHERE IdConversazione = @IdConversazione";

        var parameters = new[]
        {
            new SqlParameter("@IdConversazione", idConversazione),
            new SqlParameter("@Stato", stato)
        };

        var rowsAffected = await ExecuteNonQueryAsync(query, parameters);
        return rowsAffected > 0;
    }

    public async Task<bool> IncrementaContatoreMessaggiAsync(int idConversazione)
    {
        const string query = @"
            UPDATE Conversazioni
            SET ContatoreMessaggi = ContatoreMessaggi + 1,
                UltimoMessaggio = GETDATE()
            WHERE IdConversazione = @IdConversazione";

        var parameters = new[]
        {
            new SqlParameter("@IdConversazione", idConversazione)
        };

        var rowsAffected = await ExecuteNonQueryAsync(query, parameters);
        return rowsAffected > 0;
    }

    /// <summary>
    /// Mappa un SqlDataReader su un oggetto ConversazioneEntityModel
    /// </summary>
    private ConversazioneEntityModel MapConversazione(SqlDataReader reader)
    {
        return new ConversazioneEntityModel
        {
            IdConversazione = reader.GetInt32(reader.GetOrdinal("IdConversazione")),
            IdFamiglia = reader.GetInt32(reader.GetOrdinal("IdFamiglia")),
            IdUtente = reader.GetInt32(reader.GetOrdinal("IdUtente")),
            IdAssistente = reader.GetInt32(reader.GetOrdinal("IdAssistente")),
            TitoloConversazione = GetNullableString(reader, "TitoloConversazione"),
            StatoConversazione = reader.GetString(reader.GetOrdinal("StatoConversazione")),
            UltimoMessaggio = GetNullableDateTime(reader, "UltimoMessaggio"),
            ContatoreMessaggi = reader.GetInt32(reader.GetOrdinal("ContatoreMessaggi")),
            MetadatiConversazione = GetNullableString(reader, "MetadatiConversazione"),
            DataInserimento = reader.GetDateTime(reader.GetOrdinal("DataInserimento")),
            UtenteInserimento = reader.GetString(reader.GetOrdinal("UtenteInserimento")),
            IPInserimento = GetNullableString(reader, "IPInserimento"),
            DataModifica = GetNullableDateTime(reader, "DataModifica"),
            UtenteModifica = GetNullableString(reader, "UtenteModifica"),
            IPModifica = GetNullableString(reader, "IPModifica")
        };
    }
}
