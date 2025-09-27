namespace MicroShop.BuildingBlocks.Abstractions.Correlation
{
  public interface ICorrelationContextAccessor
  {
    CorrelationIds Current { get; set; }
  }
}
