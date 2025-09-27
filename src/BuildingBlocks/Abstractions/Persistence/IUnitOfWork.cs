namespace MicroShop.BuildingBlocks.Abstractions.Persistence
{
  /// <summary>
  /// Coordinates transactional persistence operations.
  /// </summary>
  public interface IUnitOfWork
  {
    /// <summary>
    /// Persists pending changes to the underlying data store.
    /// </summary>
    /// <param name="ct">A cancellation token.</param>
    /// <returns>The number of affected entries.</returns>
    Task<int> SaveChangesAsync(CancellationToken ct = default);
  }
}
