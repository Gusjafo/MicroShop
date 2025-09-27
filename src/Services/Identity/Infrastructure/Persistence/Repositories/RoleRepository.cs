using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MicroShop.Services.Identity.Application.Abstractions;
using MicroShop.Services.Identity.Domain.Entities;

namespace MicroShop.Services.Identity.Infrastructure.Persistence.Repositories;

internal sealed class RoleRepository(IdentityDbContext dbContext) : IRoleRepository
{
    public async Task<Role?> FindByNameAsync(string name, CancellationToken cancellationToken = default)
        => await dbContext.Roles.FirstOrDefaultAsync(role => role.Name == name, cancellationToken);

    public async Task<IReadOnlyCollection<Role>> GetDefaultRolesAsync(CancellationToken cancellationToken = default)
        => await dbContext.Roles
            .Where(role => DefaultRoles.All.Contains(role.Name))
            .OrderBy(role => role.Name)
            .ToListAsync(cancellationToken);
}
