using MicroShop.BuildingBlocks.Abstractions.Common;
using MicroShop.BuildingBlocks.Abstractions.Correlation;
using MicroShop.BuildingBlocks.Abstractions.IdGeneration;
using MicroShop.BuildingBlocks.Abstractions.Messaging;
using MicroShop.BuildingBlocks.Abstractions.Telemetry;
using MicroShop.BuildingBlocks.Abstractions.Time;
using MicroShop.BuildingBlocks.Contracts.Messaging;
using MicroShop.BuildingBlocks.Contracts.Messaging.Sample;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCorrelationContext();
builder.Services.AddSingleton<IClock, SystemClock>();
builder.Services.AddSingleton<IIdGenerator, UlidIdGenerator>();
builder.Services.AddSingleton<IEventBus, InMemoryEventBus>(); // dummy

var app = builder.Build();
app.UseCorrelationContext();

app.MapGet("/ping", () => Results.Ok(new { status = "ok" }));

app.MapPost("/demo/price-change", async (
    Catalog_ProductPriceChanged_V1 body,
    IEventBus bus,
    IClock clock,
    IIdGenerator ids,
    ICorrelationContextAccessor corr) =>
{
  var meta = new MicroShop.BuildingBlocks.Contracts.Messaging.MessageMetadata(
      MessageId: ids.NewId(),
      CorrelationId: corr.Current.CorrelationId,
      CausationId: corr.Current.CausationId,
      OccurredOnUtc: clock.UtcNow,
      Version: "1");

  using var _ = Tracing.StartActivity("Publish PriceChanged");
  await bus.PublishAsync(new MessageEnvelope<Catalog_ProductPriceChanged_V1>(body, meta));
  return Results.Ok(Result.Ok());
});

app.Run();

internal sealed class InMemoryEventBus : IEventBus
{
  public Task PublishAsync<TEvent>(MessageEnvelope<TEvent> envelope, CancellationToken ct = default) where TEvent : IIntegrationEvent
  {
    Console.WriteLine($"[EVENT] {typeof(TEvent).Name} {envelope.Metadata.MessageId} corr={envelope.Metadata.CorrelationId}");
    return Task.CompletedTask;
  }

  public Task SendAsync<TCommand>(MessageEnvelope<TCommand> envelope, CancellationToken ct = default) where TCommand : IIntegrationCommand
  {
    Console.WriteLine($"[COMMAND] {typeof(TCommand).Name} {envelope.Metadata.MessageId} corr={envelope.Metadata.CorrelationId}");
    return Task.CompletedTask;
  }
}
