using NUlid;

namespace MicroShop.BuildingBlocks.Abstractions.IdGeneration
{
  /// <summary>
  /// Generates ULID based unique identifiers.
  /// </summary>
  public sealed class UlidIdGenerator : IIdGenerator
  {
    /// <inheritdoc />
    public string NewId()
    {
      return Ulid.NewUlid().ToString();
    }
  }
}
