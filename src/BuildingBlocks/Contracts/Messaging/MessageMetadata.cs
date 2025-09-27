namespace MicroShop.BuildingBlocks.Contracts.Messaging
{
  public sealed record MessageMetadata(
      string MessageId,
      string? CorrelationId,
      string? CausationId,
      DateTimeOffset OccurredOnUtc,
      string Version);
}
