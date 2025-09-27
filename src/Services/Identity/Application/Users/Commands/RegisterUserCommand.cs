using MediatR;
using MicroShop.Services.Identity.Application.DTOs;

namespace MicroShop.Services.Identity.Application.Users.Commands;

/// <summary>
/// Command that triggers the registration of a new user.
/// </summary>
/// <param name="Request">The payload that contains the registration information.</param>
public sealed record RegisterUserCommand(RegisterUserRequest Request) : IRequest<UserDto>;
