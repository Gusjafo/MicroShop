using MicroShop.BuildingBlocks.Contracts.Messaging;

namespace MicroShop.BuildingBlocks.Abstractions.Messaging
{
  /// <summary>
  /// Defines operations for publishing integration events and commands.
  /// </summary>
  public interface IEventBus
  {
    /// <summary>
    /// Publishes an integration event to interested subscribers.
    /// </summary>
    /// <typeparam name="TEvent">The event payload type.</typeparam>
    /// <param name="envelope">The envelope containing the event and metadata.</param>
    /// <param name="ct">A cancellation token.</param>
    Task PublishAsync<TEvent>(MessageEnvelope<TEvent> envelope, CancellationToken ct = default)
          where TEvent : IIntegrationEvent;

    /// <summary>
    /// Sends an integration command to a single consumer.
    /// </summary>
    /// <typeparam name="TCommand">The command payload type.</typeparam>
    /// <param name="envelope">The envelope containing the command and metadata.</param>
    /// <param name="ct">A cancellation token.</param>
    Task SendAsync<TCommand>(MessageEnvelope<TCommand> envelope, CancellationToken ct = default)
          where TCommand : IIntegrationCommand;
  }
}
