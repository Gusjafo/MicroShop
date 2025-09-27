namespace MicroShop.BuildingBlocks.Contracts.Messaging
{
  public interface IIntegrationCommand
  {
    string CommandName => GetType().Name;
    string Version => "1";
  }
}
