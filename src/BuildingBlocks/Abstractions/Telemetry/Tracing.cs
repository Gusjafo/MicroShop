using System.Diagnostics;

namespace MicroShop.BuildingBlocks.Abstractions.Telemetry
{
  public static class Tracing
  {
    public static readonly ActivitySource Source = new("MicroShop");
    public static Activity? StartActivity(string name) => Source.StartActivity(name, ActivityKind.Internal);
  }
}
