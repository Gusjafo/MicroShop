using System.Diagnostics;

namespace MicroShop.BuildingBlocks.Abstractions.Telemetry
{
  /// <summary>
  /// Provides helpers for emitting tracing information.
  /// </summary>
  public static class Tracing
  {
    /// <summary>
    /// The shared activity source used by the application.
    /// </summary>
    public static readonly ActivitySource Source = new("MicroShop");

    /// <summary>
    /// Starts a new activity with the provided name.
    /// </summary>
    /// <param name="name">The activity name.</param>
    /// <returns>The started <see cref="Activity"/>, or <c>null</c> if tracing is disabled.</returns>
    public static Activity? StartActivity(string name)
    {
      return Source.StartActivity(name, ActivityKind.Internal);
    }
  }
}
