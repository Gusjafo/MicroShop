using MediatR;
using MicroShop.Services.Identity.Application.Abstractions;
using MicroShop.Services.Identity.Application.DTOs;
using MicroShop.Services.Identity.Application.Mapping;

namespace MicroShop.Services.Identity.Application.Users.Queries;

/// <summary>
/// Handles requests for retrieving users by identifier.
/// </summary>
public sealed class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDto?>
{
  private readonly IUserRepository userRepository;

  public GetUserByIdQueryHandler(IUserRepository userRepository)
  {
    this.userRepository = userRepository;
  }

  public async Task<UserDto?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
  {
    var user = await this.userRepository.GetByIdAsync(request.Id, cancellationToken).ConfigureAwait(false);
    return user?.ToDto();
  }
}
