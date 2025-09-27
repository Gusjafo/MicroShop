namespace MicroShop.BuildingBlocks.Abstractions.Messaging
{
  /// <summary>
  /// Represents a message persisted as part of the outbox pattern.
  /// </summary>
  /// <param name="Id">The unique message identifier.</param>
  /// <param name="MessageType">The CLR type name of the message.</param>
  /// <param name="Content">The serialized message payload.</param>
  /// <param name="OccurredOnUtc">The UTC timestamp when the message occurred.</param>
  /// <param name="CorrelationId">The correlation identifier associated with the message.</param>
  /// <param name="CausationId">The causation identifier associated with the message.</param>
  /// <param name="Version">The schema version of the message.</param>
  /// <param name="ProcessedOnUtc">The UTC timestamp when the message was processed.</param>
  /// <param name="Error">The error message captured when message processing fails.</param>
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

  /// <summary>
  /// Defines persistence operations for the outbox pattern.
  /// </summary>
  public interface IOutboxStore
  {
    /// <summary>
    /// Adds a new outbox message to the store.
    /// </summary>
    /// <param name="message">The message to add.</param>
    /// <param name="ct">A cancellation token.</param>
    Task AddAsync(OutboxMessage message, CancellationToken ct = default);

    /// <summary>
    /// Retrieves unprocessed messages from the store.
    /// </summary>
    /// <param name="take">The maximum number of messages to retrieve.</param>
    /// <param name="ct">A cancellation token.</param>
    /// <returns>A read-only list of unprocessed messages.</returns>
    Task<IReadOnlyList<OutboxMessage>> GetUnprocessedAsync(int take = 100, CancellationToken ct = default);

    /// <summary>
    /// Marks the message with the provided identifier as processed.
    /// </summary>
    /// <param name="id">The message identifier.</param>
    /// <param name="ct">A cancellation token.</param>
    Task MarkProcessedAsync(string id, CancellationToken ct = default);

    /// <summary>
    /// Marks the message with the provided identifier as failed.
    /// </summary>
    /// <param name="id">The message identifier.</param>
    /// <param name="error">The error encountered during processing.</param>
    /// <param name="ct">A cancellation token.</param>
    Task MarkFailedAsync(string id, string error, CancellationToken ct = default);
  }
}
