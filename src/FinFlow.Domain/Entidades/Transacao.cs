using FinFlow.Domain.Enums;

namespace FinFlow.Domain.Entidades;

/// <summary>
/// Representa uma transação financeira registrada pelo usuário.
/// Pode ser uma receita (entrada) ou uma saída (despesa), sempre vinculada a uma categoria.
/// </summary>
public class Transacao : EntidadeBase
{
    /// <summary>Descrição da transação (ex: "Aluguel de agosto").</summary>
    public string Descricao { get; set; } = string.Empty;

    /// <summary>Valor monetário da transação. Sempre positivo — o tipo define se é entrada ou saída.</summary>
    public decimal Valor { get; set; }

    /// <summary>Tipo da transação: Receita ou Saida.</summary>
    public TipoTransacao Tipo { get; set; }

    /// <summary>Data em que a transação ocorreu.</summary>
    public DateTime Data { get; set; }

    /// <summary>Identificador da categoria à qual esta transação pertence.</summary>
    public Guid CategoriaId { get; set; }

    /// <summary>Identificador do usuário dono desta transação.</summary>
    public Guid UsuarioId { get; set; }

    // ── Navegação: categoria e usuário relacionados ──
    public Categoria Categoria { get; set; } = null!;
    public Usuario Usuario { get; set; } = null!;
}
