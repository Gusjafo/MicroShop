namespace MicroShop.BuildingBlocks.Abstractions.Messaging
{
  /// <summary>
  /// Represents a message tracked by the inbox pattern.
  /// </summary>
  /// <param name="Id">The unique message identifier.</param>
  /// <param name="MessageType">The CLR type name of the message.</param>
  /// <param name="ReceivedOnUtc">The UTC timestamp when the message was received.</param>
  /// <param name="ProcessedOnUtc">The UTC timestamp when the message was processed.</param>
  public sealed record InboxMessage(
      string Id,
      string MessageType,
      DateTimeOffset ReceivedOnUtc,
      DateTimeOffset? ProcessedOnUtc = null);

  /// <summary>
  /// Defines persistence operations for inbox messages.
  /// </summary>
  public interface IInboxStore
  {
    /// <summary>
    /// Determines whether a message with the provided identifier already exists.
    /// </summary>
    /// <param name="id">The message identifier.</param>
    /// <param name="ct">A cancellation token.</param>
    /// <returns><c>true</c> if the message exists; otherwise <c>false</c>.</returns>
    Task<bool> ExistsAsync(string id, CancellationToken ct = default);

    /// <summary>
    /// Stores the provided inbox message.
    /// </summary>
    /// <param name="message">The inbox message to store.</param>
    /// <param name="ct">A cancellation token.</param>
    Task AddAsync(InboxMessage message, CancellationToken ct = default);

    /// <summary>
    /// Marks the specified message as processed.
    /// </summary>
    /// <param name="id">The message identifier.</param>
    /// <param name="ct">A cancellation token.</param>
    Task MarkProcessedAsync(string id, CancellationToken ct = default);
  }
}
