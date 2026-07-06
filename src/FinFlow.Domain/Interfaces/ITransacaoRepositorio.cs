using FinFlow.Domain.Entidades;
using FinFlow.Domain.Enums;

namespace FinFlow.Domain.Interfaces;

/// <summary>
/// Contrato do repositório de transações.
/// Define consultas com suporte a filtros de tipo, categoria e período.
/// </summary>
public interface ITransacaoRepositorio : IRepositorioBase<Transacao>
{
    /// <summary>
    /// Lista transações de um usuário com filtros opcionais.
    /// </summary>
    /// <param name="usuarioId">Identificador do usuário autenticado.</param>
    /// <param name="tipo">Filtra por tipo (Receita ou Saida). Nulo retorna todos.</param>
    /// <param name="categoriaId">Filtra por categoria. Nulo retorna todas.</param>
    /// <param name="dataInicio">Data inicial do período. Nulo ignora o filtro.</param>
    /// <param name="dataFim">Data final do período. Nulo ignora o filtro.</param>
    /// <param name="pagina">Número da página para paginação (começa em 1).</param>
    /// <param name="tamanhoPagina">Quantidade de registros por página.</param>
    Task<(IEnumerable<Transacao> Itens, int Total)> ListarAsync(
        Guid usuarioId,
        TipoTransacao? tipo,
        Guid? categoriaId,
        DateTime? dataInicio,
        DateTime? dataFim,
        int pagina,
        int tamanhoPagina);

    /// <summary>
    /// Busca uma transação pelo ID garantindo que pertença ao usuário informado.
    /// </summary>
    /// <param name="id">Identificador da transação.</param>
    /// <param name="usuarioId">Identificador do usuário autenticado.</param>
    Task<Transacao?> ObterPorIdEUsuarioAsync(Guid id, Guid usuarioId);

    /// <summary>
    /// Retorna todas as transações de um usuário em um mês e ano específicos.
    /// Utilizado para geração do relatório mensal.
    /// </summary>
    /// <param name="usuarioId">Identificador do usuário autenticado.</param>
    /// <param name="mes">Mês do relatório (1-12).</param>
    /// <param name="ano">Ano do relatório.</param>
    Task<IEnumerable<Transacao>> ListarPorMesAsync(Guid usuarioId, int mes, int ano);
}
