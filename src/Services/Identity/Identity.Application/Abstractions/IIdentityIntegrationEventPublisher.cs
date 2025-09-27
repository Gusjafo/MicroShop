using MicroShop.BuildingBlocks.Contracts.Messaging.Identity;
using MicroShop.Services.Identity.Domain.Entities;

namespace MicroShop.Services.Identity.Application.Abstractions;

public interface IIdentityIntegrationEventPublisher
{
    Task PublishUserRegisteredAsync(User user, CancellationToken cancellationToken = default);
}
