namespace FinFlow.Domain.Entidades;

/// <summary>
/// Classe base para todas as entidades do domínio.
/// Fornece os campos comuns de identificação e auditoria.
/// </summary>
public abstract class EntidadeBase
{
    /// <summary>Identificador único da entidade.</summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>Data e hora em que a entidade foi criada.</summary>
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
}
