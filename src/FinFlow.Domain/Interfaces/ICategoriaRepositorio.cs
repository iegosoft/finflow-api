using FinFlow.Domain.Entidades;

namespace FinFlow.Domain.Interfaces;

/// <summary>
/// Contrato do repositório de categorias.
/// Garante que as consultas sejam sempre filtradas pelo usuário dono.
/// </summary>
public interface ICategoriaRepositorio : IRepositorioBase<Categoria>
{
    /// <summary>
    /// Lista todas as categorias pertencentes a um usuário específico.
    /// </summary>
    /// <param name="usuarioId">Identificador do usuário autenticado.</param>
    Task<IEnumerable<Categoria>> ListarPorUsuarioAsync(Guid usuarioId);

    /// <summary>
    /// Busca uma categoria pelo ID garantindo que pertença ao usuário informado.
    /// Retorna nulo se a categoria não existir ou pertencer a outro usuário.
    /// </summary>
    /// <param name="id">Identificador da categoria.</param>
    /// <param name="usuarioId">Identificador do usuário autenticado.</param>
    Task<Categoria?> ObterPorIdEUsuarioAsync(Guid id, Guid usuarioId);
}
