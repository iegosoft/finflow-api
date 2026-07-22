namespace FinFlow.Application.DTOs.Autenticacao;

/// <summary>
/// Resposta retornada após autenticação ou renovação de token.
/// Contém o JWT de acesso e o refresh token para renovação de sessão.
/// </summary>
public class TokenRespostaDto
{
    /// <summary>Token JWT para autenticação nas rotas protegidas.</summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>Token de renovação com validade maior, usado para obter um novo JWT.</summary>
    public string RefreshToken { get; set; } = string.Empty;

    /// <summary>Data e hora de expiração do JWT.</summary>
    public DateTime Expiracao { get; set; }
}
