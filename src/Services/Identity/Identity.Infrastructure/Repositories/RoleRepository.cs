using Microsoft.EntityFrameworkCore;
using MicroShop.Services.Identity.Application.Abstractions;
using MicroShop.Services.Identity.Domain.Constants;
using MicroShop.Services.Identity.Domain.Entities;
using MicroShop.Services.Identity.Infrastructure.Persistence;

namespace MicroShop.Services.Identity.Infrastructure.Repositories;

internal sealed class RoleRepository(IdentityDbContext dbContext) : IRoleRepository
{
    private readonly IdentityDbContext _dbContext = dbContext;

    public async Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Roles.AsNoTracking().FirstOrDefaultAsync(role => role.Name == name, cancellationToken);
    }

    public async Task<Role?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Roles.AsNoTracking().FirstOrDefaultAsync(role => role.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Role>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Roles.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task EnsureCreatedAsync(CancellationToken cancellationToken = default)
    {
        if (!await _dbContext.Roles.AnyAsync(cancellationToken))
        {
            var defaultRoles = new[]
            {
                Role.Create(RoleNames.Customer, "Default customer role", true),
                Role.Create(RoleNames.Admin, "Administrator role")
            };

            await _dbContext.Roles.AddRangeAsync(defaultRoles, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
