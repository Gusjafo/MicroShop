using MicroShop.BuildingBlocks.Contracts.Messaging;

namespace MicroShop.BuildingBlocks.Abstractions.Messaging
{
  /// <summary>
  /// Wraps a message together with metadata for transport.
  /// </summary>
  /// <typeparam name="T">The message payload type.</typeparam>
  /// <param name="Message">The message payload.</param>
  /// <param name="Metadata">The metadata describing the message.</param>
  public sealed record MessageEnvelope<T>(
      T Message,
      MessageMetadata Metadata);
}

