using FinFlow.Application.DTOs.Categorias;

namespace FinFlow.Application.Interfaces;

/// <summary>
/// Contrato do serviço de categorias.
/// Todas as operações são isoladas por usuário autenticado.
/// </summary>
public interface ICategoriaServico
{
    /// <summary>
    /// Lista todas as categorias do usuário autenticado.
    /// </summary>
    /// <param name="usuarioId">ID do usuário autenticado via JWT.</param>
    Task<IEnumerable<CategoriaRespostaDto>> ListarAsync(Guid usuarioId);

    /// <summary>
    /// Cria uma nova categoria para o usuário autenticado.
    /// </summary>
    /// <param name="requisicao">Dados da categoria a ser criada.</param>
    /// <param name="usuarioId">ID do usuário autenticado via JWT.</param>
    Task<CategoriaRespostaDto> CriarAsync(CriarCategoriaDto requisicao, Guid usuarioId);

    /// <summary>
    /// Atualiza uma categoria existente do usuário autenticado.
    /// Lança exceção se a categoria não existir ou pertencer a outro usuário.
    /// </summary>
    /// <param name="id">ID da categoria a ser atualizada.</param>
    /// <param name="requisicao">Novos dados da categoria.</param>
    /// <param name="usuarioId">ID do usuário autenticado via JWT.</param>
    Task<CategoriaRespostaDto> AtualizarAsync(Guid id, AtualizarCategoriaDto requisicao, Guid usuarioId);

    /// <summary>
    /// Remove uma categoria do usuário autenticado.
    /// Lança exceção se a categoria não existir ou pertencer a outro usuário.
    /// </summary>
    /// <param name="id">ID da categoria a ser removida.</param>
    /// <param name="usuarioId">ID do usuário autenticado via JWT.</param>
    Task RemoverAsync(Guid id, Guid usuarioId);
}
