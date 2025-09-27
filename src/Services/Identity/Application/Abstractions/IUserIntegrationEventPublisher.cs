using MicroShop.Services.Identity.Application.Users.Models;

namespace MicroShop.Services.Identity.Application.Abstractions;

public interface IUserIntegrationEventPublisher
{
    Task PublishUserRegisteredAsync(UserRegisteredIntegrationEventDto payload, CancellationToken cancellationToken = default);
}
