using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using NugoloFamily.Core.Interfaces.Repositories;
using NugoloFamily.Core.Models.Entities;

namespace NugoloFamily.Infrastructure.Data.Repositories;

/// <summary>
/// Repository per la gestione dei Documenti tramite ADO.NET
/// </summary>
public class DocumentiRepository : BaseRepository, IDocumentiRepository
{
    public DocumentiRepository(IConfiguration configuration) : base(configuration)
    {
    }

    public async Task<DocumentoEntityModel?> GetByIdAsync(int idDocumento)
    {
        const string query = @"
            SELECT IdDocumento, IdFamiglia, IdUtente, NomeFile, PercorsoFile,
                   TipoDocumento, Categoria, DimensioneFile, MimeType,
                   TestoEstratto, MetadatiDocumento, Indicizzato,
                   AssistenteAssociato, DataDocumento, DataScadenza,
                   DataInserimento, UtenteInserimento, IPInserimento,
                   DataModifica, UtenteModifica, IPModifica
            FROM Documenti
            WHERE IdDocumento = @IdDocumento";

        var parameters = new[]
        {
            new SqlParameter("@IdDocumento", idDocumento)
        };

        return await ExecuteReaderSingleAsync(query, MapDocumento, parameters);
    }

    public async Task<IEnumerable<DocumentoEntityModel>> GetByFamigliaAsync(int idFamiglia)
    {
        const string query = @"
            SELECT IdDocumento, IdFamiglia, IdUtente, NomeFile, PercorsoFile,
                   TipoDocumento, Categoria, DimensioneFile, MimeType,
                   TestoEstratto, MetadatiDocumento, Indicizzato,
                   AssistenteAssociato, DataDocumento, DataScadenza,
                   DataInserimento, UtenteInserimento, IPInserimento,
                   DataModifica, UtenteModifica, IPModifica
            FROM Documenti
            WHERE IdFamiglia = @IdFamiglia
            ORDER BY DataInserimento DESC";

        var parameters = new[]
        {
            new SqlParameter("@IdFamiglia", idFamiglia)
        };

        return await ExecuteReaderAsync(query, MapDocumento, parameters);
    }

    public async Task<IEnumerable<DocumentoEntityModel>> GetByUtenteAsync(int idUtente)
    {
        const string query = @"
            SELECT IdDocumento, IdFamiglia, IdUtente, NomeFile, PercorsoFile,
                   TipoDocumento, Categoria, DimensioneFile, MimeType,
                   TestoEstratto, MetadatiDocumento, Indicizzato,
                   AssistenteAssociato, DataDocumento, DataScadenza,
                   DataInserimento, UtenteInserimento, IPInserimento,
                   DataModifica, UtenteModifica, IPModifica
            FROM Documenti
            WHERE IdUtente = @IdUtente
            ORDER BY DataInserimento DESC";

        var parameters = new[]
        {
            new SqlParameter("@IdUtente", idUtente)
        };

        return await ExecuteReaderAsync(query, MapDocumento, parameters);
    }

    public async Task<IEnumerable<DocumentoEntityModel>> GetByTipoAsync(int idFamiglia, string tipoDocumento)
    {
        const string query = @"
            SELECT IdDocumento, IdFamiglia, IdUtente, NomeFile, PercorsoFile,
                   TipoDocumento, Categoria, DimensioneFile, MimeType,
                   TestoEstratto, MetadatiDocumento, Indicizzato,
                   AssistenteAssociato, DataDocumento, DataScadenza,
                   DataInserimento, UtenteInserimento, IPInserimento,
                   DataModifica, UtenteModifica, IPModifica
            FROM Documenti
            WHERE IdFamiglia = @IdFamiglia AND TipoDocumento = @TipoDocumento
            ORDER BY DataInserimento DESC";

        var parameters = new[]
        {
            new SqlParameter("@IdFamiglia", idFamiglia),
            new SqlParameter("@TipoDocumento", tipoDocumento)
        };

        return await ExecuteReaderAsync(query, MapDocumento, parameters);
    }

    public async Task<IEnumerable<DocumentoEntityModel>> GetByCategoriaAsync(int idFamiglia, string categoria)
    {
        const string query = @"
            SELECT IdDocumento, IdFamiglia, IdUtente, NomeFile, PercorsoFile,
                   TipoDocumento, Categoria, DimensioneFile, MimeType,
                   TestoEstratto, MetadatiDocumento, Indicizzato,
                   AssistenteAssociato, DataDocumento, DataScadenza,
                   DataInserimento, UtenteInserimento, IPInserimento,
                   DataModifica, UtenteModifica, IPModifica
            FROM Documenti
            WHERE IdFamiglia = @IdFamiglia AND Categoria = @Categoria
            ORDER BY DataInserimento DESC";

        var parameters = new[]
        {
            new SqlParameter("@IdFamiglia", idFamiglia),
            new SqlParameter("@Categoria", categoria)
        };

        return await ExecuteReaderAsync(query, MapDocumento, parameters);
    }

    public async Task<IEnumerable<DocumentoEntityModel>> GetByAssistenteAsync(int idAssistente)
    {
        const string query = @"
            SELECT IdDocumento, IdFamiglia, IdUtente, NomeFile, PercorsoFile,
                   TipoDocumento, Categoria, DimensioneFile, MimeType,
                   TestoEstratto, MetadatiDocumento, Indicizzato,
                   AssistenteAssociato, DataDocumento, DataScadenza,
                   DataInserimento, UtenteInserimento, IPInserimento,
                   DataModifica, UtenteModifica, IPModifica
            FROM Documenti
            WHERE AssistenteAssociato = @IdAssistente
            ORDER BY DataInserimento DESC";

        var parameters = new[]
        {
            new SqlParameter("@IdAssistente", idAssistente)
        };

        return await ExecuteReaderAsync(query, MapDocumento, parameters);
    }

    public async Task<IEnumerable<DocumentoEntityModel>> GetDaIndicizzareAsync(int limite)
    {
        const string query = @"
            SELECT TOP(@Limite)
                   IdDocumento, IdFamiglia, IdUtente, NomeFile, PercorsoFile,
                   TipoDocumento, Categoria, DimensioneFile, MimeType,
                   TestoEstratto, MetadatiDocumento, Indicizzato,
                   AssistenteAssociato, DataDocumento, DataScadenza,
                   DataInserimento, UtenteInserimento, IPInserimento,
                   DataModifica, UtenteModifica, IPModifica
            FROM Documenti
            WHERE Indicizzato = 0
            ORDER BY DataInserimento ASC";

        var parameters = new[]
        {
            new SqlParameter("@Limite", limite)
        };

        return await ExecuteReaderAsync(query, MapDocumento, parameters);
    }

    public async Task<int> CreateAsync(DocumentoEntityModel documento)
    {
        const string query = @"
            INSERT INTO Documenti (IdFamiglia, IdUtente, NomeFile, PercorsoFile,
                                   TipoDocumento, Categoria, DimensioneFile, MimeType,
                                   TestoEstratto, MetadatiDocumento, Indicizzato,
                                   AssistenteAssociato, DataDocumento, DataScadenza,
                                   DataInserimento, UtenteInserimento, IPInserimento)
            VALUES (@IdFamiglia, @IdUtente, @NomeFile, @PercorsoFile,
                    @TipoDocumento, @Categoria, @DimensioneFile, @MimeType,
                    @TestoEstratto, @MetadatiDocumento, @Indicizzato,
                    @AssistenteAssociato, @DataDocumento, @DataScadenza,
                    @DataInserimento, @UtenteInserimento, @IPInserimento);
            SELECT CAST(SCOPE_IDENTITY() AS INT);";

        var parameters = new[]
        {
            new SqlParameter("@IdFamiglia", documento.IdFamiglia),
            new SqlParameter("@IdUtente", documento.IdUtente),
            new SqlParameter("@NomeFile", documento.NomeFile),
            new SqlParameter("@PercorsoFile", documento.PercorsoFile),
            new SqlParameter("@TipoDocumento", GetValueOrDBNull(documento.TipoDocumento)),
            new SqlParameter("@Categoria", GetValueOrDBNull(documento.Categoria)),
            new SqlParameter("@DimensioneFile", GetValueOrDBNull(documento.DimensioneFile)),
            new SqlParameter("@MimeType", GetValueOrDBNull(documento.MimeType)),
            new SqlParameter("@TestoEstratto", GetValueOrDBNull(documento.TestoEstratto)),
            new SqlParameter("@MetadatiDocumento", GetValueOrDBNull(documento.MetadatiDocumento)),
            new SqlParameter("@Indicizzato", documento.Indicizzato),
            new SqlParameter("@AssistenteAssociato", GetValueOrDBNull(documento.AssistenteAssociato)),
            new SqlParameter("@DataDocumento", GetValueOrDBNull(documento.DataDocumento)),
            new SqlParameter("@DataScadenza", GetValueOrDBNull(documento.DataScadenza)),
            new SqlParameter("@DataInserimento", documento.DataInserimento),
            new SqlParameter("@UtenteInserimento", documento.UtenteInserimento),
            new SqlParameter("@IPInserimento", GetValueOrDBNull(documento.IPInserimento))
        };

        return await ExecuteScalarAsync<int>(query, parameters);
    }

    public async Task<bool> UpdateAsync(DocumentoEntityModel documento)
    {
        const string query = @"
            UPDATE Documenti
            SET NomeFile = @NomeFile,
                TipoDocumento = @TipoDocumento,
                Categoria = @Categoria,
                TestoEstratto = @TestoEstratto,
                MetadatiDocumento = @MetadatiDocumento,
                Indicizzato = @Indicizzato,
                AssistenteAssociato = @AssistenteAssociato,
                DataDocumento = @DataDocumento,
                DataScadenza = @DataScadenza,
                DataModifica = @DataModifica,
                UtenteModifica = @UtenteModifica,
                IPModifica = @IPModifica
            WHERE IdDocumento = @IdDocumento";

        var parameters = new[]
        {
            new SqlParameter("@IdDocumento", documento.IdDocumento),
            new SqlParameter("@NomeFile", documento.NomeFile),
            new SqlParameter("@TipoDocumento", GetValueOrDBNull(documento.TipoDocumento)),
            new SqlParameter("@Categoria", GetValueOrDBNull(documento.Categoria)),
            new SqlParameter("@TestoEstratto", GetValueOrDBNull(documento.TestoEstratto)),
            new SqlParameter("@MetadatiDocumento", GetValueOrDBNull(documento.MetadatiDocumento)),
            new SqlParameter("@Indicizzato", documento.Indicizzato),
            new SqlParameter("@AssistenteAssociato", GetValueOrDBNull(documento.AssistenteAssociato)),
            new SqlParameter("@DataDocumento", GetValueOrDBNull(documento.DataDocumento)),
            new SqlParameter("@DataScadenza", GetValueOrDBNull(documento.DataScadenza)),
            new SqlParameter("@DataModifica", GetValueOrDBNull(documento.DataModifica)),
            new SqlParameter("@UtenteModifica", GetValueOrDBNull(documento.UtenteModifica)),
            new SqlParameter("@IPModifica", GetValueOrDBNull(documento.IPModifica))
        };

        var rowsAffected = await ExecuteNonQueryAsync(query, parameters);
        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(int idDocumento)
    {
        const string query = "DELETE FROM Documenti WHERE IdDocumento = @IdDocumento";

        var parameters = new[]
        {
            new SqlParameter("@IdDocumento", idDocumento)
        };

        var rowsAffected = await ExecuteNonQueryAsync(query, parameters);
        return rowsAffected > 0;
    }

    public async Task<bool> UpdateTestoEstrattoAsync(int idDocumento, string testoEstratto, string? metadati)
    {
        const string query = @"
            UPDATE Documenti
            SET TestoEstratto = @TestoEstratto,
                MetadatiDocumento = @MetadatiDocumento
            WHERE IdDocumento = @IdDocumento";

        var parameters = new[]
        {
            new SqlParameter("@IdDocumento", idDocumento),
            new SqlParameter("@TestoEstratto", testoEstratto),
            new SqlParameter("@MetadatiDocumento", GetValueOrDBNull(metadati))
        };

        var rowsAffected = await ExecuteNonQueryAsync(query, parameters);
        return rowsAffected > 0;
    }

    public async Task<bool> SegnaIndicizzatoAsync(int idDocumento)
    {
        const string query = @"
            UPDATE Documenti
            SET Indicizzato = 1
            WHERE IdDocumento = @IdDocumento";

        var parameters = new[]
        {
            new SqlParameter("@IdDocumento", idDocumento)
        };

        var rowsAffected = await ExecuteNonQueryAsync(query, parameters);
        return rowsAffected > 0;
    }

    /// <summary>
    /// Mappa un SqlDataReader su un oggetto DocumentoEntityModel
    /// </summary>
    private DocumentoEntityModel MapDocumento(SqlDataReader reader)
    {
        return new DocumentoEntityModel
        {
            IdDocumento = reader.GetInt32(reader.GetOrdinal("IdDocumento")),
            IdFamiglia = reader.GetInt32(reader.GetOrdinal("IdFamiglia")),
            IdUtente = reader.GetInt32(reader.GetOrdinal("IdUtente")),
            NomeFile = reader.GetString(reader.GetOrdinal("NomeFile")),
            PercorsoFile = reader.GetString(reader.GetOrdinal("PercorsoFile")),
            TipoDocumento = GetNullableString(reader, "TipoDocumento"),
            Categoria = GetNullableString(reader, "Categoria"),
            DimensioneFile = GetNullableValue<long>(reader, "DimensioneFile"),
            MimeType = GetNullableString(reader, "MimeType"),
            TestoEstratto = GetNullableString(reader, "TestoEstratto"),
            MetadatiDocumento = GetNullableString(reader, "MetadatiDocumento"),
            Indicizzato = reader.GetBoolean(reader.GetOrdinal("Indicizzato")),
            AssistenteAssociato = GetNullableValue<int>(reader, "AssistenteAssociato"),
            DataDocumento = GetNullableDateTime(reader, "DataDocumento"),
            DataScadenza = GetNullableDateTime(reader, "DataScadenza"),
            DataInserimento = reader.GetDateTime(reader.GetOrdinal("DataInserimento")),
            UtenteInserimento = reader.GetString(reader.GetOrdinal("UtenteInserimento")),
            IPInserimento = GetNullableString(reader, "IPInserimento"),
            DataModifica = GetNullableDateTime(reader, "DataModifica"),
            UtenteModifica = GetNullableString(reader, "UtenteModifica"),
            IPModifica = GetNullableString(reader, "IPModifica")
        };
    }
}
