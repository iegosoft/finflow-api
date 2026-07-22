using FinFlow.Domain.Enums;

namespace FinFlow.Application.DTOs.Transacoes;

/// <summary>
/// Dados para atualizar uma transação existente.
/// </summary>
public class AtualizarTransacaoDto
{
    /// <summary>Nova descrição da transação.</summary>
    public string Descricao { get; set; } = string.Empty;

    /// <summary>Novo valor monetário. Deve ser maior que zero.</summary>
    public decimal Valor { get; set; }

    /// <summary>Novo tipo da transação: Receita ou Saida.</summary>
    public TipoTransacao Tipo { get; set; }

    /// <summary>Nova data da transação.</summary>
    public DateTime Data { get; set; }

    /// <summary>Novo ID da categoria.</summary>
    public Guid CategoriaId { get; set; }
}
