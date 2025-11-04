using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace NugoloFamily.Infrastructure.Data;

/// <summary>
/// Classe base per tutti i repository
/// Fornisce la gestione della connessione al database tramite ADO.NET
/// </summary>
public abstract class BaseRepository
{
    protected readonly string ConnectionString;

    protected BaseRepository(IConfiguration configuration)
    {
        ConnectionString = configuration.GetConnectionString("NugoloFamilyDB")
            ?? throw new InvalidOperationException("Connection string 'NugoloFamilyDB' non trovata");
    }

    /// <summary>
    /// Crea una nuova connessione al database
    /// </summary>
    protected SqlConnection CreateConnection()
    {
        return new SqlConnection(ConnectionString);
    }

    /// <summary>
    /// Esegue una query e restituisce un singolo valore
    /// </summary>
    protected async Task<T?> ExecuteScalarAsync<T>(string query, SqlParameter[]? parameters = null)
    {
        using var connection = CreateConnection();
        using var command = new SqlCommand(query, connection);

        if (parameters != null)
        {
            command.Parameters.AddRange(parameters);
        }

        await connection.OpenAsync();
        var result = await command.ExecuteScalarAsync();

        return result == null || result == DBNull.Value ? default : (T)result;
    }

    /// <summary>
    /// Esegue un comando (INSERT, UPDATE, DELETE) e restituisce il numero di righe modificate
    /// </summary>
    protected async Task<int> ExecuteNonQueryAsync(string query, SqlParameter[]? parameters = null)
    {
        using var connection = CreateConnection();
        using var command = new SqlCommand(query, connection);

        if (parameters != null)
        {
            command.Parameters.AddRange(parameters);
        }

        await connection.OpenAsync();
        return await command.ExecuteNonQueryAsync();
    }

    /// <summary>
    /// Esegue una query e mappa i risultati usando una funzione di mapping
    /// </summary>
    protected async Task<IEnumerable<T>> ExecuteReaderAsync<T>(string query, Func<SqlDataReader, T> mapFunction, SqlParameter[]? parameters = null)
    {
        var results = new List<T>();

        using var connection = CreateConnection();
        using var command = new SqlCommand(query, connection);

        if (parameters != null)
        {
            command.Parameters.AddRange(parameters);
        }

        await connection.OpenAsync();
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            results.Add(mapFunction(reader));
        }

        return results;
    }

    /// <summary>
    /// Esegue una query e restituisce un singolo risultato
    /// </summary>
    protected async Task<T?> ExecuteReaderSingleAsync<T>(string query, Func<SqlDataReader, T> mapFunction, SqlParameter[]? parameters = null) where T : class
    {
        using var connection = CreateConnection();
        using var command = new SqlCommand(query, connection);

        if (parameters != null)
        {
            command.Parameters.AddRange(parameters);
        }

        await connection.OpenAsync();
        using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return mapFunction(reader);
        }

        return null;
    }

    /// <summary>
    /// Helper per gestire valori nullable nei parametri SQL
    /// </summary>
    protected object GetValueOrDBNull(object? value)
    {
        return value ?? DBNull.Value;
    }

    /// <summary>
    /// Helper per leggere valori nullable dal SqlDataReader
    /// </summary>
    protected T? GetNullableValue<T>(SqlDataReader reader, string columnName)
    {
        var ordinal = reader.GetOrdinal(columnName);
        return reader.IsDBNull(ordinal) ? default : reader.GetFieldValue<T>(ordinal);
    }

    /// <summary>
    /// Helper per leggere stringhe nullable dal SqlDataReader
    /// </summary>
    protected string? GetNullableString(SqlDataReader reader, string columnName)
    {
        var ordinal = reader.GetOrdinal(columnName);
        return reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal);
    }

    /// <summary>
    /// Helper per leggere DateTime nullable dal SqlDataReader
    /// </summary>
    protected DateTime? GetNullableDateTime(SqlDataReader reader, string columnName)
    {
        var ordinal = reader.GetOrdinal(columnName);
        return reader.IsDBNull(ordinal) ? null : reader.GetDateTime(ordinal);
    }
}
