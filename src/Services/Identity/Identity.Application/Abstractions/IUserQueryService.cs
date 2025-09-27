using MicroShop.Services.Identity.Application.Models;

namespace MicroShop.Services.Identity.Application.Abstractions;

public interface IUserQueryService
{
    Task<UserModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<UserModel>> GetAllAsync(CancellationToken cancellationToken = default);
}
