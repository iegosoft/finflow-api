namespace FinFlow.Domain.Enums;

/// <summary>
/// Define o tipo de uma transação financeira.
/// </summary>
public enum TipoTransacao
{
    /// <summary>Entrada de dinheiro (ex: salário, renda extra).</summary>
    Receita = 1,

    /// <summary>Saída de dinheiro (ex: conta, compra, despesa).</summary>
    Saida = 2
}
