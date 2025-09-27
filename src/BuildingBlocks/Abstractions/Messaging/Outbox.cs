namespace MicroShop.BuildingBlocks.Abstractions.Messaging
{
  public sealed record OutboxMessage(
      string Id,
      string MessageType,
      string Content,
      DateTimeOffset OccurredOnUtc,
      string? CorrelationId,
      string? CausationId,
      string Version,
      DateTimeOffset? ProcessedOnUtc = null,
      string? Error = null);

  public interface IOutboxStore
  {
    Task AddAsync(OutboxMessage message, CancellationToken ct = default);
    Task<IReadOnlyList<OutboxMessage>> GetUnprocessedAsync(int take = 100, CancellationToken ct = default);
    Task MarkProcessedAsync(string id, CancellationToken ct = default);
    Task MarkFailedAsync(string id, string error, CancellationToken ct = default);
  }
}
