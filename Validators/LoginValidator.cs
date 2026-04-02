using FluentValidation;
using TicketSupportSystem.DTOs.Requests;

namespace TicketSupportSystem.Validators
{
    public class LoginValidator : AbstractValidator<UserLoginDTO>
    {
        public LoginValidator() 
        {
            RuleFor(ul => ul.Email)
                 .NotNull()
                 .WithMessage("Email обязателен.")
                 .EmailAddress()
                 .WithMessage("Некорректный email.");
            RuleFor(ut => ut.Password)
                 .NotNull()
                 .WithMessage("Пароль обязателен.")
                 .NotEmpty()
                 .WithMessage("Пароль обязателен.")
                 .MinimumLength(8)
                 .WithMessage("Пароль должен быть минимум 8 символов.")
                 .MaximumLength(64)
                 .WithMessage("Пароль не должен быть длиннее 64 символов.");
        }
    }
}
