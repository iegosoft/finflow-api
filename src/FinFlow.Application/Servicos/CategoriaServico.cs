using FinFlow.Application.DTOs.Categorias;
using FinFlow.Application.Interfaces;
using FinFlow.Domain.Entidades;
using FinFlow.Domain.Excecoes;
using FinFlow.Domain.Interfaces;

namespace FinFlow.Application.Servicos;

/// <summary>
/// Serviço responsável pelo CRUD de categorias financeiras.
/// Garante que cada usuário acesse apenas suas próprias categorias.
/// </summary>
public class CategoriaServico : ICategoriaServico
{
    private readonly ICategoriaRepositorio _categoriaRepositorio;

    public CategoriaServico(ICategoriaRepositorio categoriaRepositorio)
    {
        _categoriaRepositorio = categoriaRepositorio;
    }

    /// <summary>
    /// Lista todas as categorias do usuário autenticado, ordenadas por nome.
    /// </summary>
    /// <param name="usuarioId">ID do usuário autenticado via JWT.</param>
    public async Task<IEnumerable<CategoriaRespostaDto>> ListarAsync(Guid usuarioId)
    {
        var categorias = await _categoriaRepositorio.ListarPorUsuarioAsync(usuarioId);
        return categorias.Select(MapearParaDto);
    }

    /// <summary>
    /// Cria uma nova categoria vinculada ao usuário autenticado.
    /// </summary>
    /// <param name="requisicao">Dados da categoria a ser criada.</param>
    /// <param name="usuarioId">ID do usuário autenticado via JWT.</param>
    public async Task<CategoriaRespostaDto> CriarAsync(CriarCategoriaDto requisicao, Guid usuarioId)
    {
        var categoria = new Categoria
        {
            Nome = requisicao.Nome.Trim(),
            Tipo = requisicao.Tipo,
            UsuarioId = usuarioId
        };

        await _categoriaRepositorio.AdicionarAsync(categoria);
        await _categoriaRepositorio.SalvarAlteracoesAsync();

        return MapearParaDto(categoria);
    }

    /// <summary>
    /// Atualiza nome e tipo de uma categoria existente do usuário autenticado.
    /// </summary>
    /// <param name="id">ID da categoria a ser atualizada.</param>
    /// <param name="requisicao">Novos dados da categoria.</param>
    /// <param name="usuarioId">ID do usuário autenticado via JWT.</param>
    public async Task<CategoriaRespostaDto> AtualizarAsync(Guid id, AtualizarCategoriaDto requisicao, Guid usuarioId)
    {
        // ── Garante que a categoria pertence ao usuário autenticado ──
        var categoria = await _categoriaRepositorio.ObterPorIdEUsuarioAsync(id, usuarioId)
            ?? throw new RegraDeNegocioException("Categoria não encontrada.");

        categoria.Nome = requisicao.Nome.Trim();
        categoria.Tipo = requisicao.Tipo;

        _categoriaRepositorio.Atualizar(categoria);
        await _categoriaRepositorio.SalvarAlteracoesAsync();

        return MapearParaDto(categoria);
    }

    /// <summary>
    /// Remove uma categoria do usuário autenticado.
    /// </summary>
    /// <param name="id">ID da categoria a ser removida.</param>
    /// <param name="usuarioId">ID do usuário autenticado via JWT.</param>
    public async Task RemoverAsync(Guid id, Guid usuarioId)
    {
        // ── Garante que a categoria pertence ao usuário autenticado ──
        var categoria = await _categoriaRepositorio.ObterPorIdEUsuarioAsync(id, usuarioId)
            ?? throw new RegraDeNegocioException("Categoria não encontrada.");

        _categoriaRepositorio.Remover(categoria);
        await _categoriaRepositorio.SalvarAlteracoesAsync();
    }

    // ── Mapeia a entidade Categoria para o DTO de resposta ──
    private static CategoriaRespostaDto MapearParaDto(Categoria categoria) => new()
    {
        Id = categoria.Id,
        Nome = categoria.Nome,
        Tipo = categoria.Tipo,
        CriadoEm = categoria.CriadoEm
    };
}
