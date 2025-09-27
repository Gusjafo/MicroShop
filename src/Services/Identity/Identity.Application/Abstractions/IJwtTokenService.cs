using MicroShop.Services.Identity.Application.Models;
using MicroShop.Services.Identity.Domain.Entities;

namespace MicroShop.Services.Identity.Application.Abstractions;

public interface IJwtTokenService
{
    AuthTokens IssueTokens(User user, IReadOnlyCollection<string> roles, IReadOnlyDictionary<string, string>? claims = null);
}
