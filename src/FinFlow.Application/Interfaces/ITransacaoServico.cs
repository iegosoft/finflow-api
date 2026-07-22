using FinFlow.Application.DTOs.Transacoes;
using FinFlow.Domain.Enums;

namespace FinFlow.Application.Interfaces;

/// <summary>
/// Contrato do serviço de transações.
/// Todas as operações são isoladas por usuário autenticado.
/// </summary>
public interface ITransacaoServico
{
    /// <summary>
    /// Lista transações do usuário com filtros opcionais e paginação.
    /// </summary>
    Task<PaginadoDto<TransacaoRespostaDto>> ListarAsync(
        Guid usuarioId,
        TipoTransacao? tipo,
        Guid? categoriaId,
        DateTime? dataInicio,
        DateTime? dataFim,
        int pagina,
        int tamanhoPagina);

    /// <summary>
    /// Busca uma transação pelo ID garantindo que pertença ao usuário autenticado.
    /// </summary>
    Task<TransacaoRespostaDto> ObterPorIdAsync(Guid id, Guid usuarioId);

    /// <summary>
    /// Cria uma nova transação para o usuário autenticado.
    /// Valida se a categoria informada pertence ao usuário antes de persistir.
    /// </summary>
    Task<TransacaoRespostaDto> CriarAsync(CriarTransacaoDto requisicao, Guid usuarioId);

    /// <summary>
    /// Atualiza uma transação existente do usuário autenticado.
    /// </summary>
    Task<TransacaoRespostaDto> AtualizarAsync(Guid id, AtualizarTransacaoDto requisicao, Guid usuarioId);

    /// <summary>
    /// Remove uma transação do usuário autenticado.
    /// </summary>
    Task RemoverAsync(Guid id, Guid usuarioId);

    /// <summary>
    /// Gera o relatório mensal com totais por categoria e saldo do mês.
    /// </summary>
    Task<RelatorioMensalDto> ObterRelatorioMensalAsync(Guid usuarioId, int mes, int ano);
}
