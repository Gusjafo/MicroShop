using MicroShop.BuildingBlocks.Abstractions.Persistence;

namespace MicroShop.Services.Identity.Infrastructure.Persistence;

internal sealed class UnitOfWork : IUnitOfWork
{
  private readonly IdentityDbContext dbContext;

  public UnitOfWork(IdentityDbContext dbContext)
  {
    this.dbContext = dbContext;
  }

  public Task<int> SaveChangesAsync(CancellationToken ct = default)
  {
    return this.dbContext.SaveChangesAsync(ct);
  }
}
