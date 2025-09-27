namespace MicroShop.BuildingBlocks.Abstractions.Common
{
  public sealed record PageRequest(int Page, int Size)
  {
    public int Skip => (Page < 1 ? 0 : (Page - 1) * Size);
  }

  public sealed record PagedResult<T>(IReadOnlyList<T> Items, int Page, int Size, long TotalCount);
}
