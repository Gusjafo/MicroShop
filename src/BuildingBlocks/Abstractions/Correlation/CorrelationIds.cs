namespace MicroShop.BuildingBlocks.Abstractions.Correlation
{
  /// <summary>
  /// Encapsulates correlation identifiers propagated between services.
  /// </summary>
  /// <param name="CorrelationId">The identifier that links related operations.</param>
  /// <param name="CausationId">The identifier for the operation that triggered the current action.</param>
  public sealed record CorrelationIds(string? CorrelationId, string? CausationId);
}
