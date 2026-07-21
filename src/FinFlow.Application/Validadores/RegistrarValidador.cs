using FinFlow.Application.DTOs.Autenticacao;
using FluentValidation;

namespace FinFlow.Application.Validadores;

/// <summary>
/// Validador das regras de entrada para registro de novo usuário.
/// </summary>
public class RegistrarValidador : AbstractValidator<RegistrarDto>
{
    public RegistrarValidador()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("O nome é obrigatório.")
            .MaximumLength(150).WithMessage("O nome deve ter no máximo 150 caracteres.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("O e-mail é obrigatório.")
            .EmailAddress().WithMessage("Informe um e-mail válido.");

        RuleFor(x => x.Senha)
            .NotEmpty().WithMessage("A senha é obrigatória.")
            .MinimumLength(6).WithMessage("A senha deve ter no mínimo 6 caracteres.");
    }
}
