namespace MicroShop.BuildingBlocks.Abstractions.Time
{
  /// <summary>
  /// Abstraction for retrieving the current time.
  /// </summary>
  public interface IClock
  {
    /// <summary>
    /// Gets the current UTC timestamp.
    /// </summary>
    DateTimeOffset UtcNow { get; }
  }
}
