using FinFlow.Domain.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinFlow.Infra.Configuracoes;

/// <summary>
/// Configuração Fluent API do mapeamento da entidade Transacao para o banco de dados.
/// </summary>
public class TransacaoConfiguracao : IEntityTypeConfiguration<Transacao>
{
    public void Configure(EntityTypeBuilder<Transacao> builder)
    {
        builder.ToTable("transacoes");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Descricao)
            .IsRequired()
            .HasMaxLength(300);

        // ── Precisão monetária: 18 dígitos no total, 2 decimais ──
        builder.Property(t => t.Valor)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(t => t.Tipo)
            .IsRequired();

        builder.Property(t => t.Data)
            .IsRequired();

        builder.Property(t => t.CategoriaId)
            .IsRequired();

        builder.Property(t => t.UsuarioId)
            .IsRequired();

        builder.Property(t => t.CriadoEm)
            .IsRequired();

        // ── Relacionamento: uma transação pertence a um único usuário ──
        builder.HasOne(t => t.Usuario)
            .WithMany(u => u.Transacoes)
            .HasForeignKey(t => t.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);

        // ── Relacionamento: uma transação pertence a uma única categoria ──
        builder.HasOne(t => t.Categoria)
            .WithMany(c => c.Transacoes)
            .HasForeignKey(t => t.CategoriaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
