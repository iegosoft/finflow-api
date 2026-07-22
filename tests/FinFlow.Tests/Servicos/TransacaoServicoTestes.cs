using FinFlow.Application.DTOs.Transacoes;
using FinFlow.Application.Servicos;
using FinFlow.Domain.Entidades;
using FinFlow.Domain.Enums;
using FinFlow.Domain.Excecoes;
using FinFlow.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace FinFlow.Tests.Servicos;

/// <summary>
/// Testes unitários do TransacaoServico.
/// Cobre criação com validação de categoria, relatório mensal e cenários de erro.
/// </summary>
public class TransacaoServicoTestes
{
    private readonly Mock<ITransacaoRepositorio> _transacaoRepositorioMock;
    private readonly Mock<ICategoriaRepositorio> _categoriaRepositorioMock;
    private readonly TransacaoServico _servico;
    private readonly Guid _usuarioId = Guid.NewGuid();

    public TransacaoServicoTestes()
    {
        _transacaoRepositorioMock = new Mock<ITransacaoRepositorio>();
        _categoriaRepositorioMock = new Mock<ICategoriaRepositorio>();
        _servico = new TransacaoServico(_transacaoRepositorioMock.Object, _categoriaRepositorioMock.Object);
    }

    [Fact]
    public async Task Criar_ComCategoriaValida_DeveRetornarTransacaoCriada()
    {
        // Arrange
        var categoriaId = Guid.NewGuid();
        var categoria = new Categoria
        {
            Id = categoriaId,
            Nome = "Alimentação",
            Tipo = TipoTransacao.Saida,
            UsuarioId = _usuarioId
        };

        var requisicao = new CriarTransacaoDto
        {
            Descricao = "Supermercado",
            Valor = 350.00m,
            Tipo = TipoTransacao.Saida,
            Data = DateTime.UtcNow,
            CategoriaId = categoriaId
        };

        _categoriaRepositorioMock
            .Setup(r => r.ObterPorIdEUsuarioAsync(categoriaId, _usuarioId))
            .ReturnsAsync(categoria);

        _transacaoRepositorioMock
            .Setup(r => r.AdicionarAsync(It.IsAny<Transacao>()))
            .Returns(Task.CompletedTask);

        _transacaoRepositorioMock
            .Setup(r => r.SalvarAlteracoesAsync())
            .ReturnsAsync(1);

        // ── Simula o retorno após persistência com a categoria carregada ──
        _transacaoRepositorioMock
            .Setup(r => r.ObterPorIdEUsuarioAsync(It.IsAny<Guid>(), _usuarioId))
            .ReturnsAsync(new Transacao
            {
                Id = Guid.NewGuid(),
                Descricao = requisicao.Descricao,
                Valor = requisicao.Valor,
                Tipo = requisicao.Tipo,
                Data = requisicao.Data,
                CategoriaId = categoriaId,
                UsuarioId = _usuarioId,
                Categoria = categoria
            });

        // Act
        var resultado = await _servico.CriarAsync(requisicao, _usuarioId);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Descricao.Should().Be("Supermercado");
        resultado.Valor.Should().Be(350.00m);
        resultado.CategoriaNome.Should().Be("Alimentação");
    }

    [Fact]
    public async Task Criar_ComCategoriaInexistente_DeveLancarExcecao()
    {
        // Arrange
        _categoriaRepositorioMock
            .Setup(r => r.ObterPorIdEUsuarioAsync(It.IsAny<Guid>(), _usuarioId))
            .ReturnsAsync((Categoria?)null);

        var requisicao = new CriarTransacaoDto
        {
            Descricao = "Salário",
            Valor = 5000m,
            Tipo = TipoTransacao.Receita,
            Data = DateTime.UtcNow,
            CategoriaId = Guid.NewGuid()
        };

        // Act & Assert
        await _servico.Invoking(s => s.CriarAsync(requisicao, _usuarioId))
            .Should().ThrowAsync<RegraDeNegocioException>()
            .WithMessage("*Categoria*");
    }

    [Fact]
    public async Task Criar_ComTipoIncompativelComCategoria_DeveLancarExcecao()
    {
        // Arrange — categoria é Receita mas transação é Saida
        var categoriaId = Guid.NewGuid();
        var categoria = new Categoria
        {
            Id = categoriaId,
            Tipo = TipoTransacao.Receita,
            UsuarioId = _usuarioId
        };

        _categoriaRepositorioMock
            .Setup(r => r.ObterPorIdEUsuarioAsync(categoriaId, _usuarioId))
            .ReturnsAsync(categoria);

        var requisicao = new CriarTransacaoDto
        {
            Descricao = "Conta de luz",
            Valor = 200m,
            Tipo = TipoTransacao.Saida,
            Data = DateTime.UtcNow,
            CategoriaId = categoriaId
        };

        // Act & Assert
        await _servico.Invoking(s => s.CriarAsync(requisicao, _usuarioId))
            .Should().ThrowAsync<RegraDeNegocioException>()
            .WithMessage("*compatível*");
    }

    [Fact]
    public async Task ObterPorId_ComTransacaoInexistente_DeveLancarExcecao()
    {
        // Arrange
        _transacaoRepositorioMock
            .Setup(r => r.ObterPorIdEUsuarioAsync(It.IsAny<Guid>(), _usuarioId))
            .ReturnsAsync((Transacao?)null);

        // Act & Assert
        await _servico.Invoking(s => s.ObterPorIdAsync(Guid.NewGuid(), _usuarioId))
            .Should().ThrowAsync<RegraDeNegocioException>()
            .WithMessage("*não encontrada*");
    }

    [Fact]
    public async Task ObterRelatorioMensal_DeveCalcularTotaisCorretamente()
    {
        // Arrange
        var categoriaReceita = new Categoria { Nome = "Salário", Tipo = TipoTransacao.Receita };
        var categoriaSaida = new Categoria { Nome = "Alimentação", Tipo = TipoTransacao.Saida };

        var transacoes = new List<Transacao>
        {
            new() { Valor = 5000m, Tipo = TipoTransacao.Receita, Categoria = categoriaReceita, Data = DateTime.UtcNow },
            new() { Valor = 300m,  Tipo = TipoTransacao.Saida,   Categoria = categoriaSaida,   Data = DateTime.UtcNow },
            new() { Valor = 150m,  Tipo = TipoTransacao.Saida,   Categoria = categoriaSaida,   Data = DateTime.UtcNow }
        };

        _transacaoRepositorioMock
            .Setup(r => r.ListarPorMesAsync(_usuarioId, 7, 2026))
            .ReturnsAsync(transacoes);

        // Act
        var relatorio = await _servico.ObterRelatorioMensalAsync(_usuarioId, 7, 2026);

        // Assert
        relatorio.TotalReceitas.Should().Be(5000m);
        relatorio.TotalSaidas.Should().Be(450m);
        relatorio.Saldo.Should().Be(4550m);
        relatorio.Categorias.Should().HaveCount(2);
    }

    [Fact]
    public async Task Remover_ComTransacaoInexistente_DeveLancarExcecao()
    {
        // Arrange
        _transacaoRepositorioMock
            .Setup(r => r.ObterPorIdEUsuarioAsync(It.IsAny<Guid>(), _usuarioId))
            .ReturnsAsync((Transacao?)null);

        // Act & Assert
        await _servico.Invoking(s => s.RemoverAsync(Guid.NewGuid(), _usuarioId))
            .Should().ThrowAsync<RegraDeNegocioException>()
            .WithMessage("*não encontrada*");
    }
}
