using FinFlow.Domain.Entidades;
using FinFlow.Domain.Enums;
using FinFlow.Domain.Interfaces;
using FinFlow.Infra.Contexto;
using Microsoft.EntityFrameworkCore;

namespace FinFlow.Infra.Repositorios;

/// <summary>
/// Repositório concreto de transações. Implementa filtros por tipo, categoria
/// e período, além de suporte a paginação e relatório mensal.
/// </summary>
public class TransacaoRepositorio : RepositorioBase<Transacao>, ITransacaoRepositorio
{
    public TransacaoRepositorio(FinFlowDbContext contexto) : base(contexto)
    {
    }

    /// <summary>
    /// Lista transações de um usuário com filtros opcionais e paginação.
    /// Retorna os itens da página solicitada e o total de registros para o frontend calcular a navegação.
    /// </summary>
    public async Task<(IEnumerable<Transacao> Itens, int Total)> ListarAsync(
        Guid usuarioId,
        TipoTransacao? tipo,
        Guid? categoriaId,
        DateTime? dataInicio,
        DateTime? dataFim,
        int pagina,
        int tamanhoPagina)
    {
        // ── Monta a query base filtrada pelo usuário ──
        var query = _dbSet
            .AsNoTracking()
            .Include(t => t.Categoria)
            .Where(t => t.UsuarioId == usuarioId);

        // ── Aplica filtros opcionais ──
        if (tipo.HasValue)
            query = query.Where(t => t.Tipo == tipo.Value);

        if (categoriaId.HasValue)
            query = query.Where(t => t.CategoriaId == categoriaId.Value);

        if (dataInicio.HasValue)
            query = query.Where(t => t.Data >= dataInicio.Value);

        if (dataFim.HasValue)
            query = query.Where(t => t.Data <= dataFim.Value);

        // ── Conta o total antes de paginar ──
        var total = await query.CountAsync();

        // ── Aplica ordenação e paginação ──
        var itens = await query
            .OrderByDescending(t => t.Data)
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .ToListAsync();

        return (itens, total);
    }

    /// <summary>
    /// Busca uma transação pelo ID garantindo que pertença ao usuário informado.
    /// Inclui a categoria relacionada para exibição completa.
    /// </summary>
    public async Task<Transacao?> ObterPorIdEUsuarioAsync(Guid id, Guid usuarioId)
        => await _dbSet
            .Include(t => t.Categoria)
            .FirstOrDefaultAsync(t => t.Id == id && t.UsuarioId == usuarioId);

    /// <summary>
    /// Retorna todas as transações de um usuário em um mês e ano específicos,
    /// incluindo a categoria para agrupamento no relatório.
    /// </summary>
    public async Task<IEnumerable<Transacao>> ListarPorMesAsync(Guid usuarioId, int mes, int ano)
        => await _dbSet
            .AsNoTracking()
            .Include(t => t.Categoria)
            .Where(t => t.UsuarioId == usuarioId
                     && t.Data.Month == mes
                     && t.Data.Year == ano)
            .OrderBy(t => t.Data)
            .ToListAsync();
}
