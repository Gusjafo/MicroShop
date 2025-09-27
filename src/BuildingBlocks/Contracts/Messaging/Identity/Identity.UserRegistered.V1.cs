using MicroShop.BuildingBlocks.Contracts.Messaging;

namespace MicroShop.BuildingBlocks.Contracts.Messaging.Identity;

/// <summary>
/// Integration event published when a new user is registered.
/// </summary>
public sealed record Identity_UserRegistered_V1(
    Guid UserId,
    string Username,
    string Email,
    DateTimeOffset RegisteredAt) : IIntegrationEvent;
