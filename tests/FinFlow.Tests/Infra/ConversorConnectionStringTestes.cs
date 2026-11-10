using FinFlow.Infra.Contexto;
using FluentAssertions;
using Npgsql;

namespace FinFlow.Tests.Infra;

/// <summary>
/// Testes unitários do ConversorConnectionString.
/// Cobre conversão do formato URI (Neon/Render) e preservação do formato chave=valor.
/// </summary>
public class ConversorConnectionStringTestes
{
    [Fact]
    public void Converter_ComFormatoChaveValor_DeveRetornarSemAlteracoes()
    {
        // Arrange
        var original = "Host=localhost;Port=5432;Database=finflow_db;Username=finflow;Password=finflow@2026";

        // Act
        var resultado = ConversorConnectionString.Converter(original);

        // Assert
        resultado.Should().Be(original);
    }

    [Fact]
    public void Converter_ComFormatoUri_DeveExtrairTodosOsDados()
    {
        // Arrange
        var uri = "postgresql://usuario:senha123@ep-exemplo.us-east-2.aws.neon.tech:5433/neondb";

        // Act
        var resultado = new NpgsqlConnectionStringBuilder(ConversorConnectionString.Converter(uri));

        // Assert
        resultado.Host.Should().Be("ep-exemplo.us-east-2.aws.neon.tech");
        resultado.Port.Should().Be(5433);
        resultado.Database.Should().Be("neondb");
        resultado.Username.Should().Be("usuario");
        resultado.Password.Should().Be("senha123");
    }

    [Fact]
    public void Converter_ComUriSemPortaESslmode_DeveUsarPortaPadraoEAplicarSsl()
    {
        // Arrange
        var uri = "postgres://usuario:senha@host.neon.tech/neondb?sslmode=require&channel_binding=require";

        // Act
        var resultado = new NpgsqlConnectionStringBuilder(ConversorConnectionString.Converter(uri));

        // Assert
        resultado.Port.Should().Be(5432);
        resultado.SslMode.Should().Be(SslMode.Require);
    }

    [Fact]
    public void Converter_ComSenhaCodificada_DeveDecodificarCaracteresEspeciais()
    {
        // Arrange
        var uri = "postgresql://usuario:senha%40forte%3A1@host/banco";

        // Act
        var resultado = new NpgsqlConnectionStringBuilder(ConversorConnectionString.Converter(uri));

        // Assert
        resultado.Password.Should().Be("senha@forte:1");
    }
}
