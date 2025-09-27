using MicroShop.Services.Identity.Application.DTOs;
using MicroShop.Services.Identity.Domain.Users;

namespace MicroShop.Services.Identity.Application.Mapping;

/// <summary>
/// Provides mapping extensions for translating domain entities into application DTOs.
/// </summary>
public static class UserMappingExtensions
{
  public static UserDto ToDto(this User user)
  {
    if (user is null)
    {
      throw new ArgumentNullException(nameof(user));
    }

    return new UserDto(
        user.Id,
        user.Username,
        user.Email,
        user.Roles.Select(role => role.Name).ToArray(),
        user.CreatedAt);
  }
}
