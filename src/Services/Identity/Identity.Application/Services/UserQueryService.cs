using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MicroShop.Services.Identity.Application.Abstractions;
using MicroShop.Services.Identity.Application.Models;
using MicroShop.Services.Identity.Domain.Entities;

namespace MicroShop.Services.Identity.Application.Services;

public sealed class UserQueryService(IUserRepository userRepository, IRoleRepository roleRepository) : IUserQueryService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IRoleRepository _roleRepository = roleRepository;

    public async Task<UserModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(id, cancellationToken);
        return user is null ? null : await ToModelAsync(user, cancellationToken);
    }

    public async Task<IReadOnlyCollection<UserModel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var users = await _userRepository.GetAllAsync(cancellationToken);
        var result = new List<UserModel>(users.Count);
        foreach (var user in users)
        {
            var model = await ToModelAsync(user, cancellationToken);
            result.Add(model);
        }

        return result;
    }

    private async Task<UserModel> ToModelAsync(User user, CancellationToken cancellationToken)
    {
        var roleNames = new List<string>();
        foreach (var assignment in user.Roles)
        {
            var role = await _roleRepository.GetByIdAsync(assignment.RoleId, cancellationToken);
            if (role is not null)
            {
                roleNames.Add(role.Name);
            }
        }

        return new UserModel(user.Id, user.Username, user.Email, user.Active, user.CreatedAt, roleNames);
    }
}
