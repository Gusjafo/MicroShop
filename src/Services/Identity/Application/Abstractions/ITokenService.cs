using MicroShop.Services.Identity.Application.Authentication.Models;
using MicroShop.Services.Identity.Domain.Entities;

namespace MicroShop.Services.Identity.Application.Abstractions;

public interface ITokenService
{
    Task<AuthResultDto> CreateTokenAsync(User user, CancellationToken cancellationToken = default);
}
