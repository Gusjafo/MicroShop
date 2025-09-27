namespace MicroShop.BuildingBlocks.Abstractions.Correlation
{
  /// <summary>
  /// Provides access to the correlation context for the current execution flow.
  /// </summary>
  public interface ICorrelationContextAccessor
  {
    /// <summary>
    /// Gets or sets the active correlation identifiers.
    /// </summary>
    CorrelationIds Current { get; set; }
  }
}
