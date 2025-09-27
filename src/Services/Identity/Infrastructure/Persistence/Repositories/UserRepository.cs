using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MicroShop.Services.Identity.Application.Abstractions;
using MicroShop.Services.Identity.Domain.Entities;

namespace MicroShop.Services.Identity.Infrastructure.Persistence.Repositories;

internal sealed class UserRepository(IdentityDbContext dbContext) : IUserRepository
{
    public async Task<User?> FindByIdAsync(string id, CancellationToken cancellationToken = default)
        => await dbContext.Users
            .Include(user => EF.Property<ICollection<Role>>(user, "_roles"))
            .FirstOrDefaultAsync(user => user.Id == id, cancellationToken);

    public async Task<User?> FindByUsernameAsync(string username, CancellationToken cancellationToken = default)
        => await dbContext.Users
            .Include(user => EF.Property<ICollection<Role>>(user, "_roles"))
            .FirstOrDefaultAsync(user => user.Username == username, cancellationToken);

    public async Task<User?> FindByEmailAsync(string email, CancellationToken cancellationToken = default)
        => await dbContext.Users
            .Include(user => EF.Property<ICollection<Role>>(user, "_roles"))
            .FirstOrDefaultAsync(user => user.Email == email, cancellationToken);

    public async Task<IReadOnlyCollection<User>> GetAllAsync(CancellationToken cancellationToken = default)
        => await dbContext.Users
            .Include(user => EF.Property<ICollection<Role>>(user, "_roles"))
            .OrderBy(user => user.Username)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await dbContext.Users.AddAsync(user, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        dbContext.Users.Update(user);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
