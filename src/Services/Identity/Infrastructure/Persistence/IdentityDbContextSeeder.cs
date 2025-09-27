using Microsoft.EntityFrameworkCore;
using MicroShop.BuildingBlocks.Abstractions.IdGeneration;
using MicroShop.Services.Identity.Domain.Entities;
using System.Linq;

namespace MicroShop.Services.Identity.Infrastructure.Persistence;

public static class IdentityDbContextSeeder
{
    public static async Task SeedAsync(IdentityDbContext dbContext, IIdGenerator idGenerator, CancellationToken cancellationToken = default)
    {
        await dbContext.Database.MigrateAsync(cancellationToken);

        if (!await dbContext.Roles.AnyAsync(cancellationToken))
        {
            var roles = DefaultRoles.All
                .Select(roleName => Role.Create(idGenerator.NewId(), roleName))
                .ToArray();

            await dbContext.Roles.AddRangeAsync(roles, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
