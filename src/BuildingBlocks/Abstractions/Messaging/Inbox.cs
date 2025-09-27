namespace MicroShop.BuildingBlocks.Abstractions.Messaging
{
  public sealed record InboxMessage(
      string Id,
      string MessageType,
      DateTimeOffset ReceivedOnUtc,
      DateTimeOffset? ProcessedOnUtc = null);

  public interface IInboxStore
  {
    Task<bool> ExistsAsync(string id, CancellationToken ct = default);
    Task AddAsync(InboxMessage message, CancellationToken ct = default);
    Task MarkProcessedAsync(string id, CancellationToken ct = default);
  }
}
