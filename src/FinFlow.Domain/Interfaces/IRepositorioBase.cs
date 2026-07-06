using FinFlow.Domain.Entidades;

namespace FinFlow.Domain.Interfaces;

/// <summary>
/// Contrato base para repositórios. Define as operações comuns de persistência
/// compartilhadas entre todos os repositórios do domínio.
/// </summary>
/// <typeparam name="T">Tipo da entidade, deve herdar de EntidadeBase.</typeparam>
public interface IRepositorioBase<T> where T : EntidadeBase
{
    /// <summary>Busca uma entidade pelo seu identificador único.</summary>
    Task<T?> ObterPorIdAsync(Guid id);

    /// <summary>Persiste uma nova entidade no banco de dados.</summary>
    Task AdicionarAsync(T entidade);

    /// <summary>Atualiza os dados de uma entidade existente.</summary>
    void Atualizar(T entidade);

    /// <summary>Remove uma entidade do banco de dados.</summary>
    void Remover(T entidade);

    /// <summary>Confirma todas as operações pendentes no banco de dados.</summary>
    Task<int> SalvarAlteracoesAsync();
}
