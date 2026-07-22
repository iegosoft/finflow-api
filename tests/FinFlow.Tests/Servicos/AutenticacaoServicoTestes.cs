using FinFlow.Application.DTOs.Autenticacao;
using FinFlow.Application.Servicos;
using FinFlow.Domain.Entidades;
using FinFlow.Domain.Excecoes;
using FinFlow.Domain.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;

namespace FinFlow.Tests.Servicos;

/// <summary>
/// Testes unitários do AutenticacaoServico.
/// Cobre os cenários de registro, login e renovação de token.
/// </summary>
public class AutenticacaoServicoTestes
{
    private readonly Mock<IUsuarioRepositorio> _usuarioRepositorioMock;
    private readonly IConfiguration _configuracao;
    private readonly AutenticacaoServico _servico;

    public AutenticacaoServicoTestes()
    {
        _usuarioRepositorioMock = new Mock<IUsuarioRepositorio>();

        // ── Configuração mínima para geração de JWT nos testes ──
        var configValues = new Dictionary<string, string?>
        {
            ["Jwt:Chave"] = "CHAVE_SECRETA_PARA_TESTES_UNITARIOS_32CHARS",
            ["Jwt:Emissor"] = "FinFlowAPI",
            ["Jwt:Audiencia"] = "FinFlowCliente",
            ["Jwt:ExpiracaoHoras"] = "1",
            ["Jwt:ExpiracaoRefreshTokenDias"] = "7"
        };

        _configuracao = new ConfigurationBuilder()
            .AddInMemoryCollection(configValues)
            .Build();

        _servico = new AutenticacaoServico(_usuarioRepositorioMock.Object, _configuracao);
    }

    [Fact]
    public async Task Registrar_ComDadosValidos_DeveRetornarToken()
    {
        // Arrange
        var requisicao = new RegistrarDto
        {
            Nome = "Iego Costa",
            Email = "iego@email.com",
            Senha = "senha123"
        };

        _usuarioRepositorioMock
            .Setup(r => r.ObterPorEmailAsync(requisicao.Email))
            .ReturnsAsync((Usuario?)null);

        _usuarioRepositorioMock
            .Setup(r => r.AdicionarAsync(It.IsAny<Usuario>()))
            .Returns(Task.CompletedTask);

        _usuarioRepositorioMock
            .Setup(r => r.SalvarAlteracoesAsync())
            .ReturnsAsync(1);

        // Act
        var resultado = await _servico.RegistrarAsync(requisicao);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Token.Should().NotBeNullOrEmpty();
        resultado.RefreshToken.Should().NotBeNullOrEmpty();
        resultado.Expiracao.Should().BeAfter(DateTime.UtcNow);
    }

    [Fact]
    public async Task Registrar_ComEmailJaCadastrado_DeveLancarExcecao()
    {
        // Arrange
        var requisicao = new RegistrarDto
        {
            Nome = "Iego Costa",
            Email = "iego@email.com",
            Senha = "senha123"
        };

        // ── Simula que o e-mail já existe no banco ──
        _usuarioRepositorioMock
            .Setup(r => r.ObterPorEmailAsync(requisicao.Email))
            .ReturnsAsync(new Usuario { Email = requisicao.Email });

        // Act & Assert
        await _servico.Invoking(s => s.RegistrarAsync(requisicao))
            .Should().ThrowAsync<RegraDeNegocioException>()
            .WithMessage("*e-mail*");
    }

    [Fact]
    public async Task Login_ComCredenciaisValidas_DeveRetornarToken()
    {
        // Arrange
        var senhaHash = BCrypt.Net.BCrypt.HashPassword("senha123");
        var usuario = new Usuario
        {
            Id = Guid.NewGuid(),
            Nome = "Iego Costa",
            Email = "iego@email.com",
            SenhaHash = senhaHash
        };

        _usuarioRepositorioMock
            .Setup(r => r.ObterPorEmailAsync(usuario.Email))
            .ReturnsAsync(usuario);

        _usuarioRepositorioMock
            .Setup(r => r.SalvarAlteracoesAsync())
            .ReturnsAsync(1);

        var requisicao = new LoginDto { Email = usuario.Email, Senha = "senha123" };

        // Act
        var resultado = await _servico.LoginAsync(requisicao);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Token.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Login_ComEmailInexistente_DeveLancarExcecao()
    {
        // Arrange
        _usuarioRepositorioMock
            .Setup(r => r.ObterPorEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((Usuario?)null);

        var requisicao = new LoginDto { Email = "inexistente@email.com", Senha = "senha123" };

        // Act & Assert
        await _servico.Invoking(s => s.LoginAsync(requisicao))
            .Should().ThrowAsync<RegraDeNegocioException>()
            .WithMessage("*inválidos*");
    }

    [Fact]
    public async Task Login_ComSenhaErrada_DeveLancarExcecao()
    {
        // Arrange
        var usuario = new Usuario
        {
            Email = "iego@email.com",
            SenhaHash = BCrypt.Net.BCrypt.HashPassword("senhaCorreta")
        };

        _usuarioRepositorioMock
            .Setup(r => r.ObterPorEmailAsync(usuario.Email))
            .ReturnsAsync(usuario);

        var requisicao = new LoginDto { Email = usuario.Email, Senha = "senhaErrada" };

        // Act & Assert
        await _servico.Invoking(s => s.LoginAsync(requisicao))
            .Should().ThrowAsync<RegraDeNegocioException>()
            .WithMessage("*inválidos*");
    }

    [Fact]
    public async Task RenovarToken_ComRefreshTokenExpirado_DeveLancarExcecao()
    {
        // Arrange
        var usuario = new Usuario
        {
            RefreshToken = "token-expirado",
            RefreshTokenExpiracao = DateTime.UtcNow.AddDays(-1)
        };

        _usuarioRepositorioMock
            .Setup(r => r.ObterPorRefreshTokenAsync("token-expirado"))
            .ReturnsAsync(usuario);

        var requisicao = new RefreshTokenDto { RefreshToken = "token-expirado" };

        // Act & Assert
        await _servico.Invoking(s => s.RenovarTokenAsync(requisicao))
            .Should().ThrowAsync<RegraDeNegocioException>()
            .WithMessage("*expirado*");
    }

    [Fact]
    public async Task RenovarToken_ComRefreshTokenInvalido_DeveLancarExcecao()
    {
        // Arrange
        _usuarioRepositorioMock
            .Setup(r => r.ObterPorRefreshTokenAsync(It.IsAny<string>()))
            .ReturnsAsync((Usuario?)null);

        var requisicao = new RefreshTokenDto { RefreshToken = "token-invalido" };

        // Act & Assert
        await _servico.Invoking(s => s.RenovarTokenAsync(requisicao))
            .Should().ThrowAsync<RegraDeNegocioException>()
            .WithMessage("*inválido*");
    }
}
