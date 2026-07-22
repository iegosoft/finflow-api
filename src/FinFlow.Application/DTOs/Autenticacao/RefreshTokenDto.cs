namespace FinFlow.Application.DTOs.Autenticacao;

/// <summary>
/// Dados enviados pelo cliente para renovar o JWT expirado.
/// </summary>
public class RefreshTokenDto
{
    /// <summary>Refresh token previamente emitido e ainda válido.</summary>
    public string RefreshToken { get; set; } = string.Empty;
}
