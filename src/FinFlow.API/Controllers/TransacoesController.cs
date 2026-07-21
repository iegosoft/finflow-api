using System.Security.Claims;
using FinFlow.Application.DTOs.Transacoes;
using FinFlow.Application.Interfaces;
using FinFlow.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinFlow.API.Controllers;

/// <summary>
/// Controller responsável pelo CRUD de transações financeiras e relatório mensal.
/// Todos os endpoints exigem autenticação JWT e operam sobre os dados do usuário autenticado.
/// </summary>
[ApiController]
[Route("api/transacoes")]
[Authorize]
public class TransacoesController : ControllerBase
{
    private readonly ITransacaoServico _transacaoServico;

    public TransacoesController(ITransacaoServico transacaoServico)
    {
        _transacaoServico = transacaoServico;
    }

    /// <summary>
    /// Lista transações do usuário com filtros opcionais e paginação.
    /// </summary>
    /// <param name="tipo">Filtra por tipo: 1 = Receita, 2 = Saida.</param>
    /// <param name="categoriaId">Filtra por categoria.</param>
    /// <param name="dataInicio">Filtra a partir desta data.</param>
    /// <param name="dataFim">Filtra até esta data.</param>
    /// <param name="pagina">Número da página (padrão: 1).</param>
    /// <param name="tamanhoPagina">Itens por página (padrão: 20).</param>
    [HttpGet]
    [ProducesResponseType(typeof(PaginadoDto<TransacaoRespostaDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar(
        [FromQuery] TipoTransacao? tipo,
        [FromQuery] Guid? categoriaId,
        [FromQuery] DateTime? dataInicio,
        [FromQuery] DateTime? dataFim,
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanhoPagina = 20)
    {
        var usuarioId = ObterUsuarioId();
        var resultado = await _transacaoServico.ListarAsync(
            usuarioId, tipo, categoriaId, dataInicio, dataFim, pagina, tamanhoPagina);
        return Ok(resultado);
    }

    /// <summary>
    /// Busca uma transação específica pelo ID.
    /// </summary>
    /// <param name="id">ID da transação.</param>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(TransacaoRespostaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId(Guid id)
    {
        var usuarioId = ObterUsuarioId();
        var transacao = await _transacaoServico.ObterPorIdAsync(id, usuarioId);
        return Ok(transacao);
    }

    /// <summary>
    /// Cria uma nova transação para o usuário autenticado.
    /// </summary>
    /// <param name="requisicao">Dados da transação a ser criada.</param>
    [HttpPost]
    [ProducesResponseType(typeof(TransacaoRespostaDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Criar([FromBody] CriarTransacaoDto requisicao)
    {
        var usuarioId = ObterUsuarioId();
        var transacao = await _transacaoServico.CriarAsync(requisicao, usuarioId);
        return StatusCode(StatusCodes.Status201Created, transacao);
    }

    /// <summary>
    /// Atualiza uma transação existente do usuário autenticado.
    /// </summary>
    /// <param name="id">ID da transação a ser atualizada.</param>
    /// <param name="requisicao">Novos dados da transação.</param>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(TransacaoRespostaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Atualizar(Guid id, [FromBody] AtualizarTransacaoDto requisicao)
    {
        var usuarioId = ObterUsuarioId();
        var transacao = await _transacaoServico.AtualizarAsync(id, requisicao, usuarioId);
        return Ok(transacao);
    }

    /// <summary>
    /// Remove uma transação do usuário autenticado.
    /// </summary>
    /// <param name="id">ID da transação a ser removida.</param>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Remover(Guid id)
    {
        var usuarioId = ObterUsuarioId();
        await _transacaoServico.RemoverAsync(id, usuarioId);
        return NoContent();
    }

    /// <summary>
    /// Retorna o relatório mensal com totais por categoria e saldo do mês.
    /// </summary>
    /// <param name="mes">Mês do relatório (1-12). Padrão: mês atual.</param>
    /// <param name="ano">Ano do relatório. Padrão: ano atual.</param>
    [HttpGet("relatorio-mensal")]
    [ProducesResponseType(typeof(RelatorioMensalDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> RelatorioMensal(
        [FromQuery] int mes = 0,
        [FromQuery] int ano = 0)
    {
        // ── Se não informado, usa o mês e ano correntes ──
        if (mes == 0) mes = DateTime.UtcNow.Month;
        if (ano == 0) ano = DateTime.UtcNow.Year;

        var usuarioId = ObterUsuarioId();
        var relatorio = await _transacaoServico.ObterRelatorioMensalAsync(usuarioId, mes, ano);
        return Ok(relatorio);
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
