using FluentValidation;
using TicketSupportSystem.DTOs.Requests;

namespace TicketSupportSystem.Validators
{
    public class RegistrationValidator : AbstractValidator<UserRegistrationDTO>
    {
        public RegistrationValidator() 
        {
            RuleFor(ur => ur.Name)
                .NotNull()
                .WithMessage("Имя обязательно.")
                .NotEmpty()
                .WithMessage("Имя обязательно.")
                .MaximumLength(100)
                .WithMessage("Имя не должно быть длиннее 100 символов.");
            RuleFor(ur => ur.Surname)
                .NotNull()
                .WithMessage("Фамилия обязательна.")
                .NotEmpty()
                .WithMessage("Фамилия обязательна.")
                .MaximumLength(100)
                .WithMessage("Фамилия не должна быть длиннее 100 символов.");
            RuleFor(ur => ur.Email)
                 .NotNull()
                 .WithMessage("Email обязателен.")
                 .EmailAddress()
                 .WithMessage("Некорректный email.");
            RuleFor(ur => ur.UserName)
                 .NotNull()
                 .WithMessage("Username обязателен.")
                 .NotEmpty()
                 .WithMessage("Username обязателен.");
            RuleFor(ur => ur.PhoneNumber)
                 .NotNull()
                 .WithMessage("Номер телефона обязателен.")
                 .NotEmpty()
                 .WithMessage("Номер телефона обязателен.");
            RuleFor(ur => ur.Password)
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
