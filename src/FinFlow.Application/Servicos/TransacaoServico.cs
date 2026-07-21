using FinFlow.Application.DTOs.Transacoes;
using FinFlow.Application.Interfaces;
using FinFlow.Domain.Entidades;
using FinFlow.Domain.Enums;
using FinFlow.Domain.Excecoes;
using FinFlow.Domain.Interfaces;

namespace FinFlow.Application.Servicos;

/// <summary>
/// Serviço responsável pelo CRUD de transações financeiras.
/// Garante isolamento por usuário, valida categorias e gera relatório mensal.
/// </summary>
public class TransacaoServico : ITransacaoServico
{
    private readonly ITransacaoRepositorio _transacaoRepositorio;
    private readonly ICategoriaRepositorio _categoriaRepositorio;

    public TransacaoServico(ITransacaoRepositorio transacaoRepositorio, ICategoriaRepositorio categoriaRepositorio)
    {
        _transacaoRepositorio = transacaoRepositorio;
        _categoriaRepositorio = categoriaRepositorio;
    }

    /// <summary>
    /// Lista transações do usuário com filtros opcionais e paginação.
    /// </summary>
    public async Task<PaginadoDto<TransacaoRespostaDto>> ListarAsync(
        Guid usuarioId, TipoTransacao? tipo, Guid? categoriaId,
        DateTime? dataInicio, DateTime? dataFim, int pagina, int tamanhoPagina)
    {
        var (itens, total) = await _transacaoRepositorio.ListarAsync(
            usuarioId, tipo, categoriaId, dataInicio, dataFim, pagina, tamanhoPagina);

        return new PaginadoDto<TransacaoRespostaDto>
        {
            Itens = itens.Select(MapearParaDto),
            Total = total,
            Pagina = pagina,
            TamanhoPagina = tamanhoPagina
        };
    }

    /// <summary>
    /// Busca uma transação pelo ID garantindo que pertença ao usuário autenticado.
    /// </summary>
    public async Task<TransacaoRespostaDto> ObterPorIdAsync(Guid id, Guid usuarioId)
    {
        var transacao = await _transacaoRepositorio.ObterPorIdEUsuarioAsync(id, usuarioId)
            ?? throw new RegraDeNegocioException("Transação não encontrada.");

        return MapearParaDto(transacao);
    }

    /// <summary>
    /// Cria uma nova transação para o usuário autenticado.
    /// Valida se a categoria informada pertence ao usuário antes de persistir.
    /// </summary>
    public async Task<TransacaoRespostaDto> CriarAsync(CriarTransacaoDto requisicao, Guid usuarioId)
    {
        // ── Valida se a categoria pertence ao usuário autenticado ──
        var categoria = await _categoriaRepositorio.ObterPorIdEUsuarioAsync(requisicao.CategoriaId, usuarioId)
            ?? throw new RegraDeNegocioException("Categoria não encontrada ou não pertence ao usuário.");

        // ── Valida se o tipo da transação é compatível com o tipo da categoria ──
        if (categoria.Tipo != requisicao.Tipo)
            throw new RegraDeNegocioException(
                $"O tipo da transação ({requisicao.Tipo}) deve ser compatível com o tipo da categoria ({categoria.Tipo}).");

        var transacao = new Transacao
        {
            Descricao = requisicao.Descricao.Trim(),
            Valor = requisicao.Valor,
            Tipo = requisicao.Tipo,
            Data = requisicao.Data.ToUniversalTime(),
            CategoriaId = requisicao.CategoriaId,
            UsuarioId = usuarioId
        };

        await _transacaoRepositorio.AdicionarAsync(transacao);
        await _transacaoRepositorio.SalvarAlteracoesAsync();

        // ── Recarrega com a categoria para popular o DTO de resposta ──
        var criada = await _transacaoRepositorio.ObterPorIdEUsuarioAsync(transacao.Id, usuarioId);
        return MapearParaDto(criada!);
    }

    /// <summary>
    /// Atualiza uma transação existente do usuário autenticado.
    /// </summary>
    public async Task<TransacaoRespostaDto> AtualizarAsync(Guid id, AtualizarTransacaoDto requisicao, Guid usuarioId)
    {
        // ── Garante que a transação pertence ao usuário ──
        var transacao = await _transacaoRepositorio.ObterPorIdEUsuarioAsync(id, usuarioId)
            ?? throw new RegraDeNegocioException("Transação não encontrada.");

        // ── Valida a nova categoria se foi alterada ──
        if (transacao.CategoriaId != requisicao.CategoriaId)
        {
            var categoria = await _categoriaRepositorio.ObterPorIdEUsuarioAsync(requisicao.CategoriaId, usuarioId)
                ?? throw new RegraDeNegocioException("Categoria não encontrada ou não pertence ao usuário.");

            if (categoria.Tipo != requisicao.Tipo)
                throw new RegraDeNegocioException(
                    $"O tipo da transação ({requisicao.Tipo}) deve ser compatível com o tipo da categoria ({categoria.Tipo}).");
        }

        transacao.Descricao = requisicao.Descricao.Trim();
        transacao.Valor = requisicao.Valor;
        transacao.Tipo = requisicao.Tipo;
        transacao.Data = requisicao.Data.ToUniversalTime();
        transacao.CategoriaId = requisicao.CategoriaId;

        _transacaoRepositorio.Atualizar(transacao);
        await _transacaoRepositorio.SalvarAlteracoesAsync();

        var atualizada = await _transacaoRepositorio.ObterPorIdEUsuarioAsync(id, usuarioId);
        return MapearParaDto(atualizada!);
    }

    /// <summary>
    /// Remove uma transação do usuário autenticado.
    /// </summary>
    public async Task RemoverAsync(Guid id, Guid usuarioId)
    {
        var transacao = await _transacaoRepositorio.ObterPorIdEUsuarioAsync(id, usuarioId)
            ?? throw new RegraDeNegocioException("Transação não encontrada.");

        _transacaoRepositorio.Remover(transacao);
        await _transacaoRepositorio.SalvarAlteracoesAsync();
    }

    /// <summary>
    /// Gera o relatório mensal com totais por categoria e saldo do mês.
    /// </summary>
    public async Task<RelatorioMensalDto> ObterRelatorioMensalAsync(Guid usuarioId, int mes, int ano)
    {
        var transacoes = await _transacaoRepositorio.ListarPorMesAsync(usuarioId, mes, ano);

        // ── Agrupa transações por categoria e calcula totais ──
        var porCategoria = transacoes
            .GroupBy(t => new { t.Categoria.Nome, Tipo = t.Categoria.Tipo.ToString() })
            .Select(g => new RelatorioCategoriasDto
            {
                CategoriaNome = g.Key.Nome,
                Tipo = g.Key.Tipo,
                Total = g.Sum(t => t.Valor),
                Quantidade = g.Count()
            })
            .OrderByDescending(c => c.Total)
            .ToList();

        return new RelatorioMensalDto
        {
            Mes = mes,
            Ano = ano,
            TotalReceitas = transacoes.Where(t => t.Tipo == TipoTransacao.Receita).Sum(t => t.Valor),
            TotalSaidas = transacoes.Where(t => t.Tipo == TipoTransacao.Saida).Sum(t => t.Valor),
            Categorias = porCategoria
        };
    }

    // ── Mapeia a entidade Transacao para o DTO de resposta ──
    private static TransacaoRespostaDto MapearParaDto(Transacao t) => new()
    {
        Id = t.Id,
        Descricao = t.Descricao,
        Valor = t.Valor,
        Tipo = t.Tipo,
        Data = t.Data,
        CategoriaId = t.CategoriaId,
        CategoriaNome = t.Categoria?.Nome ?? string.Empty,
        CriadoEm = t.CriadoEm
    };
}
