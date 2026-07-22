using FinFlow.Domain.Entidades;
using FinFlow.Domain.Interfaces;
using FinFlow.Infra.Contexto;
using Microsoft.EntityFrameworkCore;

namespace FinFlow.Infra.Repositorios;

/// <summary>
/// Repositório concreto de usuários. Implementa consultas específicas
/// de autenticação e gerenciamento de sessão.
/// </summary>
public class UsuarioRepositorio : RepositorioBase<Usuario>, IUsuarioRepositorio
{
    public UsuarioRepositorio(FinFlowDbContext contexto) : base(contexto)
    {
    }

    /// <summary>
    /// Busca um usuário pelo endereço de e-mail.
    /// Utilizado na autenticação e na validação de e-mail único no cadastro.
    /// </summary>
    public async Task<Usuario?> ObterPorEmailAsync(string email)
        => await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

    /// <summary>
    /// Busca um usuário pelo valor do refresh token.
    /// Utilizado na renovação de sessão JWT.
    /// </summary>
    public async Task<Usuario?> ObterPorRefreshTokenAsync(string refreshToken)
        => await _dbSet
            .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
}
