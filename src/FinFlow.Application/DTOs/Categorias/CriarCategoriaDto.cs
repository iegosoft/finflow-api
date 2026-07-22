using FinFlow.Domain.Enums;

namespace FinFlow.Application.DTOs.Categorias;

/// <summary>
/// Dados necessários para criar uma nova categoria financeira.
/// </summary>
public class CriarCategoriaDto
{
    /// <summary>Nome descritivo da categoria (ex: Alimentação, Salário).</summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>Tipo da categoria: Receita ou Saida.</summary>
    public TipoTransacao Tipo { get; set; }
}
