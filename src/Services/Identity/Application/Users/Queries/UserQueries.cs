using MicroShop.Services.Identity.Application.Abstractions;
using MicroShop.Services.Identity.Application.Users.Models;
using System.Linq;

namespace MicroShop.Services.Identity.Application.Users.Queries;

public sealed class UserQueries
{
    private readonly IUserRepository _userRepository;

    public UserQueries(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserDto?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.FindByIdAsync(id, cancellationToken);
        return user is null
            ? null
            : new UserDto(user.Id, user.Username, user.Email, user.Roles.Select(role => role.Name).ToArray(), user.CreatedAt);
    }

    public async Task<IReadOnlyCollection<UserDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var users = await _userRepository.GetAllAsync(cancellationToken);
        return users
            .Select(user => new UserDto(user.Id, user.Username, user.Email, user.Roles.Select(role => role.Name).ToArray(), user.CreatedAt))
            .OrderBy(user => user.Username, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }
}
