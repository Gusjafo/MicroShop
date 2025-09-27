using MediatR;
using MicroShop.BuildingBlocks.Abstractions.Common;
using MicroShop.Services.Identity.Application.DTOs;

namespace MicroShop.Services.Identity.Application.Users.Queries;

/// <summary>
/// Retrieves a paginated list of users.
/// </summary>
/// <param name="Page">The requested page number.</param>
/// <param name="Size">The number of items per page.</param>
public sealed record ListUsersQuery(int Page, int Size) : IRequest<PagedResult<UserDto>>;
