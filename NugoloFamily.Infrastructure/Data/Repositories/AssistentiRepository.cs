using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using NugoloFamily.Core.Interfaces.Repositories;
using NugoloFamily.Core.Models.Entities;

namespace NugoloFamily.Infrastructure.Data.Repositories;

/// <summary>
/// Repository per la gestione degli Assistenti tramite ADO.NET
/// </summary>
public class AssistentiRepository : BaseRepository, IAssistentiRepository
{
    public AssistentiRepository(IConfiguration configuration) : base(configuration)
    {
    }

    public async Task<AssistenteEntityModel?> GetByIdAsync(int idAssistente)
    {
        const string query = @"
            SELECT IdAssistente, NomeAssistente, CodiceAssistente, Descrizione,
                   Icona, Categoria, Attivo, Ordinamento, RichiedeConfigurazione,
                   ParametriConfigurazione,
                   DataInserimento, UtenteInserimento, IPInserimento,
                   DataModifica, UtenteModifica, IPModifica
            FROM Assistenti
            WHERE IdAssistente = @IdAssistente";

        var parameters = new[]
        {
            new SqlParameter("@IdAssistente", idAssistente)
        };

        return await ExecuteReaderSingleAsync(query, MapAssistente, parameters);
    }

    public async Task<AssistenteEntityModel?> GetByCodiceAsync(string codiceAssistente)
    {
        const string query = @"
            SELECT IdAssistente, NomeAssistente, CodiceAssistente, Descrizione,
                   Icona, Categoria, Attivo, Ordinamento, RichiedeConfigurazione,
                   ParametriConfigurazione,
                   DataInserimento, UtenteInserimento, IPInserimento,
                   DataModifica, UtenteModifica, IPModifica
            FROM Assistenti
            WHERE CodiceAssistente = @CodiceAssistente";

        var parameters = new[]
        {
            new SqlParameter("@CodiceAssistente", codiceAssistente)
        };

        return await ExecuteReaderSingleAsync(query, MapAssistente, parameters);
    }

    public async Task<IEnumerable<AssistenteEntityModel>> GetAllAsync()
    {
        const string query = @"
            SELECT IdAssistente, NomeAssistente, CodiceAssistente, Descrizione,
                   Icona, Categoria, Attivo, Ordinamento, RichiedeConfigurazione,
                   ParametriConfigurazione,
                   DataInserimento, UtenteInserimento, IPInserimento,
                   DataModifica, UtenteModifica, IPModifica
            FROM Assistenti
            ORDER BY Ordinamento, NomeAssistente";

        return await ExecuteReaderAsync(query, MapAssistente);
    }

    public async Task<IEnumerable<AssistenteEntityModel>> GetAttiviAsync()
    {
        const string query = @"
            SELECT IdAssistente, NomeAssistente, CodiceAssistente, Descrizione,
                   Icona, Categoria, Attivo, Ordinamento, RichiedeConfigurazione,
                   ParametriConfigurazione,
                   DataInserimento, UtenteInserimento, IPInserimento,
                   DataModifica, UtenteModifica, IPModifica
            FROM Assistenti
            WHERE Attivo = 1
            ORDER BY Ordinamento, NomeAssistente";

        return await ExecuteReaderAsync(query, MapAssistente);
    }

    public async Task<IEnumerable<AssistenteEntityModel>> GetByCategoriaAsync(string categoria)
    {
        const string query = @"
            SELECT IdAssistente, NomeAssistente, CodiceAssistente, Descrizione,
                   Icona, Categoria, Attivo, Ordinamento, RichiedeConfigurazione,
                   ParametriConfigurazione,
                   DataInserimento, UtenteInserimento, IPInserimento,
                   DataModifica, UtenteModifica, IPModifica
            FROM Assistenti
            WHERE Categoria = @Categoria
            ORDER BY Ordinamento, NomeAssistente";

        var parameters = new[]
        {
            new SqlParameter("@Categoria", categoria)
        };

        return await ExecuteReaderAsync(query, MapAssistente, parameters);
    }

    public async Task<int> CreateAsync(AssistenteEntityModel assistente)
    {
        const string query = @"
            INSERT INTO Assistenti (NomeAssistente, CodiceAssistente, Descrizione,
                                    Icona, Categoria, Attivo, Ordinamento,
                                    RichiedeConfigurazione, ParametriConfigurazione,
                                    DataInserimento, UtenteInserimento, IPInserimento)
            VALUES (@NomeAssistente, @CodiceAssistente, @Descrizione,
                    @Icona, @Categoria, @Attivo, @Ordinamento,
                    @RichiedeConfigurazione, @ParametriConfigurazione,
                    @DataInserimento, @UtenteInserimento, @IPInserimento);
            SELECT CAST(SCOPE_IDENTITY() AS INT);";

        var parameters = new[]
        {
            new SqlParameter("@NomeAssistente", assistente.NomeAssistente),
            new SqlParameter("@CodiceAssistente", assistente.CodiceAssistente),
            new SqlParameter("@Descrizione", GetValueOrDBNull(assistente.Descrizione)),
            new SqlParameter("@Icona", GetValueOrDBNull(assistente.Icona)),
            new SqlParameter("@Categoria", GetValueOrDBNull(assistente.Categoria)),
            new SqlParameter("@Attivo", assistente.Attivo),
            new SqlParameter("@Ordinamento", assistente.Ordinamento),
            new SqlParameter("@RichiedeConfigurazione", assistente.RichiedeConfigurazione),
            new SqlParameter("@ParametriConfigurazione", GetValueOrDBNull(assistente.ParametriConfigurazione)),
            new SqlParameter("@DataInserimento", assistente.DataInserimento),
            new SqlParameter("@UtenteInserimento", assistente.UtenteInserimento),
            new SqlParameter("@IPInserimento", GetValueOrDBNull(assistente.IPInserimento))
        };

        return await ExecuteScalarAsync<int>(query, parameters);
    }

    public async Task<bool> UpdateAsync(AssistenteEntityModel assistente)
    {
        const string query = @"
            UPDATE Assistenti
            SET NomeAssistente = @NomeAssistente,
                Descrizione = @Descrizione,
                Icona = @Icona,
                Categoria = @Categoria,
                Attivo = @Attivo,
                Ordinamento = @Ordinamento,
                RichiedeConfigurazione = @RichiedeConfigurazione,
                ParametriConfigurazione = @ParametriConfigurazione,
                DataModifica = @DataModifica,
                UtenteModifica = @UtenteModifica,
                IPModifica = @IPModifica
            WHERE IdAssistente = @IdAssistente";

        var parameters = new[]
        {
            new SqlParameter("@IdAssistente", assistente.IdAssistente),
            new SqlParameter("@NomeAssistente", assistente.NomeAssistente),
            new SqlParameter("@Descrizione", GetValueOrDBNull(assistente.Descrizione)),
            new SqlParameter("@Icona", GetValueOrDBNull(assistente.Icona)),
            new SqlParameter("@Categoria", GetValueOrDBNull(assistente.Categoria)),
            new SqlParameter("@Attivo", assistente.Attivo),
            new SqlParameter("@Ordinamento", assistente.Ordinamento),
            new SqlParameter("@RichiedeConfigurazione", assistente.RichiedeConfigurazione),
            new SqlParameter("@ParametriConfigurazione", GetValueOrDBNull(assistente.ParametriConfigurazione)),
            new SqlParameter("@DataModifica", GetValueOrDBNull(assistente.DataModifica)),
            new SqlParameter("@UtenteModifica", GetValueOrDBNull(assistente.UtenteModifica)),
            new SqlParameter("@IPModifica", GetValueOrDBNull(assistente.IPModifica))
        };

        var rowsAffected = await ExecuteNonQueryAsync(query, parameters);
        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(int idAssistente)
    {
        const string query = "DELETE FROM Assistenti WHERE IdAssistente = @IdAssistente";

        var parameters = new[]
        {
            new SqlParameter("@IdAssistente", idAssistente)
        };

        var rowsAffected = await ExecuteNonQueryAsync(query, parameters);
        return rowsAffected > 0;
    }

    /// <summary>
    /// Mappa un SqlDataReader su un oggetto AssistenteEntityModel
    /// </summary>
    private AssistenteEntityModel MapAssistente(SqlDataReader reader)
    {
        return new AssistenteEntityModel
        {
            IdAssistente = reader.GetInt32(reader.GetOrdinal("IdAssistente")),
            NomeAssistente = reader.GetString(reader.GetOrdinal("NomeAssistente")),
            CodiceAssistente = reader.GetString(reader.GetOrdinal("CodiceAssistente")),
            Descrizione = GetNullableString(reader, "Descrizione"),
            Icona = GetNullableString(reader, "Icona"),
            Categoria = GetNullableString(reader, "Categoria"),
            Attivo = reader.GetBoolean(reader.GetOrdinal("Attivo")),
            Ordinamento = reader.GetInt32(reader.GetOrdinal("Ordinamento")),
            RichiedeConfigurazione = reader.GetBoolean(reader.GetOrdinal("RichiedeConfigurazione")),
            ParametriConfigurazione = GetNullableString(reader, "ParametriConfigurazione"),
            DataInserimento = reader.GetDateTime(reader.GetOrdinal("DataInserimento")),
            UtenteInserimento = reader.GetString(reader.GetOrdinal("UtenteInserimento")),
            IPInserimento = GetNullableString(reader, "IPInserimento"),
            DataModifica = GetNullableDateTime(reader, "DataModifica"),
            UtenteModifica = GetNullableString(reader, "UtenteModifica"),
            IPModifica = GetNullableString(reader, "IPModifica")
        };
    }
}
