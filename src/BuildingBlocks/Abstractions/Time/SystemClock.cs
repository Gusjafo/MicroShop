namespace MicroShop.BuildingBlocks.Abstractions.Time
{
  /// <summary>
  /// Provides the system implementation of <see cref="IClock"/>.
  /// </summary>
  public sealed class SystemClock : IClock
  {
    /// <inheritdoc />
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
  }
}
