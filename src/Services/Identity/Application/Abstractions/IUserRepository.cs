using MicroShop.BuildingBlocks.Abstractions.Common;
using MicroShop.Services.Identity.Domain.Users;

namespace MicroShop.Services.Identity.Application.Abstractions;

/// <summary>
/// Provides data access operations for <see cref="User"/> aggregates.
/// </summary>
public interface IUserRepository
{
  Task AddAsync(User user, CancellationToken ct = default);

  Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default);

  Task<User?> GetByUsernameAsync(string username, CancellationToken ct = default);

  Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);

  Task<bool> ExistsByUsernameOrEmailAsync(string username, string email, CancellationToken ct = default);

  Task<IReadOnlyList<User>> ListAsync(PageRequest page, CancellationToken ct = default);

  Task<long> CountAsync(CancellationToken ct = default);
}
