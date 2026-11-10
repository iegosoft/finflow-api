using Npgsql;

namespace FinFlow.Infra.Contexto;

/// <summary>
/// Normaliza connection strings do PostgreSQL para o formato aceito pelo Npgsql.
/// Provedores em nuvem como Neon e Render fornecem a conexão no formato URI
/// (postgresql://usuario:senha@host:porta/banco), que o Npgsql não lê diretamente.
/// </summary>
public static class ConversorConnectionString
{
    /// <summary>
    /// Converte uma connection string no formato URI para o formato chave=valor.
    /// Se a string já estiver no formato chave=valor, retorna sem alterações.
    /// </summary>
    /// <param name="connectionString">Connection string no formato URI ou chave=valor.</param>
    public static string Converter(string connectionString)
    {
        // ── Mantém o formato chave=valor usado no ambiente local ──
        if (!connectionString.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase) &&
            !connectionString.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase))
        {
            return connectionString;
        }

        var uri = new Uri(connectionString);
        var credenciais = uri.UserInfo.Split(':', 2);

        var construtor = new NpgsqlConnectionStringBuilder
        {
            Host = uri.Host,
            Port = uri.Port > 0 ? uri.Port : 5432,
            Database = uri.AbsolutePath.TrimStart('/'),
            Username = Uri.UnescapeDataString(credenciais[0]),
            Password = credenciais.Length > 1 ? Uri.UnescapeDataString(credenciais[1]) : null
        };

        // ── Aplica o modo SSL informado na query string (ex.: ?sslmode=require) ──
        var parametros = uri.Query.TrimStart('?')
            .Split('&', StringSplitOptions.RemoveEmptyEntries)
            .Select(parametro => parametro.Split('=', 2))
            .Where(partes => partes.Length == 2)
            .ToDictionary(partes => partes[0], partes => partes[1], StringComparer.OrdinalIgnoreCase);

        if (parametros.TryGetValue("sslmode", out var modoSsl) &&
            Enum.TryParse<SslMode>(modoSsl.Replace("-", string.Empty), ignoreCase: true, out var modoConvertido))
        {
            construtor.SslMode = modoConvertido;
        }

        return construtor.ConnectionString;
    }
}
