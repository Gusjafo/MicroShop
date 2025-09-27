using FluentValidation;
using MicroShop.Services.Identity.Application.DTOs;

namespace MicroShop.Services.Identity.Application.Users.Validators;

/// <summary>
/// Validates login payloads.
/// </summary>
public sealed class LoginRequestValidator : AbstractValidator<LoginRequest>
{
  public LoginRequestValidator()
  {
    this.RuleFor(request => request.UsernameOrEmail)
        .NotEmpty();

    this.RuleFor(request => request.Password)
        .NotEmpty();
  }
}
