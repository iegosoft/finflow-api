using FinFlow.Domain.Entidades;
using FinFlow.Domain.Interfaces;
using FinFlow.Infra.Contexto;
using Microsoft.EntityFrameworkCore;

namespace FinFlow.Infra.Repositorios;

/// <summary>
/// Repositório concreto de categorias. Garante que todas as consultas
/// sejam filtradas pelo usuário dono, evitando acesso indevido a dados.
/// </summary>
public class CategoriaRepositorio : RepositorioBase<Categoria>, ICategoriaRepositorio
{
    public CategoriaRepositorio(FinFlowDbContext contexto) : base(contexto)
    {
    }

    /// <summary>
    /// Lista todas as categorias pertencentes a um usuário específico,
    /// ordenadas por nome.
    /// </summary>
    public async Task<IEnumerable<Categoria>> ListarPorUsuarioAsync(Guid usuarioId)
        => await _dbSet
            .AsNoTracking()
            .Where(c => c.UsuarioId == usuarioId)
            .OrderBy(c => c.Nome)
            .ToListAsync();

    /// <summary>
    /// Busca uma categoria pelo ID garantindo que pertença ao usuário informado.
    /// Retorna nulo se não existir ou pertencer a outro usuário.
    /// </summary>
    public async Task<Categoria?> ObterPorIdEUsuarioAsync(Guid id, Guid usuarioId)
        => await _dbSet
            .FirstOrDefaultAsync(c => c.Id == id && c.UsuarioId == usuarioId);
}
