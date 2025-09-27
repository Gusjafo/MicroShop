using MicroShop.BuildingBlocks.Contracts.Messaging;

namespace MicroShop.BuildingBlocks.Abstractions.Messaging
{
  public interface IEventBus
  {
    Task PublishAsync<TEvent>(MessageEnvelope<TEvent> envelope, CancellationToken ct = default)
          where TEvent : IIntegrationEvent;

    Task SendAsync<TCommand>(MessageEnvelope<TCommand> envelope, CancellationToken ct = default)
          where TCommand : IIntegrationCommand;
  }
}
