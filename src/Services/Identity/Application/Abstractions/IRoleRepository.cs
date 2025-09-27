using MicroShop.Services.Identity.Domain.Users;

namespace MicroShop.Services.Identity.Application.Abstractions;

/// <summary>
/// Provides data access operations for <see cref="Role"/> entities.
/// </summary>
public interface IRoleRepository
{
  Task<Role?> GetByNameAsync(string name, CancellationToken ct = default);

  Task EnsureDefaultsAsync(IEnumerable<string> roleNames, CancellationToken ct = default);
}
