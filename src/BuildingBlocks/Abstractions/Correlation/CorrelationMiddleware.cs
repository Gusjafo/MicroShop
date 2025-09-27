using System.Diagnostics;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace MicroShop.BuildingBlocks.Abstractions.Correlation
{
  public sealed class CorrelationContextAccessor : ICorrelationContextAccessor
  {
    public CorrelationIds Current { get; set; } = new(null, null);
  }

  public static class CorrelationExtensions
  {
    public const string CorrelationHeader = "x-correlation-id";
    public const string CausationHeader = "x-causation-id";

    public static IServiceCollection AddCorrelationContext(this IServiceCollection services)
    {
      return services.AddSingleton<ICorrelationContextAccessor, CorrelationContextAccessor>();
    }

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
