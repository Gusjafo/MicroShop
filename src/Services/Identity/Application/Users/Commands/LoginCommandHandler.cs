using MediatR;
using MicroShop.Services.Identity.Application.Abstractions;
using MicroShop.Services.Identity.Application.DTOs;

namespace MicroShop.Services.Identity.Application.Users.Commands;

/// <summary>
/// Handles authentication attempts and issues JWTs upon successful validation.
/// </summary>
public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResultDto>
{
  private readonly IUserRepository userRepository;
  private readonly IPasswordHasher passwordHasher;
  private readonly IJwtService jwtService;

  public LoginCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher, IJwtService jwtService)
  {
    this.userRepository = userRepository;
    this.passwordHasher = passwordHasher;
    this.jwtService = jwtService;
  }

  public async Task<AuthResultDto> Handle(LoginCommand command, CancellationToken cancellationToken)
  {
    var request = command.Request;
    var identifier = request.UsernameOrEmail.Trim().ToLowerInvariant();

    var user = await this.userRepository.GetByUsernameAsync(identifier, cancellationToken).ConfigureAwait(false)
               ?? await this.userRepository.GetByEmailAsync(identifier, cancellationToken).ConfigureAwait(false)
               ?? throw new UnauthorizedAccessException("Invalid credentials.");

    if (!this.passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password))
    {
      throw new UnauthorizedAccessException("Invalid credentials.");
    }

    return this.jwtService.CreateToken(user, user.Roles.Select(role => role.Name));
  }
}
