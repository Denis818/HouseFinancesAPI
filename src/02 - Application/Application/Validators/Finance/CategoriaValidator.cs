using Domain.Dtos.Finance.Records;
using FluentValidation;

namespace Application.Validators.Finance
{
    public class CategoriaValidator : AbstractValidator<CategoriaDto>
    {
        public CategoriaValidator()
        {
            RuleFor(x => x.Descricao).NotEmpty().WithMessage("É obrigatório.")
                                .Length(3, 20).WithMessage("Deve ter entre 3 a 25 caracteres.");
        }
    }
}
