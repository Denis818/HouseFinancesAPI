using Domain.Dtos.Membros;
using FluentValidation;

namespace Application.Validators.Membros
{
    public class MembroValidator : AbstractValidator<MembroDto>
    {
        public MembroValidator()
        {
            RuleFor(x => x.Nome)
                .NotEmpty()
                .WithMessage("O {PropertyName} é obrigatório.")
                .Length(3, 25)
                .WithMessage(
                    "O {PropertyName} deve ter entre {MinLength} a {MaxLength} caracteres."
                );

            RuleFor(x => x.Telefone)
               .NotEmpty()
               .WithMessage("O telefone é obrigatório.")
               .Matches(@"^\(\d{2}\) \d{5}-\d{4}$")
               .WithMessage("O telefone deve ser um número válido no formato (XX) XXXXX-XXXX.");
        }
    }
}
