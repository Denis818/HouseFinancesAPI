using Domain.Models;
using FluentValidation;

namespace Application.Validators.Finance
{
    public class MembroValidator : AbstractValidator<MembroDto>
    {
        public MembroValidator()
        {
            RuleFor(x => x.Nome).NotEmpty().WithMessage("É obrigatório.")
                                .Length(3, 25).WithMessage("Deve ter entre 3 a 25 caracteres.");
        }
    }
}
