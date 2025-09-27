namespace MicroShop.BuildingBlocks.Contracts.Messaging
{
  /// <summary>
  /// Defines the contract for integration commands dispatched between bounded contexts.
  /// </summary>
  public interface IIntegrationCommand
  {
    /// <summary>
    /// Gets the command name that uniquely identifies the integration command instance.
    /// </summary>
    string CommandName => GetType().Name;

    /// <summary>
    /// Gets the semantic version of the integration command definition.
    /// </summary>
    string Version => "1";
  }
}
