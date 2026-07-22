using FinFlow.Domain.Enums;

namespace FinFlow.Domain.Entidades;

/// <summary>
/// Representa uma categoria financeira criada pelo usuário.
/// Categorias são usadas para classificar transações por tipo (receita ou saída).
/// </summary>
public class Categoria : EntidadeBase
{
    /// <summary>Nome descritivo da categoria (ex: Alimentação, Salário).</summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>Tipo da categoria: Receita ou Saida.</summary>
    public TipoTransacao Tipo { get; set; }

    /// <summary>Identificador do usuário dono desta categoria.</summary>
    public Guid UsuarioId { get; set; }

    // ── Navegação: usuário dono e transações vinculadas ──
    public Usuario Usuario { get; set; } = null!;
    public ICollection<Transacao> Transacoes { get; set; } = [];
}
