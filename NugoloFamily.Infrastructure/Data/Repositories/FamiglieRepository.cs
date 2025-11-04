using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using NugoloFamily.Core.Interfaces.Repositories;
using NugoloFamily.Core.Models.Entities;

namespace NugoloFamily.Infrastructure.Data.Repositories;

/// <summary>
/// Repository per la gestione delle Famiglie tramite ADO.NET
/// </summary>
public class FamiglieRepository : BaseRepository, IFamiglieRepository
{
    public FamiglieRepository(IConfiguration configuration) : base(configuration)
    {
    }

    public async Task<FamigliaEntityModel?> GetByIdAsync(int idFamiglia)
    {
        const string query = @"
            SELECT IdFamiglia, NomeFamiglia, CodiceUnivoco, Email, Telefono, Indirizzo,
                   StatoAttivazione, DataScadenzaAbbonamento, PianoAbbonamento, LimiteUtenti,
                   DataInserimento, UtenteInserimento, IPInserimento,
                   DataModifica, UtenteModifica, IPModifica
            FROM Famiglie
            WHERE IdFamiglia = @IdFamiglia";

        var parameters = new[]
        {
            new SqlParameter("@IdFamiglia", idFamiglia)
        };

        return await ExecuteReaderSingleAsync(query, MapFamiglia, parameters);
    }

    public async Task<FamigliaEntityModel?> GetByCodiceUnivocoAsync(string codiceUnivoco)
    {
        const string query = @"
            SELECT IdFamiglia, NomeFamiglia, CodiceUnivoco, Email, Telefono, Indirizzo,
                   StatoAttivazione, DataScadenzaAbbonamento, PianoAbbonamento, LimiteUtenti,
                   DataInserimento, UtenteInserimento, IPInserimento,
                   DataModifica, UtenteModifica, IPModifica
            FROM Famiglie
            WHERE CodiceUnivoco = @CodiceUnivoco";

        var parameters = new[]
        {
            new SqlParameter("@CodiceUnivoco", codiceUnivoco)
        };

        return await ExecuteReaderSingleAsync(query, MapFamiglia, parameters);
    }

    public async Task<IEnumerable<FamigliaEntityModel>> GetAllAsync()
    {
        const string query = @"
            SELECT IdFamiglia, NomeFamiglia, CodiceUnivoco, Email, Telefono, Indirizzo,
                   StatoAttivazione, DataScadenzaAbbonamento, PianoAbbonamento, LimiteUtenti,
                   DataInserimento, UtenteInserimento, IPInserimento,
                   DataModifica, UtenteModifica, IPModifica
            FROM Famiglie
            ORDER BY NomeFamiglia";

        return await ExecuteReaderAsync(query, MapFamiglia);
    }

    public async Task<IEnumerable<FamigliaEntityModel>> GetAttiviAsync()
    {
        const string query = @"
            SELECT IdFamiglia, NomeFamiglia, CodiceUnivoco, Email, Telefono, Indirizzo,
                   StatoAttivazione, DataScadenzaAbbonamento, PianoAbbonamento, LimiteUtenti,
                   DataInserimento, UtenteInserimento, IPInserimento,
                   DataModifica, UtenteModifica, IPModifica
            FROM Famiglie
            WHERE StatoAttivazione = 'Attivo'
            ORDER BY NomeFamiglia";

        return await ExecuteReaderAsync(query, MapFamiglia);
    }

    public async Task<int> CreateAsync(FamigliaEntityModel famiglia)
    {
        const string query = @"
            INSERT INTO Famiglie (NomeFamiglia, CodiceUnivoco, Email, Telefono, Indirizzo,
                                  StatoAttivazione, DataScadenzaAbbonamento, PianoAbbonamento, LimiteUtenti,
                                  DataInserimento, UtenteInserimento, IPInserimento)
            VALUES (@NomeFamiglia, @CodiceUnivoco, @Email, @Telefono, @Indirizzo,
                    @StatoAttivazione, @DataScadenzaAbbonamento, @PianoAbbonamento, @LimiteUtenti,
                    @DataInserimento, @UtenteInserimento, @IPInserimento);
            SELECT CAST(SCOPE_IDENTITY() AS INT);";

        var parameters = new[]
        {
            new SqlParameter("@NomeFamiglia", famiglia.NomeFamiglia),
            new SqlParameter("@CodiceUnivoco", famiglia.CodiceUnivoco),
            new SqlParameter("@Email", famiglia.Email),
            new SqlParameter("@Telefono", GetValueOrDBNull(famiglia.Telefono)),
            new SqlParameter("@Indirizzo", GetValueOrDBNull(famiglia.Indirizzo)),
            new SqlParameter("@StatoAttivazione", famiglia.StatoAttivazione),
            new SqlParameter("@DataScadenzaAbbonamento", GetValueOrDBNull(famiglia.DataScadenzaAbbonamento)),
            new SqlParameter("@PianoAbbonamento", GetValueOrDBNull(famiglia.PianoAbbonamento)),
            new SqlParameter("@LimiteUtenti", famiglia.LimiteUtenti),
            new SqlParameter("@DataInserimento", famiglia.DataInserimento),
            new SqlParameter("@UtenteInserimento", famiglia.UtenteInserimento),
            new SqlParameter("@IPInserimento", GetValueOrDBNull(famiglia.IPInserimento))
        };

        return await ExecuteScalarAsync<int>(query, parameters);
    }

    public async Task<bool> UpdateAsync(FamigliaEntityModel famiglia)
    {
        const string query = @"
            UPDATE Famiglie
            SET NomeFamiglia = @NomeFamiglia,
                Email = @Email,
                Telefono = @Telefono,
                Indirizzo = @Indirizzo,
                StatoAttivazione = @StatoAttivazione,
                DataScadenzaAbbonamento = @DataScadenzaAbbonamento,
                PianoAbbonamento = @PianoAbbonamento,
                LimiteUtenti = @LimiteUtenti,
                DataModifica = @DataModifica,
                UtenteModifica = @UtenteModifica,
                IPModifica = @IPModifica
            WHERE IdFamiglia = @IdFamiglia";

        var parameters = new[]
        {
            new SqlParameter("@IdFamiglia", famiglia.IdFamiglia),
            new SqlParameter("@NomeFamiglia", famiglia.NomeFamiglia),
            new SqlParameter("@Email", famiglia.Email),
            new SqlParameter("@Telefono", GetValueOrDBNull(famiglia.Telefono)),
            new SqlParameter("@Indirizzo", GetValueOrDBNull(famiglia.Indirizzo)),
            new SqlParameter("@StatoAttivazione", famiglia.StatoAttivazione),
            new SqlParameter("@DataScadenzaAbbonamento", GetValueOrDBNull(famiglia.DataScadenzaAbbonamento)),
            new SqlParameter("@PianoAbbonamento", GetValueOrDBNull(famiglia.PianoAbbonamento)),
            new SqlParameter("@LimiteUtenti", famiglia.LimiteUtenti),
            new SqlParameter("@DataModifica", GetValueOrDBNull(famiglia.DataModifica)),
            new SqlParameter("@UtenteModifica", GetValueOrDBNull(famiglia.UtenteModifica)),
            new SqlParameter("@IPModifica", GetValueOrDBNull(famiglia.IPModifica))
        };

        var rowsAffected = await ExecuteNonQueryAsync(query, parameters);
        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(int idFamiglia)
    {
        const string query = "DELETE FROM Famiglie WHERE IdFamiglia = @IdFamiglia";

        var parameters = new[]
        {
            new SqlParameter("@IdFamiglia", idFamiglia)
        };

        var rowsAffected = await ExecuteNonQueryAsync(query, parameters);
        return rowsAffected > 0;
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        const string query = "SELECT COUNT(1) FROM Famiglie WHERE Email = @Email";

        var parameters = new[]
        {
            new SqlParameter("@Email", email)
        };

        var count = await ExecuteScalarAsync<int>(query, parameters);
        return count > 0;
    }

    public async Task<bool> ExistsByCodiceUnivocoAsync(string codiceUnivoco)
    {
        const string query = "SELECT COUNT(1) FROM Famiglie WHERE CodiceUnivoco = @CodiceUnivoco";

        var parameters = new[]
        {
            new SqlParameter("@CodiceUnivoco", codiceUnivoco)
        };

        var count = await ExecuteScalarAsync<int>(query, parameters);
        return count > 0;
    }

    /// <summary>
    /// Mappa un SqlDataReader su un oggetto FamigliaEntityModel
    /// </summary>
    private FamigliaEntityModel MapFamiglia(SqlDataReader reader)
    {
        return new FamigliaEntityModel
        {
            IdFamiglia = reader.GetInt32(reader.GetOrdinal("IdFamiglia")),
            NomeFamiglia = reader.GetString(reader.GetOrdinal("NomeFamiglia")),
            CodiceUnivoco = reader.GetString(reader.GetOrdinal("CodiceUnivoco")),
            Email = reader.GetString(reader.GetOrdinal("Email")),
            Telefono = GetNullableString(reader, "Telefono"),
            Indirizzo = GetNullableString(reader, "Indirizzo"),
            StatoAttivazione = reader.GetString(reader.GetOrdinal("StatoAttivazione")),
            DataScadenzaAbbonamento = GetNullableDateTime(reader, "DataScadenzaAbbonamento"),
            PianoAbbonamento = GetNullableString(reader, "PianoAbbonamento"),
            LimiteUtenti = reader.GetInt32(reader.GetOrdinal("LimiteUtenti")),
            DataInserimento = reader.GetDateTime(reader.GetOrdinal("DataInserimento")),
            UtenteInserimento = reader.GetString(reader.GetOrdinal("UtenteInserimento")),
            IPInserimento = GetNullableString(reader, "IPInserimento"),
            DataModifica = GetNullableDateTime(reader, "DataModifica"),
            UtenteModifica = GetNullableString(reader, "UtenteModifica"),
            IPModifica = GetNullableString(reader, "IPModifica")
        };
    }
}
