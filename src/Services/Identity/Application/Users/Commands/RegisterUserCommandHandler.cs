using MediatR;
using MicroShop.BuildingBlocks.Abstractions.Correlation;
using MicroShop.BuildingBlocks.Abstractions.Messaging;
using MicroShop.BuildingBlocks.Abstractions.Persistence;
using MicroShop.BuildingBlocks.Abstractions.Time;
using MicroShop.BuildingBlocks.Contracts.Messaging;
using MicroShop.BuildingBlocks.Contracts.Messaging.Identity;
using MicroShop.Services.Identity.Application.Abstractions;
using MicroShop.Services.Identity.Application.DTOs;
using MicroShop.Services.Identity.Application.Mapping;
using MicroShop.Services.Identity.Domain.Users;

namespace MicroShop.Services.Identity.Application.Users.Commands;

/// <summary>
/// Handles the <see cref="RegisterUserCommand"/> by creating the user and publishing the integration event.
/// </summary>
public sealed class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, UserDto>
{
  private const string PlaceholderPasswordHash = "__placeholder__";
  private static readonly string[] DefaultRoles = ["Customer", "Admin"];

  private readonly IUserRepository userRepository;
  private readonly IRoleRepository roleRepository;
  private readonly IPasswordHasher passwordHasher;
  private readonly IUnitOfWork unitOfWork;
  private readonly IEventBus eventBus;
  private readonly IClock clock;
  private readonly ICorrelationContextAccessor correlationContextAccessor;

  public RegisterUserCommandHandler(
      IUserRepository userRepository,
      IRoleRepository roleRepository,
      IPasswordHasher passwordHasher,
      IUnitOfWork unitOfWork,
      IEventBus eventBus,
      IClock clock,
      ICorrelationContextAccessor correlationContextAccessor)
  {
    this.userRepository = userRepository;
    this.roleRepository = roleRepository;
    this.passwordHasher = passwordHasher;
    this.unitOfWork = unitOfWork;
    this.eventBus = eventBus;
    this.clock = clock;
    this.correlationContextAccessor = correlationContextAccessor;
  }

  public async Task<UserDto> Handle(RegisterUserCommand command, CancellationToken cancellationToken)
  {
    var request = command.Request;

    var username = Normalize(request.Username);
    var email = Normalize(request.Email);

    if (await this.userRepository.ExistsByUsernameOrEmailAsync(username, email, cancellationToken).ConfigureAwait(false))
    {
      throw new InvalidOperationException("A user with the same username or email already exists.");
    }

    var now = this.clock.UtcNow;
    var user = User.Create(Guid.NewGuid(), username, email, PlaceholderPasswordHash, now);
    var hashedPassword = this.passwordHasher.HashPassword(user, request.Password);
    user.SetPasswordHash(hashedPassword);

    await this.roleRepository.EnsureDefaultsAsync(DefaultRoles, cancellationToken).ConfigureAwait(false);
    var customerRole = await this.roleRepository.GetByNameAsync("Customer", cancellationToken).ConfigureAwait(false)
                       ?? throw new InvalidOperationException("The default Customer role is missing.");

    user.AssignRole(customerRole);
    await this.userRepository.AddAsync(user, cancellationToken).ConfigureAwait(false);
    await this.unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

    await this.PublishIntegrationEventAsync(user, cancellationToken).ConfigureAwait(false);
    user.ClearDomainEvents();

    return user.ToDto();
  }

  private async Task PublishIntegrationEventAsync(User user, CancellationToken cancellationToken)
  {
    var correlation = this.correlationContextAccessor.Current;
    var metadata = new MessageMetadata(
        Guid.NewGuid().ToString("N"),
        correlation.CorrelationId,
        correlation.CausationId,
        this.clock.UtcNow,
        version: "1");

    var integrationEvent = new UserRegisteredV1(user.Id.ToString(), user.Username, user.Email, metadata);
    var envelope = new MessageEnvelope<UserRegisteredV1>(integrationEvent, metadata);
    await this.eventBus.PublishAsync(envelope, cancellationToken).ConfigureAwait(false);
  }

  private static string Normalize(string value)
  {
    if (string.IsNullOrWhiteSpace(value))
    {
      throw new ArgumentException("Value cannot be empty.", nameof(value));
    }

    return value.Trim().ToLowerInvariant();
  }
}
