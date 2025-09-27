using System.Text.Json;
using MassTransit;
using MicroShop.BuildingBlocks.Abstractions.Messaging;
using MicroShop.BuildingBlocks.Contracts.Messaging;

namespace MicroShop.Services.Identity.Infrastructure.Messaging;

internal sealed class MassTransitEventBus : IEventBus
{
  private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

  private readonly IPublishEndpoint publishEndpoint;
  private readonly ISendEndpointProvider sendEndpointProvider;

  public MassTransitEventBus(IPublishEndpoint publishEndpoint, ISendEndpointProvider sendEndpointProvider)
  {
    this.publishEndpoint = publishEndpoint;
    this.sendEndpointProvider = sendEndpointProvider;
  }

  public async Task PublishAsync<TEvent>(MessageEnvelope<TEvent> envelope, CancellationToken ct = default)
      where TEvent : IIntegrationEvent
  {
    await this.publishEndpoint.Publish(envelope.Message, context =>
    {
      ApplyMetadata(envelope.Metadata, context);
    }, ct).ConfigureAwait(false);
  }

  public async Task SendAsync<TCommand>(MessageEnvelope<TCommand> envelope, CancellationToken ct = default)
      where TCommand : IIntegrationCommand
  {
    var endpoint = await this.sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{typeof(TCommand).Name}"))
        .ConfigureAwait(false);

    await endpoint.Send(envelope.Message, context =>
    {
      ApplyMetadata(envelope.Metadata, context);
    }, ct).ConfigureAwait(false);
  }

  private static void ApplyMetadata(MessageMetadata metadata, SendContext context)
  {
    if (Guid.TryParse(metadata.MessageId, out var messageId))
    {
      context.MessageId = messageId;
    }
    else if (!string.IsNullOrWhiteSpace(metadata.MessageId))
    {
      context.Headers.Set("message-id", metadata.MessageId);
    }

    if (!string.IsNullOrWhiteSpace(metadata.CorrelationId) && Guid.TryParse(metadata.CorrelationId, out var correlationId))
    {
      context.CorrelationId = correlationId;
    }

    if (!string.IsNullOrWhiteSpace(metadata.CausationId) && Guid.TryParse(metadata.CausationId, out var causationId))
    {
      context.Headers.Set("causation-id", causationId);
    }

    context.Headers.Set("occurred-on-utc", metadata.OccurredOnUtc.ToString("O"));
    context.Headers.Set("schema-version", metadata.Version);
    context.Headers.Set("metadata", JsonSerializer.Serialize(metadata, SerializerOptions));
  }
}
