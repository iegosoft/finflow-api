using FinFlow.Domain.Enums;

namespace FinFlow.Application.DTOs.Categorias;

/// <summary>
/// Dados para atualizar uma categoria existente.
/// </summary>
public class AtualizarCategoriaDto
{
    /// <summary>Novo nome da categoria.</summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>Novo tipo da categoria: Receita ou Saida.</summary>
    public TipoTransacao Tipo { get; set; }
}
