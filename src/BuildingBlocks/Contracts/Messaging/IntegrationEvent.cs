namespace MicroShop.BuildingBlocks.Contracts.Messaging
{
  /// <summary>
  /// Represents the contract implemented by integration events that flow across services.
  /// </summary>
  public interface IIntegrationEvent
  {
    /// <summary>
    /// Gets the event name that uniquely identifies the integration event instance.
    /// </summary>
    string MessageName => GetType().Name;

    /// <summary>
    /// Gets the semantic version of the integration event definition.
    /// </summary>
    string Version => "1";
  }
}
