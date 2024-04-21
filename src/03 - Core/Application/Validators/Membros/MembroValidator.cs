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
        }
    }
}
