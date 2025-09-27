using MediatR;
using MicroShop.BuildingBlocks.Abstractions.Common;
using MicroShop.Services.Identity.Application.Abstractions;
using MicroShop.Services.Identity.Application.DTOs;
using MicroShop.Services.Identity.Application.Mapping;

namespace MicroShop.Services.Identity.Application.Users.Queries;

/// <summary>
/// Handles paginated user listing requests.
/// </summary>
public sealed class ListUsersQueryHandler : IRequestHandler<ListUsersQuery, PagedResult<UserDto>>
{
  private readonly IUserRepository userRepository;

  public ListUsersQueryHandler(IUserRepository userRepository)
  {
    this.userRepository = userRepository;
  }

  public async Task<PagedResult<UserDto>> Handle(ListUsersQuery request, CancellationToken cancellationToken)
  {
    var pageRequest = new PageRequest(request.Page, request.Size);
    var users = await this.userRepository.ListAsync(pageRequest, cancellationToken).ConfigureAwait(false);
    var total = await this.userRepository.CountAsync(cancellationToken).ConfigureAwait(false);

    var dtos = users.Select(user => user.ToDto()).ToList();
    return new PagedResult<UserDto>(dtos, request.Page, request.Size, total);
  }
}
