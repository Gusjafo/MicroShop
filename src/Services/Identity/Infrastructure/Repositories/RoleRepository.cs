using Microsoft.EntityFrameworkCore;
using MicroShop.Services.Identity.Application.Abstractions;
using MicroShop.Services.Identity.Domain.Users;
using MicroShop.Services.Identity.Infrastructure.Persistence;

namespace MicroShop.Services.Identity.Infrastructure.Repositories;

internal sealed class RoleRepository : IRoleRepository
{
  private readonly IdentityDbContext dbContext;

  public RoleRepository(IdentityDbContext dbContext)
  {
    this.dbContext = dbContext;
  }

  public async Task EnsureDefaultsAsync(IEnumerable<string> roleNames, CancellationToken ct = default)
  {
    var existingNames = await this.dbContext.Roles
        .Where(role => roleNames.Contains(role.Name, StringComparer.OrdinalIgnoreCase))
        .Select(role => role.Name)
        .ToListAsync(ct)
        .ConfigureAwait(false);

    foreach (var roleName in roleNames.Except(existingNames, StringComparer.OrdinalIgnoreCase))
    {
      var normalized = roleName.Trim();
      if (normalized.Length == 0)
      {
        continue;
      }

      await this.dbContext.Roles.AddAsync(Role.Create(Guid.NewGuid(), normalized), ct).ConfigureAwait(false);
    }
  }

  public async Task<Role?> GetByNameAsync(string name, CancellationToken ct = default)
  {
    var normalized = name.Trim();
    return await this.dbContext.Roles.FirstOrDefaultAsync(role => role.Name == normalized, ct).ConfigureAwait(false);
  }
}
