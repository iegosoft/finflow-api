namespace FinFlow.Application.DTOs.Autenticacao;

/// <summary>
/// Credenciais enviadas pelo usuário para autenticação.
/// </summary>
public class LoginDto
{
    /// <summary>E-mail cadastrado na plataforma.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Senha do usuário.</summary>
    public string Senha { get; set; } = string.Empty;
}
