using FinFlow.Application.DTOs.Categorias;
using FinFlow.Application.Servicos;
using FinFlow.Domain.Entidades;
using FinFlow.Domain.Enums;
using FinFlow.Domain.Excecoes;
using FinFlow.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace FinFlow.Tests.Servicos;

/// <summary>
/// Testes unitários do CategoriaServico.
/// Cobre listagem, criação, atualização e controle de acesso por usuário.
/// </summary>
public class CategoriaServicoTestes
{
    private readonly Mock<ICategoriaRepositorio> _repositorioMock;
    private readonly CategoriaServico _servico;
    private readonly Guid _usuarioId = Guid.NewGuid();

    public CategoriaServicoTestes()
    {
        _repositorioMock = new Mock<ICategoriaRepositorio>();
        _servico = new CategoriaServico(_repositorioMock.Object);
    }

    [Fact]
    public async Task Listar_DeveRetornarCategoriasDoUsuario()
    {
        // Arrange
        var categorias = new List<Categoria>
        {
            new() { Id = Guid.NewGuid(), Nome = "Salário", Tipo = TipoTransacao.Receita, UsuarioId = _usuarioId },
            new() { Id = Guid.NewGuid(), Nome = "Alimentação", Tipo = TipoTransacao.Saida, UsuarioId = _usuarioId }
        };

        _repositorioMock
            .Setup(r => r.ListarPorUsuarioAsync(_usuarioId))
            .ReturnsAsync(categorias);

        // Act
        var resultado = await _servico.ListarAsync(_usuarioId);

        // Assert
        resultado.Should().HaveCount(2);
        resultado.Should().Contain(c => c.Nome == "Salário");
        resultado.Should().Contain(c => c.Nome == "Alimentação");
    }

    [Fact]
    public async Task Criar_ComDadosValidos_DeveRetornarCategoriaCreada()
    {
        // Arrange
        var requisicao = new CriarCategoriaDto { Nome = "Transporte", Tipo = TipoTransacao.Saida };

        _repositorioMock
            .Setup(r => r.AdicionarAsync(It.IsAny<Categoria>()))
            .Returns(Task.CompletedTask);

        _repositorioMock
            .Setup(r => r.SalvarAlteracoesAsync())
            .ReturnsAsync(1);

        // Act
        var resultado = await _servico.CriarAsync(requisicao, _usuarioId);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Nome.Should().Be("Transporte");
        resultado.Tipo.Should().Be(TipoTransacao.Saida);
        _repositorioMock.Verify(r => r.AdicionarAsync(It.IsAny<Categoria>()), Times.Once);
    }

    [Fact]
    public async Task Atualizar_ComCategoriaInexistente_DeveLancarExcecao()
    {
        // Arrange
        var categoriaId = Guid.NewGuid();

        // ── Simula que a categoria não pertence ao usuário ou não existe ──
        _repositorioMock
            .Setup(r => r.ObterPorIdEUsuarioAsync(categoriaId, _usuarioId))
            .ReturnsAsync((Categoria?)null);

        var requisicao = new AtualizarCategoriaDto { Nome = "Novo Nome", Tipo = TipoTransacao.Receita };

        // Act & Assert
        await _servico.Invoking(s => s.AtualizarAsync(categoriaId, requisicao, _usuarioId))
            .Should().ThrowAsync<RegraDeNegocioException>()
            .WithMessage("*não encontrada*");
    }

    [Fact]
    public async Task Atualizar_ComCategoriaValida_DeveRetornarCategoriaAtualizada()
    {
        // Arrange
        var categoria = new Categoria
        {
            Id = Guid.NewGuid(),
            Nome = "Nome Antigo",
            Tipo = TipoTransacao.Saida,
            UsuarioId = _usuarioId
        };

        _repositorioMock
            .Setup(r => r.ObterPorIdEUsuarioAsync(categoria.Id, _usuarioId))
            .ReturnsAsync(categoria);

        _repositorioMock
            .Setup(r => r.SalvarAlteracoesAsync())
            .ReturnsAsync(1);

        var requisicao = new AtualizarCategoriaDto { Nome = "Nome Novo", Tipo = TipoTransacao.Saida };

        // Act
        var resultado = await _servico.AtualizarAsync(categoria.Id, requisicao, _usuarioId);

        // Assert
        resultado.Nome.Should().Be("Nome Novo");
        _repositorioMock.Verify(r => r.Atualizar(It.IsAny<Categoria>()), Times.Once);
    }

    [Fact]
    public async Task Remover_ComCategoriaInexistente_DeveLancarExcecao()
    {
        // Arrange
        var categoriaId = Guid.NewGuid();

        _repositorioMock
            .Setup(r => r.ObterPorIdEUsuarioAsync(categoriaId, _usuarioId))
            .ReturnsAsync((Categoria?)null);

        // Act & Assert
        await _servico.Invoking(s => s.RemoverAsync(categoriaId, _usuarioId))
            .Should().ThrowAsync<RegraDeNegocioException>()
            .WithMessage("*não encontrada*");
    }

    [Fact]
    public async Task Remover_ComCategoriaValida_DeveRemoverEPersistir()
    {
        // Arrange
        var categoria = new Categoria
        {
            Id = Guid.NewGuid(),
            Nome = "A Remover",
            Tipo = TipoTransacao.Saida,
            UsuarioId = _usuarioId
        };

        _repositorioMock
            .Setup(r => r.ObterPorIdEUsuarioAsync(categoria.Id, _usuarioId))
            .ReturnsAsync(categoria);

        _repositorioMock
            .Setup(r => r.SalvarAlteracoesAsync())
            .ReturnsAsync(1);

        // Act
        await _servico.RemoverAsync(categoria.Id, _usuarioId);

        // Assert
        _repositorioMock.Verify(r => r.Remover(It.IsAny<Categoria>()), Times.Once);
        _repositorioMock.Verify(r => r.SalvarAlteracoesAsync(), Times.Once);
    }
}
