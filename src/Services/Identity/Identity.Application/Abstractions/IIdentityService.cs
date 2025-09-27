using MicroShop.Services.Identity.Application.Models;

namespace MicroShop.Services.Identity.Application.Abstractions;

public interface IIdentityService
{
    Task<UserModel> RegisterAsync(RegisterUserRequest request, CancellationToken cancellationToken = default);

    Task<AuthTokens> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
}
