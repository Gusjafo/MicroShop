using MicroShop.BuildingBlocks.Contracts.Messaging;

namespace MicroShop.BuildingBlocks.Contracts.Messaging.Identity;

/// <summary>
/// Integration event emitted when a new user is registered.
/// </summary>
/// <param name="UserId">The identifier of the user.</param>
/// <param name="Username">The username of the newly registered user.</param>
/// <param name="Email">The email of the newly registered user.</param>
/// <param name="Metadata">Metadata that accompanies the message.</param>
public sealed record UserRegisteredV1(
    string UserId,
    string Username,
    string Email,
    MessageMetadata Metadata) : IIntegrationEvent;
