using System;
using FinFlow.Infra.Contexto;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FinFlow.Infra.Migrations
{
    [DbContext(typeof(FinFlowDbContext))]
    partial class FinFlowDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("FinFlow.Domain.Entidades.Categoria", b =>
            {
                b.Property<Guid>("Id").HasColumnType("uuid");
                b.Property<DateTime>("CriadoEm").HasColumnType("timestamp with time zone");
                b.Property<string>("Nome").IsRequired().HasMaxLength(100).HasColumnType("character varying(100)");
                b.Property<int>("Tipo").HasColumnType("integer");
                b.Property<Guid>("UsuarioId").HasColumnType("uuid");
                b.HasKey("Id");
                b.HasIndex("UsuarioId");
                b.ToTable("categorias");
            });

            modelBuilder.Entity("FinFlow.Domain.Entidades.Transacao", b =>
            {
                b.Property<Guid>("Id").HasColumnType("uuid");
                b.Property<Guid>("CategoriaId").HasColumnType("uuid");
                b.Property<DateTime>("CriadoEm").HasColumnType("timestamp with time zone");
                b.Property<DateTime>("Data").HasColumnType("timestamp with time zone");
                b.Property<string>("Descricao").IsRequired().HasMaxLength(300).HasColumnType("character varying(300)");
                b.Property<int>("Tipo").HasColumnType("integer");
                b.Property<Guid>("UsuarioId").HasColumnType("uuid");
                b.Property<decimal>("Valor").HasPrecision(18, 2).HasColumnType("numeric(18,2)");
                b.HasKey("Id");
                b.HasIndex("CategoriaId");
                b.HasIndex("UsuarioId");
                b.ToTable("transacoes");
            });

            modelBuilder.Entity("FinFlow.Domain.Entidades.Usuario", b =>
            {
                b.Property<Guid>("Id").HasColumnType("uuid");
                b.Property<DateTime>("CriadoEm").HasColumnType("timestamp with time zone");
                b.Property<string>("Email").IsRequired().HasMaxLength(200).HasColumnType("character varying(200)");
                b.Property<string>("Nome").IsRequired().HasMaxLength(150).HasColumnType("character varying(150)");
                b.Property<string>("RefreshToken").HasMaxLength(500).HasColumnType("character varying(500)");
                b.Property<DateTime?>("RefreshTokenExpiracao").HasColumnType("timestamp with time zone");
                b.Property<string>("SenhaHash").IsRequired().HasColumnType("text");
                b.HasKey("Id");
                b.HasIndex("Email").IsUnique();
                b.ToTable("usuarios");
            });

            modelBuilder.Entity("FinFlow.Domain.Entidades.Categoria", b =>
            {
                b.HasOne("FinFlow.Domain.Entidades.Usuario", "Usuario")
                    .WithMany("Categorias")
                    .HasForeignKey("UsuarioId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
                b.Navigation("Usuario");
            });

            modelBuilder.Entity("FinFlow.Domain.Entidades.Transacao", b =>
            {
                b.HasOne("FinFlow.Domain.Entidades.Categoria", "Categoria")
                    .WithMany("Transacoes")
                    .HasForeignKey("CategoriaId")
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();
                b.HasOne("FinFlow.Domain.Entidades.Usuario", "Usuario")
                    .WithMany("Transacoes")
                    .HasForeignKey("UsuarioId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
                b.Navigation("Categoria");
                b.Navigation("Usuario");
            });

            modelBuilder.Entity("FinFlow.Domain.Entidades.Categoria", b =>
            {
                b.Navigation("Transacoes");
            });

            modelBuilder.Entity("FinFlow.Domain.Entidades.Usuario", b =>
            {
                b.Navigation("Categorias");
                b.Navigation("Transacoes");
            });
#pragma warning restore 612, 618
        }
    }
}
