namespace MicroShop.BuildingBlocks.Abstractions.IdGeneration
{
  /// <summary>
  /// Provides a mechanism for generating unique identifiers.
  /// </summary>
  public interface IIdGenerator
  {
    /// <summary>
    /// Creates a new unique identifier.
    /// </summary>
    /// <returns>A new identifier string.</returns>
    string NewId();
  }
}
