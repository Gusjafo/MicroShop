using FluentValidation;
using MicroShop.BuildingBlocks.Abstractions.IdGeneration;
using MicroShop.BuildingBlocks.Abstractions.Time;
using MicroShop.Services.Identity.Application.Abstractions;
using MicroShop.Services.Identity.Application.Authentication.Models;
using MicroShop.Services.Identity.Application.Users.Models;
using MicroShop.Services.Identity.Domain.Entities;
using System.Linq;

namespace MicroShop.Services.Identity.Application.Authentication.Commands;

public sealed class RegisterUserCommandHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IPasswordHasherService _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly IUserIntegrationEventPublisher _eventPublisher;
    private readonly IValidator<RegisterUserCommand> _validator;
    private readonly IIdGenerator _idGenerator;
    private readonly IClock _clock;

    public RegisterUserCommandHandler(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IPasswordHasherService passwordHasher,
        ITokenService tokenService,
        IUserIntegrationEventPublisher eventPublisher,
        IValidator<RegisterUserCommand> validator,
        IIdGenerator idGenerator,
        IClock clock)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _eventPublisher = eventPublisher;
        _validator = validator;
        _idGenerator = idGenerator;
        _clock = clock;
    }

    public async Task<AuthResultDto> HandleAsync(RegisterUserCommand command, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(command, cancellationToken);

        var existingByUsername = await _userRepository.FindByUsernameAsync(command.Username, cancellationToken);
        if (existingByUsername is not null)
        {
            throw new InvalidOperationException("Username already taken.");
        }

        var existingByEmail = await _userRepository.FindByEmailAsync(command.Email, cancellationToken);
        if (existingByEmail is not null)
        {
            throw new InvalidOperationException("Email already registered.");
        }

        var defaultRoles = await _roleRepository.GetDefaultRolesAsync(cancellationToken);
        if (defaultRoles.Count == 0)
        {
            throw new InvalidOperationException("No default roles are configured.");
        }

        var user = User.Create(
            _idGenerator,
            _clock,
            command.Username,
            command.Email,
            string.Empty,
            defaultRoles);

        var passwordHash = _passwordHasher.HashPassword(user, command.Password);
        user.UpdatePasswordHash(passwordHash);

        await _userRepository.AddAsync(user, cancellationToken);

        var token = await _tokenService.CreateTokenAsync(user, cancellationToken);

        var integrationEvent = new UserRegisteredIntegrationEventDto(
            user.Id,
            user.Username,
            user.Email,
            user.Roles.Select(role => role.Name).ToArray(),
            user.CreatedAt);

        await _eventPublisher.PublishUserRegisteredAsync(integrationEvent, cancellationToken);

        user.ClearDomainEvents();

        return token;
    }
}
