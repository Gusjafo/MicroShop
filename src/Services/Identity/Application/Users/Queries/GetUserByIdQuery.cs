using MediatR;
using MicroShop.Services.Identity.Application.DTOs;

namespace MicroShop.Services.Identity.Application.Users.Queries;

/// <summary>
/// Retrieves a user by its identifier.
/// </summary>
/// <param name="Id">The identifier of the user.</param>
public sealed record GetUserByIdQuery(Guid Id) : IRequest<UserDto?>;
