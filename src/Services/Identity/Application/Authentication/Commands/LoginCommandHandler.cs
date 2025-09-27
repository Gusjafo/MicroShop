using FluentValidation;
using MicroShop.Services.Identity.Application.Abstractions;
using MicroShop.Services.Identity.Application.Authentication.Models;
using MicroShop.Services.Identity.Domain.Entities;

namespace MicroShop.Services.Identity.Application.Authentication.Commands;

public sealed class LoginCommandHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasherService _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly IValidator<LoginCommand> _validator;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IPasswordHasherService passwordHasher,
        ITokenService tokenService,
        IValidator<LoginCommand> validator)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _validator = validator;
    }

    public async Task<AuthResultDto> HandleAsync(LoginCommand command, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(command, cancellationToken);

        User? user = await _userRepository.FindByUsernameAsync(command.UsernameOrEmail, cancellationToken);
        if (user is null && command.UsernameOrEmail.Contains('@', StringComparison.Ordinal))
        {
            user = await _userRepository.FindByEmailAsync(command.UsernameOrEmail, cancellationToken);
        }

        if (user is null)
        {
            throw new UnauthorizedAccessException("Invalid credentials.");
        }

        var verification = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, command.Password);
        if (verification == PasswordVerificationResult.Failed)
        {
            throw new UnauthorizedAccessException("Invalid credentials.");
        }

        if (verification == PasswordVerificationResult.SuccessRehashNeeded)
        {
            var newHash = _passwordHasher.HashPassword(user, command.Password);
            user.UpdatePasswordHash(newHash);
            await _userRepository.UpdateAsync(user, cancellationToken);
        }

        return await _tokenService.CreateTokenAsync(user, cancellationToken);
    }
}
