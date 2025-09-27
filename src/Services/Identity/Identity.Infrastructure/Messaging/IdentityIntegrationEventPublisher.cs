using MassTransit;
using MicroShop.BuildingBlocks.Contracts.Messaging.Identity;
using MicroShop.Services.Identity.Application.Abstractions;
using MicroShop.Services.Identity.Domain.Entities;

namespace MicroShop.Services.Identity.Infrastructure.Messaging;

internal sealed class IdentityIntegrationEventPublisher(IPublishEndpoint publishEndpoint) : IIdentityIntegrationEventPublisher
{
    private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;

    public Task PublishUserRegisteredAsync(User user, CancellationToken cancellationToken = default)
    {
        var @event = new Identity_UserRegistered_V1(user.Id, user.Username, user.Email, user.CreatedAt);
        return _publishEndpoint.Publish(@event, cancellationToken);
    }
}
