using FinFlow.Domain.Entidades;
using FinFlow.Domain.Interfaces;
using FinFlow.Infra.Contexto;
using Microsoft.EntityFrameworkCore;

namespace FinFlow.Infra.Repositorios;

/// <summary>
/// Implementação base dos repositórios. Encapsula as operações comuns de
/// persistência usando o Entity Framework Core.
/// </summary>
/// <typeparam name="T">Tipo da entidade, deve herdar de EntidadeBase.</typeparam>
public class RepositorioBase<T> : IRepositorioBase<T> where T : EntidadeBase
{
    protected readonly FinFlowDbContext _contexto;
    protected readonly DbSet<T> _dbSet;

    public RepositorioBase(FinFlowDbContext contexto)
    {
        _contexto = contexto;
        _dbSet = contexto.Set<T>();
    }

    /// <summary>Busca uma entidade pelo seu identificador único.</summary>
    public async Task<T?> ObterPorIdAsync(Guid id)
        => await _dbSet.FindAsync(id);

    /// <summary>Persiste uma nova entidade no banco de dados.</summary>
    public async Task AdicionarAsync(T entidade)
        => await _dbSet.AddAsync(entidade);

    /// <summary>Atualiza os dados de uma entidade existente.</summary>
    public void Atualizar(T entidade)
        => _dbSet.Update(entidade);

    /// <summary>Remove uma entidade do banco de dados.</summary>
    public void Remover(T entidade)
        => _dbSet.Remove(entidade);

    /// <summary>Confirma todas as operações pendentes no banco de dados.</summary>
    public async Task<int> SalvarAlteracoesAsync()
        => await _contexto.SaveChangesAsync();
}
