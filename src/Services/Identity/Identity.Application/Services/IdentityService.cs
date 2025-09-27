using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using MicroShop.Services.Identity.Application.Abstractions;
using MicroShop.Services.Identity.Application.Exceptions;
using MicroShop.Services.Identity.Application.Models;
using MicroShop.Services.Identity.Domain.Constants;
using MicroShop.Services.Identity.Domain.Entities;

namespace MicroShop.Services.Identity.Application.Services;

public sealed class IdentityService(
    IUserRepository userRepository,
    IRoleRepository roleRepository,
    IIdentityIntegrationEventPublisher integrationEventPublisher,
    IJwtTokenService jwtTokenService,
    IPasswordHasher<User> passwordHasher) : IIdentityService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IRoleRepository _roleRepository = roleRepository;
    private readonly IIdentityIntegrationEventPublisher _integrationEventPublisher = integrationEventPublisher;
    private readonly IJwtTokenService _jwtTokenService = jwtTokenService;
    private readonly IPasswordHasher<User> _passwordHasher = passwordHasher;

    public async Task<UserModel> RegisterAsync(RegisterUserRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var usernameNormalized = request.Username.Trim().ToLowerInvariant();
        var emailNormalized = request.Email.Trim().ToLowerInvariant();

        if (await _userRepository.GetByUsernameAsync(usernameNormalized, cancellationToken) is not null)
        {
            throw new UserAlreadyExistsException(request.Username);
        }

        if (await _userRepository.GetByEmailAsync(emailNormalized, cancellationToken) is not null)
        {
            throw new UserAlreadyExistsException(request.Email);
        }

        var user = new User(usernameNormalized, emailNormalized);
        var passwordHash = _passwordHasher.HashPassword(user, request.Password);
        user.SetPasswordHash(passwordHash);

        var requestedRoles = request.Roles?.Count > 0
            ? request.Roles
            : new[] { RoleNames.Customer };

        var roleNames = new List<string>();
        foreach (var roleName in requestedRoles!)
        {
            var role = await _roleRepository.GetByNameAsync(roleName, cancellationToken);
            if (role is null)
            {
                throw new RoleNotFoundException(roleName);
            }

            user.AssignRole(role);
            roleNames.Add(role.Name);
        }

        await _userRepository.AddAsync(user, cancellationToken);
        await _integrationEventPublisher.PublishUserRegisteredAsync(user, cancellationToken);

        return ToModel(user, roleNames);
    }

    public async Task<AuthTokens> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var identifier = request.UsernameOrEmail.Trim().ToLowerInvariant();
        var user = await _userRepository.GetByUsernameAsync(identifier, cancellationToken)
                   ?? await _userRepository.GetByEmailAsync(identifier, cancellationToken);

        if (user is null)
        {
            throw new InvalidCredentialsException();
        }

        if (_passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password) == PasswordVerificationResult.Failed)
        {
            throw new InvalidCredentialsException();
        }

        var roles = user.Roles.Select(r => r.RoleId).ToArray();
        var resolvedRoles = new List<string>(roles.Length);
        foreach (var roleId in roles)
        {
            var role = await _roleRepository.GetByIdAsync(roleId, cancellationToken);
            if (role is not null)
            {
                resolvedRoles.Add(role.Name);
            }
        }

        var tokens = _jwtTokenService.IssueTokens(user, resolvedRoles);
        return tokens;
    }

    private static UserModel ToModel(User user, IReadOnlyCollection<string>? roleNames = null)
    {
        roleNames ??= user.Roles.Select(r => r.RoleId.ToString()).ToArray();
        return new UserModel(user.Id, user.Username, user.Email, user.Active, user.CreatedAt, roleNames);
    }
}
