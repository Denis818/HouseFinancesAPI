using Domain.Dtos.User;
using FluentValidation;

namespace Application.Validators.User
{
    public class UserValidator : AbstractValidator<UserDto>
    {
        public UserValidator()
        {
            RuleFor(x => x.Email).NotEmpty()
                                 .WithMessage("É obrigatório.")
                                 .EmailAddress().WithMessage("Deve ser um email válido.")
                                 .Length(3, 25).WithMessage("Deve ter entre 3 a 25 caracteres.");

            RuleFor(x => x.Password).NotEmpty().WithMessage("A senha é obrigatória.")
                                    .MinimumLength(4).WithMessage("A senha deve ter no mínimo 4 caracteres.")
                                    .Matches("[A-Z]").WithMessage("A senha deve conter ao menos uma letra maiúscula.")
                                    .Matches("[0-9]").WithMessage("A senha deve conter ao menos um número.")
                                    .Matches("[^a-zA-Z0-9]").WithMessage("A senha deve conter ao menos um caractere especial.");

        }
    }
}
