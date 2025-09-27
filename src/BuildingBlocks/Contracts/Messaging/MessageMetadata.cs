using System;

namespace MicroShop.BuildingBlocks.Contracts.Messaging
{
  /// <summary>
  /// Encapsulates metadata that accompanies integration messages for tracing and versioning purposes.
  /// </summary>
  public sealed record MessageMetadata
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="MessageMetadata"/> record.
    /// </summary>
    /// <param name="messageId">The unique identifier associated with the message.</param>
    /// <param name="correlationId">The optional identifier that correlates related messages.</param>
    /// <param name="causationId">The optional identifier that indicates the cause of the message.</param>
    /// <param name="occurredOnUtc">The timestamp in UTC when the message occurred.</param>
    /// <param name="version">The semantic version of the message schema.</param>
    public MessageMetadata(
        string messageId,
        string? correlationId,
        string? causationId,
        DateTimeOffset occurredOnUtc,
        string version)
    {
      MessageId = messageId;
      CorrelationId = correlationId;
      CausationId = causationId;
      OccurredOnUtc = occurredOnUtc;
      Version = version;
    }

    /// <summary>
    /// Gets the unique identifier associated with the message.
    /// </summary>
    public string MessageId { get; init; }

    /// <summary>
    /// Gets the optional identifier that correlates related messages.
    /// </summary>
    public string? CorrelationId { get; init; }

    /// <summary>
    /// Gets the optional identifier that indicates the cause of the message.
    /// </summary>
    public string? CausationId { get; init; }

    /// <summary>
    /// Gets the timestamp in UTC when the message occurred.
    /// </summary>
    public DateTimeOffset OccurredOnUtc { get; init; }

    /// <summary>
    /// Gets the semantic version of the message schema.
    /// </summary>
    public string Version { get; init; }
  }
}
