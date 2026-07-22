using FinFlow.Application.DTOs.Autenticacao;

namespace FinFlow.Application.Interfaces;

/// <summary>
/// Contrato do serviço de autenticação.
/// Define as operações de registro, login e renovação de sessão.
/// </summary>
public interface IAutenticacaoServico
{
    /// <summary>
    /// Registra um novo usuário na plataforma.
    /// Valida se o e-mail já existe e aplica hash na senha antes de persistir.
    /// </summary>
    /// <param name="requisicao">Dados do novo usuário.</param>
    /// <returns>Token JWT e refresh token para a sessão criada.</returns>
    Task<TokenRespostaDto> RegistrarAsync(RegistrarDto requisicao);

    /// <summary>
    /// Autentica um usuário com e-mail e senha.
    /// Valida o hash da senha e retorna os tokens de sessão.
    /// </summary>
    /// <param name="requisicao">Credenciais do usuário.</param>
    /// <returns>Token JWT e refresh token.</returns>
    Task<TokenRespostaDto> LoginAsync(LoginDto requisicao);

    /// <summary>
    /// Renova o JWT usando um refresh token válido e não expirado.
    /// Invalida o refresh token anterior e emite um novo par de tokens.
    /// </summary>
    /// <param name="requisicao">Refresh token atual do usuário.</param>
    /// <returns>Novo token JWT e novo refresh token.</returns>
    Task<TokenRespostaDto> RenovarTokenAsync(RefreshTokenDto requisicao);
}
