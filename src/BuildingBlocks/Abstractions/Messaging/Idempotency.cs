namespace MicroShop.BuildingBlocks.Abstractions.Messaging
{
  /// <summary>
  /// Defines storage operations required to enforce message idempotency.
  /// </summary>
  public interface IIdempotencyStore
  {
    /// <summary>
    /// Attempts to begin processing for the specified message key.
    /// </summary>
    /// <param name="key">The unique message identifier.</param>
    /// <param name="ttl">The time the reservation remains valid.</param>
    /// <param name="ct">A token to observe while waiting for the task to complete.</param>
    /// <returns><c>true</c> if the message can be processed; otherwise <c>false</c>.</returns>
    Task<bool> TryBeginAsync(string key, TimeSpan ttl, CancellationToken ct = default);

    /// <summary>
    /// Marks the specified message key as completed.
    /// </summary>
    /// <param name="key">The unique message identifier.</param>
    /// <param name="ct">A token to observe while waiting for the task to complete.</param>
    Task EndAsync(string key, CancellationToken ct = default);
  }
}
