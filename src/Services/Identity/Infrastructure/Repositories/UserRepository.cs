using Microsoft.EntityFrameworkCore;
using MicroShop.BuildingBlocks.Abstractions.Common;
using MicroShop.Services.Identity.Application.Abstractions;
using MicroShop.Services.Identity.Domain.Users;
using MicroShop.Services.Identity.Infrastructure.Persistence;

namespace MicroShop.Services.Identity.Infrastructure.Repositories;

internal sealed class UserRepository : IUserRepository
{
  private readonly IdentityDbContext dbContext;

  public UserRepository(IdentityDbContext dbContext)
  {
    this.dbContext = dbContext;
  }

  public Task AddAsync(User user, CancellationToken ct = default)
  {
    return this.dbContext.Users.AddAsync(user, ct).AsTask();
  }

  public async Task<long> CountAsync(CancellationToken ct = default)
  {
    return await this.dbContext.Users.AsNoTracking().LongCountAsync(ct).ConfigureAwait(false);
  }

  public async Task<bool> ExistsByUsernameOrEmailAsync(string username, string email, CancellationToken ct = default)
  {
    return await this.dbContext.Users
        .AsNoTracking()
        .AnyAsync(user => user.Username == username || user.Email == email, ct)
        .ConfigureAwait(false);
  }

  public async Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
  {
    return await this.dbContext.Users
        .AsNoTracking()
        .Include(user => user.Roles)
        .FirstOrDefaultAsync(user => user.Email == email, ct)
        .ConfigureAwait(false);
  }

  public async Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default)
  {
    return await this.dbContext.Users
        .AsNoTracking()
        .Include(user => user.Roles)
        .FirstOrDefaultAsync(user => user.Id == id, ct)
        .ConfigureAwait(false);
  }

  public async Task<User?> GetByUsernameAsync(string username, CancellationToken ct = default)
  {
    return await this.dbContext.Users
        .AsNoTracking()
        .Include(user => user.Roles)
        .FirstOrDefaultAsync(user => user.Username == username, ct)
        .ConfigureAwait(false);
  }

  public async Task<IReadOnlyList<User>> ListAsync(PageRequest page, CancellationToken ct = default)
  {
    return await this.dbContext.Users
        .AsNoTracking()
        .Include(user => user.Roles)
        .OrderBy(user => user.Username)
        .Skip(page.Skip)
        .Take(page.Size)
        .ToListAsync(ct)
        .ConfigureAwait(false);
  }
}
