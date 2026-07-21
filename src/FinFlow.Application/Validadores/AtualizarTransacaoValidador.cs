using FinFlow.Application.DTOs.Transacoes;
using FluentValidation;

namespace FinFlow.Application.Validadores;

/// <summary>
/// Validador das regras de entrada para atualização de uma transação.
/// </summary>
public class AtualizarTransacaoValidador : AbstractValidator<AtualizarTransacaoDto>
{
    public AtualizarTransacaoValidador()
    {
        RuleFor(x => x.Descricao)
            .NotEmpty().WithMessage("A descrição é obrigatória.")
            .MaximumLength(300).WithMessage("A descrição deve ter no máximo 300 caracteres.");

        RuleFor(x => x.Valor)
            .GreaterThan(0).WithMessage("O valor deve ser maior que zero.");

        RuleFor(x => x.Tipo)
            .IsInEnum().WithMessage("O tipo deve ser Receita (1) ou Saida (2).");

        RuleFor(x => x.Data)
            .NotEmpty().WithMessage("A data é obrigatória.");

        RuleFor(x => x.CategoriaId)
            .NotEmpty().WithMessage("A categoria é obrigatória.");
    }
}
