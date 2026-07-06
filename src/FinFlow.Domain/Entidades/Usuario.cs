namespace FinFlow.Domain.Entidades;

/// <summary>
/// Representa um usuário cadastrado na plataforma FinFlow.
/// Contém credenciais de acesso e dados de refresh token para renovação de sessão.
/// </summary>
public class Usuario : EntidadeBase
{
    /// <summary>Nome completo do usuário.</summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>Endereço de e-mail único utilizado para autenticação.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Hash da senha gerado via BCrypt.</summary>
    public string SenhaHash { get; set; } = string.Empty;

    /// <summary>Token de renovação do JWT. Nulo quando não há sessão ativa.</summary>
    public string? RefreshToken { get; set; }

    /// <summary>Data de expiração do refresh token.</summary>
    public DateTime? RefreshTokenExpiracao { get; set; }

    // ── Navegação: categorias e transações pertencentes ao usuário ──
    public ICollection<Categoria> Categorias { get; set; } = [];
    public ICollection<Transacao> Transacoes { get; set; } = [];
}
