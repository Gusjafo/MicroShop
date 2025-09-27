using FluentValidation;
using MicroShop.Services.Identity.Application.DTOs;

namespace MicroShop.Services.Identity.Application.Users.Validators;

/// <summary>
/// Validates registration payloads.
/// </summary>
public sealed class RegisterUserRequestValidator : AbstractValidator<RegisterUserRequest>
{
  public RegisterUserRequestValidator()
  {
    this.RuleFor(request => request.Username)
        .NotEmpty()
        .MinimumLength(3)
        .MaximumLength(32)
        .Must(username => !username.Contains(' '))
        .WithMessage("Username cannot contain spaces.");

    this.RuleFor(request => request.Email)
        .NotEmpty()
        .EmailAddress();

    this.RuleFor(request => request.Password)
        .NotEmpty()
        .MinimumLength(8)
        .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
        .Matches("[0-9]").WithMessage("Password must contain at least one digit.")
        .Matches("[\\W_]").WithMessage("Password must contain at least one symbol.");
  }
}
