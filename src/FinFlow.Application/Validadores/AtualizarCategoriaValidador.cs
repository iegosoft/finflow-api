using FinFlow.Application.DTOs.Categorias;
using FluentValidation;

namespace FinFlow.Application.Validadores;

/// <summary>
/// Validador das regras de entrada para atualização de uma categoria.
/// </summary>
public class AtualizarCategoriaValidador : AbstractValidator<AtualizarCategoriaDto>
{
    public AtualizarCategoriaValidador()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("O nome da categoria é obrigatório.")
            .MaximumLength(100).WithMessage("O nome deve ter no máximo 100 caracteres.");

        RuleFor(x => x.Tipo)
            .IsInEnum().WithMessage("O tipo deve ser Receita (1) ou Saida (2).");
    }
}
