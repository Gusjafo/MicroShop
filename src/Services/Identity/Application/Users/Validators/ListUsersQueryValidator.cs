using FluentValidation;

namespace MicroShop.Services.Identity.Application.Users.Validators;

/// <summary>
/// Validates pagination requests for listing users.
/// </summary>
public sealed class ListUsersQueryValidator : AbstractValidator<Queries.ListUsersQuery>
{
  public ListUsersQueryValidator()
  {
    this.RuleFor(query => query.Page)
        .GreaterThanOrEqualTo(1);

    this.RuleFor(query => query.Size)
        .GreaterThan(0)
        .LessThanOrEqualTo(100);
  }
}
