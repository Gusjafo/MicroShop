namespace MicroShop.BuildingBlocks.Abstractions.Common
{
  public sealed record Error(string Code, string Message)
  {
    public static Error None => new("", "");
    public override string ToString()
    {
      return $"{this.Code}: {this.Message}";
    }
  }
}
