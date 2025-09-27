namespace MicroShop.BuildingBlocks.Abstractions.Persistence
{
  public interface IUnitOfWork
  {
    Task<int> SaveChangesAsync(CancellationToken ct = default);
  }
}
