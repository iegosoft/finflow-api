using FinFlow.Domain.Enums;

namespace FinFlow.Application.DTOs.Transacoes;

/// <summary>
/// DTO de resposta para operações com transações.
/// Inclui o nome da categoria para exibição sem necessidade de join no cliente.
/// </summary>
public class TransacaoRespostaDto
{
    /// <summary>Identificador único da transação.</summary>
    public Guid Id { get; set; }

    /// <summary>Descrição da transação.</summary>
    public string Descricao { get; set; } = string.Empty;

    /// <summary>Valor monetário da transação.</summary>
    public decimal Valor { get; set; }

    /// <summary>Tipo: Receita ou Saida.</summary>
    public TipoTransacao Tipo { get; set; }

    /// <summary>Data em que a transação ocorreu.</summary>
    public DateTime Data { get; set; }

    /// <summary>ID da categoria vinculada.</summary>
    public Guid CategoriaId { get; set; }

    /// <summary>Nome da categoria vinculada.</summary>
    public string CategoriaNome { get; set; } = string.Empty;

    /// <summary>Data de criação do registro.</summary>
    public DateTime CriadoEm { get; set; }
}
