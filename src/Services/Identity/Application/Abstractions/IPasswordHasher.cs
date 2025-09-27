using MicroShop.Services.Identity.Domain.Users;

namespace MicroShop.Services.Identity.Application.Abstractions;

/// <summary>
/// Provides hashing and verification capabilities for user passwords.
/// </summary>
public interface IPasswordHasher
{
  string HashPassword(User user, string password);

  bool VerifyHashedPassword(User user, string hashedPassword, string providedPassword);
}
