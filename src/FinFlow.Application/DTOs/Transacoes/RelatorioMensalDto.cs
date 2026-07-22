namespace FinFlow.Application.DTOs.Transacoes;

/// <summary>
/// DTO do relatório mensal financeiro do usuário.
/// Agrupa totais por categoria e apresenta o saldo do mês.
/// </summary>
public class RelatorioMensalDto
{
    /// <summary>Mês de referência do relatório (1-12).</summary>
    public int Mes { get; set; }

    /// <summary>Ano de referência do relatório.</summary>
    public int Ano { get; set; }

    /// <summary>Soma de todas as receitas do mês.</summary>
    public decimal TotalReceitas { get; set; }

    /// <summary>Soma de todas as saídas do mês.</summary>
    public decimal TotalSaidas { get; set; }

    /// <summary>Saldo do mês: TotalReceitas - TotalSaidas.</summary>
    public decimal Saldo => TotalReceitas - TotalSaidas;

    /// <summary>Totais agrupados por categoria.</summary>
    public IEnumerable<RelatorioCategoriasDto> Categorias { get; set; } = [];
}

/// <summary>
/// Total de transações agrupadas por categoria no relatório mensal.
/// </summary>
public class RelatorioCategoriasDto
{
    /// <summary>Nome da categoria.</summary>
    public string CategoriaNome { get; set; } = string.Empty;

    /// <summary>Tipo da categoria: Receita ou Saida.</summary>
    public string Tipo { get; set; } = string.Empty;

    /// <summary>Soma dos valores das transações desta categoria no mês.</summary>
    public decimal Total { get; set; }

    /// <summary>Quantidade de transações nesta categoria no mês.</summary>
    public int Quantidade { get; set; }
}
