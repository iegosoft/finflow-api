using FinFlow.Domain.Enums;

namespace FinFlow.Application.DTOs.Transacoes;

/// <summary>
/// Dados necessários para registrar uma nova transação financeira.
/// </summary>
public class CriarTransacaoDto
{
    /// <summary>Descrição da transação (ex: "Aluguel de julho").</summary>
    public string Descricao { get; set; } = string.Empty;

    /// <summary>Valor monetário da transação. Deve ser maior que zero.</summary>
    public decimal Valor { get; set; }

    /// <summary>Tipo da transação: Receita ou Saida.</summary>
    public TipoTransacao Tipo { get; set; }

    /// <summary>Data em que a transação ocorreu.</summary>
    public DateTime Data { get; set; }

    /// <summary>ID da categoria à qual esta transação pertence.</summary>
    public Guid CategoriaId { get; set; }
}
