namespace MicroShop.BuildingBlocks.Abstractions.Common
{
  /// <summary>
  /// Describes pagination options for a resource collection.
  /// </summary>
  /// <param name="Page">The requested page number (1-based).</param>
  /// <param name="Size">The number of items requested per page.</param>
  public sealed record PageRequest(int Page, int Size)
  {
    /// <summary>
    /// Gets the number of items to skip when applying the pagination options.
    /// </summary>
    public int Skip => this.Page < 1 ? 0 : (this.Page - 1) * this.Size;
  }

  /// <summary>
  /// Represents a paginated response that includes items and paging metadata.
  /// </summary>
  /// <typeparam name="T">The type of the item contained in the page.</typeparam>
  /// <param name="Items">The items returned for the current page.</param>
  /// <param name="Page">The current page number (1-based).</param>
  /// <param name="Size">The size of the page.</param>
  /// <param name="TotalCount">The total number of available items.</param>
  public sealed record PagedResult<T>(IReadOnlyList<T> Items, int Page, int Size, long TotalCount);
}
