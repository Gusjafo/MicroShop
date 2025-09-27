using MicroShop.Services.Identity.Application.DTOs;
using MicroShop.Services.Identity.Domain.Users;

namespace MicroShop.Services.Identity.Application.Abstractions;

/// <summary>
/// Generates JSON Web Tokens for authenticated principals.
/// </summary>
public interface IJwtService
{
  AuthResultDto CreateToken(User user, IEnumerable<string> roles);
}
