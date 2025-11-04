using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using NugoloFamily.Core.Interfaces.Repositories;
using NugoloFamily.Core.Models.Entities;

namespace NugoloFamily.Infrastructure.Data.Repositories;

/// <summary>
/// Repository per la gestione dei Messaggi tramite ADO.NET
/// </summary>
public class MessaggiRepository : BaseRepository, IMessaggiRepository
{
    public MessaggiRepository(IConfiguration configuration) : base(configuration)
    {
    }

    public async Task<MessaggioEntityModel?> GetByIdAsync(long idMessaggio)
    {
        const string query = @"
            SELECT IdMessaggio, IdConversazione, IdUtente, TipoMittente,
                   TestoMessaggio, TipoContenuto, URLContenuto, DurataContenuto,
                   DimensioneFile, MimeType, MetadatiContenuto,
                   Sentiment, IntentRilevato, ConfidenzaIntent,
                   TokenUtilizzati, ModelloAIUtilizzato,
                   DataInserimento, UtenteInserimento, IPInserimento
            FROM Messaggi
            WHERE IdMessaggio = @IdMessaggio";

        var parameters = new[]
        {
            new SqlParameter("@IdMessaggio", idMessaggio)
        };

        return await ExecuteReaderSingleAsync(query, MapMessaggio, parameters);
    }

    public async Task<IEnumerable<MessaggioEntityModel>> GetByConversazioneAsync(int idConversazione)
    {
        const string query = @"
            SELECT IdMessaggio, IdConversazione, IdUtente, TipoMittente,
                   TestoMessaggio, TipoContenuto, URLContenuto, DurataContenuto,
                   DimensioneFile, MimeType, MetadatiContenuto,
                   Sentiment, IntentRilevato, ConfidenzaIntent,
                   TokenUtilizzati, ModelloAIUtilizzato,
                   DataInserimento, UtenteInserimento, IPInserimento
            FROM Messaggi
            WHERE IdConversazione = @IdConversazione
            ORDER BY DataInserimento ASC";

        var parameters = new[]
        {
            new SqlParameter("@IdConversazione", idConversazione)
        };

        return await ExecuteReaderAsync(query, MapMessaggio, parameters);
    }

    public async Task<IEnumerable<MessaggioEntityModel>> GetByConversazionePaginatiAsync(int idConversazione, int pagina, int dimensionePagina)
    {
        const string query = @"
            SELECT IdMessaggio, IdConversazione, IdUtente, TipoMittente,
                   TestoMessaggio, TipoContenuto, URLContenuto, DurataContenuto,
                   DimensioneFile, MimeType, MetadatiContenuto,
                   Sentiment, IntentRilevato, ConfidenzaIntent,
                   TokenUtilizzati, ModelloAIUtilizzato,
                   DataInserimento, UtenteInserimento, IPInserimento
            FROM Messaggi
            WHERE IdConversazione = @IdConversazione
            ORDER BY DataInserimento ASC
            OFFSET @Offset ROWS
            FETCH NEXT @PageSize ROWS ONLY";

        var offset = (pagina - 1) * dimensionePagina;

        var parameters = new[]
        {
            new SqlParameter("@IdConversazione", idConversazione),
            new SqlParameter("@Offset", offset),
            new SqlParameter("@PageSize", dimensionePagina)
        };

        return await ExecuteReaderAsync(query, MapMessaggio, parameters);
    }

    public async Task<IEnumerable<MessaggioEntityModel>> GetByUtenteAsync(int idUtente)
    {
        const string query = @"
            SELECT IdMessaggio, IdConversazione, IdUtente, TipoMittente,
                   TestoMessaggio, TipoContenuto, URLContenuto, DurataContenuto,
                   DimensioneFile, MimeType, MetadatiContenuto,
                   Sentiment, IntentRilevato, ConfidenzaIntent,
                   TokenUtilizzati, ModelloAIUtilizzato,
                   DataInserimento, UtenteInserimento, IPInserimento
            FROM Messaggi
            WHERE IdUtente = @IdUtente
            ORDER BY DataInserimento DESC";

        var parameters = new[]
        {
            new SqlParameter("@IdUtente", idUtente)
        };

        return await ExecuteReaderAsync(query, MapMessaggio, parameters);
    }

    public async Task<IEnumerable<MessaggioEntityModel>> GetByTipoContenutoAsync(string tipoContenuto)
    {
        const string query = @"
            SELECT IdMessaggio, IdConversazione, IdUtente, TipoMittente,
                   TestoMessaggio, TipoContenuto, URLContenuto, DurataContenuto,
                   DimensioneFile, MimeType, MetadatiContenuto,
                   Sentiment, IntentRilevato, ConfidenzaIntent,
                   TokenUtilizzati, ModelloAIUtilizzato,
                   DataInserimento, UtenteInserimento, IPInserimento
            FROM Messaggi
            WHERE TipoContenuto = @TipoContenuto
            ORDER BY DataInserimento DESC";

        var parameters = new[]
        {
            new SqlParameter("@TipoContenuto", tipoContenuto)
        };

        return await ExecuteReaderAsync(query, MapMessaggio, parameters);
    }

    public async Task<long> CreateAsync(MessaggioEntityModel messaggio)
    {
        const string query = @"
            INSERT INTO Messaggi (IdConversazione, IdUtente, TipoMittente,
                                  TestoMessaggio, TipoContenuto, URLContenuto,
                                  DurataContenuto, DimensioneFile, MimeType,
                                  MetadatiContenuto, Sentiment, IntentRilevato,
                                  ConfidenzaIntent, TokenUtilizzati, ModelloAIUtilizzato,
                                  DataInserimento, UtenteInserimento, IPInserimento)
            VALUES (@IdConversazione, @IdUtente, @TipoMittente,
                    @TestoMessaggio, @TipoContenuto, @URLContenuto,
                    @DurataContenuto, @DimensioneFile, @MimeType,
                    @MetadatiContenuto, @Sentiment, @IntentRilevato,
                    @ConfidenzaIntent, @TokenUtilizzati, @ModelloAIUtilizzato,
                    @DataInserimento, @UtenteInserimento, @IPInserimento);
            SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

        var parameters = new[]
        {
            new SqlParameter("@IdConversazione", messaggio.IdConversazione),
            new SqlParameter("@IdUtente", GetValueOrDBNull(messaggio.IdUtente)),
            new SqlParameter("@TipoMittente", messaggio.TipoMittente),
            new SqlParameter("@TestoMessaggio", GetValueOrDBNull(messaggio.TestoMessaggio)),
            new SqlParameter("@TipoContenuto", messaggio.TipoContenuto),
            new SqlParameter("@URLContenuto", GetValueOrDBNull(messaggio.URLContenuto)),
            new SqlParameter("@DurataContenuto", GetValueOrDBNull(messaggio.DurataContenuto)),
            new SqlParameter("@DimensioneFile", GetValueOrDBNull(messaggio.DimensioneFile)),
            new SqlParameter("@MimeType", GetValueOrDBNull(messaggio.MimeType)),
            new SqlParameter("@MetadatiContenuto", GetValueOrDBNull(messaggio.MetadatiContenuto)),
            new SqlParameter("@Sentiment", GetValueOrDBNull(messaggio.Sentiment)),
            new SqlParameter("@IntentRilevato", GetValueOrDBNull(messaggio.IntentRilevato)),
            new SqlParameter("@ConfidenzaIntent", GetValueOrDBNull(messaggio.ConfidenzaIntent)),
            new SqlParameter("@TokenUtilizzati", GetValueOrDBNull(messaggio.TokenUtilizzati)),
            new SqlParameter("@ModelloAIUtilizzato", GetValueOrDBNull(messaggio.ModelloAIUtilizzato)),
            new SqlParameter("@DataInserimento", messaggio.DataInserimento),
            new SqlParameter("@UtenteInserimento", messaggio.UtenteInserimento),
            new SqlParameter("@IPInserimento", GetValueOrDBNull(messaggio.IPInserimento))
        };

        return await ExecuteScalarAsync<long>(query, parameters);
    }

    public async Task<bool> UpdateAsync(MessaggioEntityModel messaggio)
    {
        const string query = @"
            UPDATE Messaggi
            SET TestoMessaggio = @TestoMessaggio,
                Sentiment = @Sentiment,
                IntentRilevato = @IntentRilevato,
                ConfidenzaIntent = @ConfidenzaIntent,
                TokenUtilizzati = @TokenUtilizzati,
                ModelloAIUtilizzato = @ModelloAIUtilizzato
            WHERE IdMessaggio = @IdMessaggio";

        var parameters = new[]
        {
            new SqlParameter("@IdMessaggio", messaggio.IdMessaggio),
            new SqlParameter("@TestoMessaggio", GetValueOrDBNull(messaggio.TestoMessaggio)),
            new SqlParameter("@Sentiment", GetValueOrDBNull(messaggio.Sentiment)),
            new SqlParameter("@IntentRilevato", GetValueOrDBNull(messaggio.IntentRilevato)),
            new SqlParameter("@ConfidenzaIntent", GetValueOrDBNull(messaggio.ConfidenzaIntent)),
            new SqlParameter("@TokenUtilizzati", GetValueOrDBNull(messaggio.TokenUtilizzati)),
            new SqlParameter("@ModelloAIUtilizzato", GetValueOrDBNull(messaggio.ModelloAIUtilizzato))
        };

        var rowsAffected = await ExecuteNonQueryAsync(query, parameters);
        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(long idMessaggio)
    {
        const string query = "DELETE FROM Messaggi WHERE IdMessaggio = @IdMessaggio";

        var parameters = new[]
        {
            new SqlParameter("@IdMessaggio", idMessaggio)
        };

        var rowsAffected = await ExecuteNonQueryAsync(query, parameters);
        return rowsAffected > 0;
    }

    public async Task<int> CountByConversazioneAsync(int idConversazione)
    {
        const string query = "SELECT COUNT(1) FROM Messaggi WHERE IdConversazione = @IdConversazione";

        var parameters = new[]
        {
            new SqlParameter("@IdConversazione", idConversazione)
        };

        return await ExecuteScalarAsync<int>(query, parameters);
    }

    /// <summary>
    /// Mappa un SqlDataReader su un oggetto MessaggioEntityModel
    /// </summary>
    private MessaggioEntityModel MapMessaggio(SqlDataReader reader)
    {
        return new MessaggioEntityModel
        {
            IdMessaggio = reader.GetInt64(reader.GetOrdinal("IdMessaggio")),
            IdConversazione = reader.GetInt32(reader.GetOrdinal("IdConversazione")),
            IdUtente = GetNullableValue<int>(reader, "IdUtente"),
            TipoMittente = reader.GetString(reader.GetOrdinal("TipoMittente")),
            TestoMessaggio = GetNullableString(reader, "TestoMessaggio"),
            TipoContenuto = reader.GetString(reader.GetOrdinal("TipoContenuto")),
            URLContenuto = GetNullableString(reader, "URLContenuto"),
            DurataContenuto = GetNullableValue<int>(reader, "DurataContenuto"),
            DimensioneFile = GetNullableValue<long>(reader, "DimensioneFile"),
            MimeType = GetNullableString(reader, "MimeType"),
            MetadatiContenuto = GetNullableString(reader, "MetadatiContenuto"),
            Sentiment = GetNullableString(reader, "Sentiment"),
            IntentRilevato = GetNullableString(reader, "IntentRilevato"),
            ConfidenzaIntent = GetNullableValue<decimal>(reader, "ConfidenzaIntent"),
            TokenUtilizzati = GetNullableValue<int>(reader, "TokenUtilizzati"),
            ModelloAIUtilizzato = GetNullableString(reader, "ModelloAIUtilizzato"),
            DataInserimento = reader.GetDateTime(reader.GetOrdinal("DataInserimento")),
            UtenteInserimento = reader.GetString(reader.GetOrdinal("UtenteInserimento")),
            IPInserimento = GetNullableString(reader, "IPInserimento")
        };
    }
}
