namespace MicroShop.BuildingBlocks.Abstractions.Common
{
  public sealed record PageRequest(int Page, int Size)
  {
    public int Skip => this.Page < 1 ? 0 : (this.Page - 1) * this.Size;
  }

  public sealed record PagedResult<T>(IReadOnlyList<T> Items, int Page, int Size, long TotalCount);
}
