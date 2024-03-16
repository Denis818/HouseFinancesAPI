using Domain.Models.Dtos;
using FluentValidation;

namespace Application.Validators
{
    public class DespesaValidator : AbstractValidator<DespesaDto>
    {
        public DespesaValidator()
        {
            RuleFor(x => x.Categoria).NotEmpty().WithMessage("É obrigatório.")
                                     .Length(3, 25).WithMessage("Deve ter entre 3 a 25 caracteres.");

            RuleFor(x => x.Item).NotEmpty().WithMessage("É obrigatório.")
                                .Length(3, 25).WithMessage("Deve ter entre 3 a 25 caracteres.");

            RuleFor(x => (double)x.Preco).InclusiveBetween(0.01, 999)
                                         .WithMessage("Não pode ser menor que 0.1, e maior que 999.");

            RuleFor(x => x.Quantidade).InclusiveBetween(1, 999)
                                      .WithMessage("Não pode ser menor que 1, e maior que 999.");

            RuleFor(x => x.Fornecedor).NotEmpty().WithMessage("É obrigatório.")
                                      .Length(3, 25).WithMessage("Deve ter entre 3 a 25 caracteres.");

            RuleFor(x => x.DataCompra).NotEmpty().WithMessage("É obrigatório.")
                                      .Must(data => data <= DateTime.Today).WithMessage("Não pode ser no futuro.")
                                      .Must(data => data.Year >= DateTime.Now.Year).WithMessage("Não pode ser de um ano anterior ao ano atual.");

        }
    }
}
