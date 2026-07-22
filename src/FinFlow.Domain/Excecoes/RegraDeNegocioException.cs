namespace FinFlow.Domain.Excecoes;

/// <summary>
/// Exceção lançada quando uma regra de negócio do domínio é violada.
/// Utilizada para comunicar ao caller que a operação não pode ser concluída
/// por um motivo de lógica de negócio (não um erro de sistema).
/// </summary>
public class RegraDeNegocioException : Exception
{
    /// <summary>
    /// Inicializa a exceção com uma mensagem descritiva da regra violada.
    /// </summary>
    /// <param name="mensagem">Descrição da regra de negócio violada.</param>
    public RegraDeNegocioException(string mensagem) : base(mensagem)
    {
    }
}
