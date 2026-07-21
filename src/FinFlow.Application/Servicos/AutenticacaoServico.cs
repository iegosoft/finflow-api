using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using FinFlow.Application.DTOs.Autenticacao;
using FinFlow.Application.Interfaces;
using FinFlow.Domain.Entidades;
using FinFlow.Domain.Excecoes;
using FinFlow.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace FinFlow.Application.Servicos;

/// <summary>
/// Serviço responsável pelo registro, autenticação e renovação de sessão dos usuários.
/// Utiliza BCrypt para hash de senha e JWT para geração de tokens de acesso.
/// </summary>
public class AutenticacaoServico : IAutenticacaoServico
{
    private readonly IUsuarioRepositorio _usuarioRepositorio;
    private readonly IConfiguration _configuracao;

    public AutenticacaoServico(IUsuarioRepositorio usuarioRepositorio, IConfiguration configuracao)
    {
        _usuarioRepositorio = usuarioRepositorio;
        _configuracao = configuracao;
    }

    /// <summary>
    /// Registra um novo usuário na plataforma.
    /// Valida unicidade do e-mail e aplica hash na senha antes de persistir.
    /// </summary>
    /// <param name="requisicao">Dados do novo usuário.</param>
    /// <returns>Token JWT e refresh token para a sessão criada.</returns>
    public async Task<TokenRespostaDto> RegistrarAsync(RegistrarDto requisicao)
    {
        // ── Valida se o e-mail já está cadastrado ──
        var usuarioExistente = await _usuarioRepositorio.ObterPorEmailAsync(requisicao.Email);
        if (usuarioExistente is not null)
            throw new RegraDeNegocioException("Já existe uma conta cadastrada com este e-mail.");

        // ── Cria o usuário com senha hasheada via BCrypt ──
        var usuario = new Usuario
        {
            Nome = requisicao.Nome,
            Email = requisicao.Email.ToLower().Trim(),
            SenhaHash = BCrypt.Net.BCrypt.HashPassword(requisicao.Senha)
        };

        // ── Gera e associa o refresh token antes de persistir ──
        AtualizarRefreshToken(usuario);

        await _usuarioRepositorio.AdicionarAsync(usuario);
        await _usuarioRepositorio.SalvarAlteracoesAsync();

        return GerarTokenResposta(usuario);
    }

    /// <summary>
    /// Autentica um usuário com e-mail e senha.
    /// Valida as credenciais e retorna os tokens de sessão.
    /// </summary>
    /// <param name="requisicao">Credenciais do usuário.</param>
    /// <returns>Token JWT e refresh token.</returns>
    public async Task<TokenRespostaDto> LoginAsync(LoginDto requisicao)
    {
        // ── Busca o usuário pelo e-mail ──
        var usuario = await _usuarioRepositorio.ObterPorEmailAsync(requisicao.Email);
        if (usuario is null)
            throw new RegraDeNegocioException("E-mail ou senha inválidos.");

        // ── Verifica se a senha corresponde ao hash armazenado ──
        if (!BCrypt.Net.BCrypt.Verify(requisicao.Senha, usuario.SenhaHash))
            throw new RegraDeNegocioException("E-mail ou senha inválidos.");

        // ── Renova o refresh token a cada login ──
        AtualizarRefreshToken(usuario);
        await _usuarioRepositorio.SalvarAlteracoesAsync();

        return GerarTokenResposta(usuario);
    }

    /// <summary>
    /// Renova o JWT usando um refresh token válido e não expirado.
    /// Invalida o refresh token anterior e emite um novo par de tokens.
    /// </summary>
    /// <param name="requisicao">Refresh token atual do usuário.</param>
    /// <returns>Novo token JWT e novo refresh token.</returns>
    public async Task<TokenRespostaDto> RenovarTokenAsync(RefreshTokenDto requisicao)
    {
        // ── Busca o usuário pelo refresh token informado ──
        var usuario = await _usuarioRepositorio.ObterPorRefreshTokenAsync(requisicao.RefreshToken);
        if (usuario is null)
            throw new RegraDeNegocioException("Refresh token inválido.");

        // ── Valida se o refresh token não expirou ──
        if (usuario.RefreshTokenExpiracao < DateTime.UtcNow)
            throw new RegraDeNegocioException("Refresh token expirado. Faça login novamente.");

        // ── Emite novo refresh token e persiste ──
        AtualizarRefreshToken(usuario);
        await _usuarioRepositorio.SalvarAlteracoesAsync();

        return GerarTokenResposta(usuario);
    }

    // ── Gera um refresh token aleatório e define sua expiração no usuário ──
    private void AtualizarRefreshToken(Usuario usuario)
    {
        var diasExpiracao = int.Parse(_configuracao["Jwt:ExpiracaoRefreshTokenDias"] ?? "7");
        usuario.RefreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        usuario.RefreshTokenExpiracao = DateTime.UtcNow.AddDays(diasExpiracao);
    }

    // ── Monta o DTO de resposta com JWT e refresh token ──
    private TokenRespostaDto GerarTokenResposta(Usuario usuario)
    {
        var (token, expiracao) = GerarJwt(usuario);
        return new TokenRespostaDto
        {
            Token = token,
            RefreshToken = usuario.RefreshToken!,
            Expiracao = expiracao
        };
    }

    // ── Gera o JWT com claims de identidade do usuário ──
    private (string Token, DateTime Expiracao) GerarJwt(Usuario usuario)
    {
        var chave = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuracao["Jwt:Chave"]!));

        var credenciais = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256);

        var horasExpiracao = int.Parse(_configuracao["Jwt:ExpiracaoHoras"] ?? "1");
        var expiracao = DateTime.UtcNow.AddHours(horasExpiracao);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, usuario.Email),
            new Claim(JwtRegisteredClaimNames.Name, usuario.Nome),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _configuracao["Jwt:Emissor"],
            audience: _configuracao["Jwt:Audiencia"],
            claims: claims,
            expires: expiracao,
            signingCredentials: credenciais);

        return (new JwtSecurityTokenHandler().WriteToken(token), expiracao);
    }
}
