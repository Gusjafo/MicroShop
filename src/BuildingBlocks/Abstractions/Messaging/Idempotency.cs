namespace MicroShop.BuildingBlocks.Abstractions.Messaging
{
  public interface IIdempotencyStore
  {
    Task<bool> TryBeginAsync(string key, TimeSpan ttl, CancellationToken ct = default);
    Task EndAsync(string key, CancellationToken ct = default);
  }
}
