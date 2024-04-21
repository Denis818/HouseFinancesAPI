using Domain.Dtos.Despesas.Criacao;
using FluentValidation;

namespace Application.Validators.Despesas
{
    public class DespesaValidator : AbstractValidator<DespesaDto>
    {
        public DespesaValidator()
        {
            RuleFor(x => x.Item)
                .NotEmpty()
                .WithMessage("O {PropertyName} é obrigatório.")
                .Length(3, 25)
                .WithMessage(
                    "O {PropertyName} deve ter entre {MinLength} a {MaxLength} caracteres."
                );

            RuleFor(x => (double)x.Preco)
                .InclusiveBetween(0.01, 9999)
                .WithMessage(
                    "O {PropertyName} não pode ser menor que 0.01, e maior que 9999."
                );

            RuleFor(x => x.Quantidade)
                .InclusiveBetween(1, 999)
                .WithMessage(
                    "A {PropertyName} não pode ser menor que 1, e maior que 9999."
                );

            RuleFor(x => x.Fornecedor)
                .NotEmpty()
                .WithMessage("O {PropertyName} é obrigatório.")
                .Length(3, 25)
                .WithMessage(
                    "O {PropertyName} deve ter entre {MinLength} a {MaxLength} caracteres."
                );

            RuleFor(x => x.DataCompra)
                .NotEmpty()
                .WithMessage("A {PropertyName} é obrigatório.")
                .Must(data => data <= DateTime.Today)
                .WithMessage("A {PropertyName} não pode ser no futuro.")
                .Must(data => data.Year >= DateTime.Now.Year)
                .WithMessage("A {PropertyName} não pode ser de um ano anterior ao ano atual.");
        }
    }
}
