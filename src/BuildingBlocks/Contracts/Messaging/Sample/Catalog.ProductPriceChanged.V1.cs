namespace MicroShop.BuildingBlocks.Contracts.Messaging.Sample
{
  /// <summary>
  /// Represents the integration event emitted when a catalog product price changes.
  /// </summary>
  public sealed record Catalog_ProductPriceChanged_V1 : Messaging.IIntegrationEvent
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="Catalog_ProductPriceChanged_V1"/> record.
    /// </summary>
    /// <param name="productId">The unique identifier of the product whose price changed.</param>
    /// <param name="oldPrice">The previous price of the product.</param>
    /// <param name="newPrice">The updated price of the product.</param>
    /// <param name="currency">The ISO currency code of the prices.</param>
    public Catalog_ProductPriceChanged_V1(Guid productId, decimal oldPrice, decimal newPrice, string currency)
    {
      ProductId = productId;
      OldPrice = oldPrice;
      NewPrice = newPrice;
      Currency = currency;
    }

    /// <summary>
    /// Gets the unique identifier of the product whose price changed.
    /// </summary>
    public Guid ProductId { get; init; }

    /// <summary>
    /// Gets the previous price of the product.
    /// </summary>
    public decimal OldPrice { get; init; }

    /// <summary>
    /// Gets the updated price of the product.
    /// </summary>
    public decimal NewPrice { get; init; }

    /// <summary>
    /// Gets the ISO currency code of the prices.
    /// </summary>
    public string Currency { get; init; }
  }
}
