using Microsoft.AspNetCore.Identity;
using MicroShop.Services.Identity.Application.Abstractions;
using MicroShop.Services.Identity.Domain.Users;

namespace MicroShop.Services.Identity.Infrastructure.Security;

internal sealed class PasswordHasherAdapter : IPasswordHasher
{
  private readonly IPasswordHasher<User> passwordHasher;

  public PasswordHasherAdapter(IPasswordHasher<User> passwordHasher)
  {
    this.passwordHasher = passwordHasher;
  }

  public string HashPassword(User user, string password)
  {
    return this.passwordHasher.HashPassword(user, password);
  }

  public bool VerifyHashedPassword(User user, string hashedPassword, string providedPassword)
  {
    var result = this.passwordHasher.VerifyHashedPassword(user, hashedPassword, providedPassword);
    return result == PasswordVerificationResult.Success || result == PasswordVerificationResult.SuccessRehashNeeded;
  }
}
