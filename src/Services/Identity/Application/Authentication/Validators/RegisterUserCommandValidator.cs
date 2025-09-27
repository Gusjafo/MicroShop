using FluentValidation;
using MicroShop.Services.Identity.Application.Authentication.Commands;

namespace MicroShop.Services.Identity.Application.Authentication.Validators;

public sealed class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(64)
            .Matches("^[a-zA-Z0-9_.-]+$");

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8)
            .Matches("[A-Z]").WithMessage("Password must contain an uppercase letter.")
            .Matches("[a-z]").WithMessage("Password must contain a lowercase letter.")
            .Matches("[0-9]").WithMessage("Password must contain a digit.")
            .Matches("[!@#$%^&*()_+\-=\\[\\]{};':\"\\|,.<>\/?]").WithMessage("Password must contain a special character.");
    }
}
