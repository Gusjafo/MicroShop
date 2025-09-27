using MicroShop.Services.Identity.Domain.Entities;

namespace MicroShop.Services.Identity.Application.Abstractions;

public interface IRoleRepository
{
    Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

    Task<Role?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Role>> GetAllAsync(CancellationToken cancellationToken = default);

    Task EnsureCreatedAsync(CancellationToken cancellationToken = default);
}
