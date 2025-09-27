namespace MicroShop.BuildingBlocks.Contracts.Messaging
{
  public interface IIntegrationEvent
  {
    string MessageName => GetType().Name;
    string Version => "1";
  }

}
