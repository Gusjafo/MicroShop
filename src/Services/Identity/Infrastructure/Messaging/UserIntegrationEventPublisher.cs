using MassTransit;
using MicroShop.Services.Identity.Application.Abstractions;
using MicroShop.Services.Identity.Application.Users.Models;
using MicroShop.Services.Identity.Infrastructure.Messaging.Events;

namespace MicroShop.Services.Identity.Infrastructure.Messaging;

internal sealed class UserIntegrationEventPublisher(IBus bus) : IUserIntegrationEventPublisher
{
    public async Task PublishUserRegisteredAsync(UserRegisteredIntegrationEventDto payload, CancellationToken cancellationToken = default)
    {
        var message = new UserRegisteredIntegrationEventV1(payload.UserId, payload.Username, payload.Email, payload.Roles, payload.RegisteredAt);
        await bus.Publish(message, cancellationToken);
    }
}
