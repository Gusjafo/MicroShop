using System.Diagnostics;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace MicroShop.BuildingBlocks.Abstractions.Correlation
{
  /// <summary>
  /// Provides access to the current <see cref="CorrelationIds"/> for the active request.
  /// </summary>
  public sealed class CorrelationContextAccessor : ICorrelationContextAccessor
  {
    /// <summary>
    /// Gets or sets the current correlation context.
    /// </summary>
    public CorrelationIds Current { get; set; } = new(null, null);
  }

  /// <summary>
  /// Extension methods for registering and using correlation context services.
  /// </summary>
  public static class CorrelationExtensions
  {
    /// <summary>
    /// The HTTP header used to transport the correlation identifier.
    /// </summary>
    public const string CorrelationHeader = "x-correlation-id";

    /// <summary>
    /// The HTTP header used to transport the causation identifier.
    /// </summary>
    public const string CausationHeader = "x-causation-id";

    /// <summary>
    /// Registers the correlation context accessor in the service collection.
    /// </summary>
    /// <param name="services">The application service collection.</param>
    /// <returns>The original service collection for chaining.</returns>
    public static IServiceCollection AddCorrelationContext(this IServiceCollection services)
    {
      return services.AddSingleton<ICorrelationContextAccessor, CorrelationContextAccessor>();
    }

    /// <summary>
    /// Adds middleware that ensures correlation identifiers are available for requests.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <returns>The original application builder for chaining.</returns>
    public static IApplicationBuilder UseCorrelationContext(this IApplicationBuilder app)
    {
      return app.Use(async (ctx, next) =>
            {
              var acc = ctx.RequestServices.GetRequiredService<ICorrelationContextAccessor>();
              var corr = ctx.Request.Headers[CorrelationHeader].ToString();
              var caus = ctx.Request.Headers[CausationHeader].ToString();

              if (string.IsNullOrWhiteSpace(corr))
              { corr = Activity.Current?.TraceId.ToString() ?? Guid.NewGuid().ToString("N"); }

              acc.Current = new(corr, string.IsNullOrWhiteSpace(caus) ? null : caus);
              ctx.Response.Headers[CorrelationHeader] = corr;

              await next();
            });
    }
  }
}
