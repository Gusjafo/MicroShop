using NUlid;

namespace MicroShop.BuildingBlocks.Abstractions.IdGeneration
{
  public sealed class UlidIdGenerator : IIdGenerator
  {
    public string NewId()
    {
      return Ulid.NewUlid().ToString();
    }
  }
}
