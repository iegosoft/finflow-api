using FinFlow.Application.DTOs.Autenticacao;
using FinFlow.Application.Interfaces;
using FinFlow.Domain.Excecoes;
using Microsoft.AspNetCore.Mvc;

namespace FinFlow.API.Controllers;

/// <summary>
/// Controller responsável pelos endpoints de autenticação.
/// Gerencia registro de conta, login e renovação de token JWT.
/// </summary>
[ApiController]
[Route("api/auth")]
public class AutenticacaoController : ControllerBase
{
    private readonly IAutenticacaoServico _autenticacaoServico;

    public AutenticacaoController(IAutenticacaoServico autenticacaoServico)
    {
        _autenticacaoServico = autenticacaoServico;
    }

    /// <summary>
    /// Registra um novo usuário na plataforma e retorna os tokens de sessão.
    /// </summary>
    /// <param name="requisicao">Dados do novo usuário (nome, e-mail e senha).</param>
    /// <returns>JWT e refresh token para uso imediato.</returns>
    [HttpPost("registrar")]
    [ProducesResponseType(typeof(TokenRespostaDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Registrar([FromBody] RegistrarDto requisicao)
    {
        try
        {
            var resultado = await _autenticacaoServico.RegistrarAsync(requisicao);
            return StatusCode(StatusCodes.Status201Created, resultado);
        }
        catch (RegraDeNegocioException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    /// <summary>
    /// Autentica o usuário com e-mail e senha e retorna os tokens de sessão.
    /// </summary>
    /// <param name="requisicao">Credenciais do usuário (e-mail e senha).</param>
    /// <returns>JWT e refresh token.</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(TokenRespostaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginDto requisicao)
    {
        try
        {
            var resultado = await _autenticacaoServico.LoginAsync(requisicao);
            return Ok(resultado);
        }
        catch (RegraDeNegocioException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    /// <summary>
    /// Renova o JWT usando um refresh token válido, emitindo um novo par de tokens.
    /// </summary>
    /// <param name="requisicao">Refresh token atual do usuário.</param>
    /// <returns>Novo JWT e novo refresh token.</returns>
    [HttpPost("refresh-token")]
    [ProducesResponseType(typeof(TokenRespostaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RenovarToken([FromBody] RefreshTokenDto requisicao)
    {
        try
        {
            var resultado = await _autenticacaoServico.RenovarTokenAsync(requisicao);
            return Ok(resultado);
        }
        catch (RegraDeNegocioException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }
}
