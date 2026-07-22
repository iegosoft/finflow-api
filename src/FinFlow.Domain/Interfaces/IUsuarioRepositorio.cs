using FinFlow.Domain.Entidades;

namespace FinFlow.Domain.Interfaces;

/// <summary>
/// Contrato do repositório de usuários.
/// Estende as operações base com consultas específicas do domínio de autenticação.
/// </summary>
public interface IUsuarioRepositorio : IRepositorioBase<Usuario>
{
    /// <summary>
    /// Busca um usuário pelo endereço de e-mail.
    /// Utilizado na autenticação e na validação de e-mail único no cadastro.
    /// </summary>
    /// <param name="email">E-mail a ser pesquisado.</param>
    Task<Usuario?> ObterPorEmailAsync(string email);

    /// <summary>
    /// Busca um usuário pelo valor do refresh token.
    /// Utilizado na renovação de sessão JWT.
    /// </summary>
    /// <param name="refreshToken">Token de renovação a ser pesquisado.</param>
    Task<Usuario?> ObterPorRefreshTokenAsync(string refreshToken);
}
