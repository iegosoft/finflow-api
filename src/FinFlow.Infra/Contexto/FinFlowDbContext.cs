using FinFlow.Domain.Entidades;
using FinFlow.Infra.Configuracoes;
using Microsoft.EntityFrameworkCore;

namespace FinFlow.Infra.Contexto;

/// <summary>
/// Contexto principal do Entity Framework Core para a aplicação FinFlow.
/// Centraliza o acesso ao banco de dados e aplica os mapeamentos Fluent API.
/// </summary>
public class FinFlowDbContext : DbContext
{
    public FinFlowDbContext(DbContextOptions<FinFlowDbContext> opcoes) : base(opcoes)
    {
    }

    // ── DbSets representando as tabelas do banco ──
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Transacao> Transacoes => Set<Transacao>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ── Aplica todas as configurações Fluent API do assembly atual ──
        modelBuilder.ApplyConfiguration(new UsuarioConfiguracao());
        modelBuilder.ApplyConfiguration(new CategoriaConfiguracao());
        modelBuilder.ApplyConfiguration(new TransacaoConfiguracao());
    }
}
