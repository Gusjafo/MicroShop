namespace MicroShop.BuildingBlocks.Contracts.Messaging.Sample
{
  public sealed record Catalog_ProductPriceChanged_V1(
      Guid ProductId,
      decimal OldPrice,
      decimal NewPrice,
      string Currency) : Messaging.IIntegrationEvent;
}
