using FinFlow.Application.DTOs.Categorias;
using FinFlow.Domain.Enums;
using FluentValidation;

namespace FinFlow.Application.Validadores;

/// <summary>
/// Validador das regras de entrada para criação de uma categoria.
/// </summary>
public class CriarCategoriaValidador : AbstractValidator<CriarCategoriaDto>
{
    public CriarCategoriaValidador()
    {
        // ── Nome é obrigatório e tem limite de caracteres ──
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("O nome da categoria é obrigatório.")
            .MaximumLength(100).WithMessage("O nome deve ter no máximo 100 caracteres.");

        // ── Tipo deve ser um valor válido do enum ──
        RuleFor(x => x.Tipo)
            .IsInEnum().WithMessage("O tipo deve ser Receita (1) ou Saida (2).");
    }
}
