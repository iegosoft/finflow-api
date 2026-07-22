namespace FinFlow.Application.DTOs.Transacoes;

/// <summary>
/// Wrapper genérico para respostas paginadas.
/// Fornece os dados da página atual e metadados de navegação.
/// </summary>
public class PaginadoDto<T>
{
    /// <summary>Itens da página atual.</summary>
    public IEnumerable<T> Itens { get; set; } = [];

    /// <summary>Total de registros encontrados (sem paginação).</summary>
    public int Total { get; set; }

    /// <summary>Número da página atual.</summary>
    public int Pagina { get; set; }

    /// <summary>Quantidade de itens por página.</summary>
    public int TamanhoPagina { get; set; }

    /// <summary>Total de páginas disponíveis.</summary>
    public int TotalPaginas => (int)Math.Ceiling((double)Total / TamanhoPagina);
}
