using MicroShop.Services.Identity.Domain.Entities;

namespace MicroShop.Services.Identity.Application.Abstractions;

public interface IRoleRepository
{
    Task<Role?> FindByNameAsync(string name, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Role>> GetDefaultRolesAsync(CancellationToken cancellationToken = default);
}
