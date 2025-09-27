using MicroShop.BuildingBlocks.Contracts.Messaging;

namespace MicroShop.Services.Identity.Infrastructure.Messaging.Events;

public sealed record UserRegisteredIntegrationEventV1(
    string UserId,
    string Username,
    string Email,
    IReadOnlyCollection<string> Roles,
    DateTimeOffset RegisteredAt) : IIntegrationEvent;
