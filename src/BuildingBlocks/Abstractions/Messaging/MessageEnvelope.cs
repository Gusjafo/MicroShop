using MicroShop.BuildingBlocks.Contracts.Messaging;

namespace MicroShop.BuildingBlocks.Abstractions.Messaging
{
  public sealed record MessageEnvelope<T>(
      T Message,
      MessageMetadata Metadata);
}

