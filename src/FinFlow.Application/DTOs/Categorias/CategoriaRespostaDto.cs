using FinFlow.Domain.Enums;

namespace FinFlow.Application.DTOs.Categorias;

/// <summary>
/// DTO de resposta para operações com categorias.
/// Expõe apenas os dados necessários ao cliente, sem expor a entidade diretamente.
/// </summary>
public class CategoriaRespostaDto
{
    /// <summary>Identificador único da categoria.</summary>
    public Guid Id { get; set; }

    /// <summary>Nome descritivo da categoria.</summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>Tipo da categoria: Receita ou Saida.</summary>
    public TipoTransacao Tipo { get; set; }

    /// <summary>Data de criação da categoria.</summary>
    public DateTime CriadoEm { get; set; }
}
