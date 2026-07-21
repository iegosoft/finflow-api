using System.Security.Claims;
using FinFlow.Application.DTOs.Categorias;
using FinFlow.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinFlow.API.Controllers;

/// <summary>
/// Controller responsável pelo CRUD de categorias financeiras.
/// Todos os endpoints exigem autenticação JWT e operam sobre as categorias do usuário autenticado.
/// </summary>
[ApiController]
[Route("api/categorias")]
[Authorize]
public class CategoriasController : ControllerBase
{
    private readonly ICategoriaServico _categoriaServico;

    public CategoriasController(ICategoriaServico categoriaServico)
    {
        _categoriaServico = categoriaServico;
    }

    /// <summary>
    /// Lista todas as categorias do usuário autenticado.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CategoriaRespostaDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar()
    {
        var usuarioId = ObterUsuarioId();
        var categorias = await _categoriaServico.ListarAsync(usuarioId);
        return Ok(categorias);
    }

    /// <summary>
    /// Cria uma nova categoria para o usuário autenticado.
    /// </summary>
    /// <param name="requisicao">Dados da nova categoria.</param>
    [HttpPost]
    [ProducesResponseType(typeof(CategoriaRespostaDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Criar([FromBody] CriarCategoriaDto requisicao)
    {
        var usuarioId = ObterUsuarioId();
        var categoria = await _categoriaServico.CriarAsync(requisicao, usuarioId);
        return StatusCode(StatusCodes.Status201Created, categoria);
    }

    /// <summary>
    /// Atualiza uma categoria existente do usuário autenticado.
    /// </summary>
    /// <param name="id">ID da categoria a ser atualizada.</param>
    /// <param name="requisicao">Novos dados da categoria.</param>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(CategoriaRespostaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Atualizar(Guid id, [FromBody] AtualizarCategoriaDto requisicao)
    {
        var usuarioId = ObterUsuarioId();
        var categoria = await _categoriaServico.AtualizarAsync(id, requisicao, usuarioId);
        return Ok(categoria);
    }

    /// <summary>
    /// Remove uma categoria do usuário autenticado.
    /// </summary>
    /// <param name="id">ID da categoria a ser removida.</param>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Remover(Guid id)
    {
        var usuarioId = ObterUsuarioId();
        await _categoriaServico.RemoverAsync(id, usuarioId);
        return NoContent();
    }

    // ── Extrai o ID do usuário autenticado a partir das claims do JWT ──
    private Guid ObterUsuarioId()
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub")
            ?? throw new UnauthorizedAccessException("Usuário não autenticado.");

        return Guid.Parse(claim);
    }
}
