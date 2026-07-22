namespace FinFlow.Application.DTOs.Autenticacao;

/// <summary>
/// Dados necessários para registrar um novo usuário na plataforma.
/// </summary>
public class RegistrarDto
{
    /// <summary>Nome completo do usuário.</summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>E-mail único que será usado para autenticação.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Senha escolhida pelo usuário (mínimo 6 caracteres).</summary>
    public string Senha { get; set; } = string.Empty;
}
