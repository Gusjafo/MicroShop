using MediatR;
using MicroShop.Services.Identity.Application.DTOs;

namespace MicroShop.Services.Identity.Application.Users.Commands;

/// <summary>
/// Command that validates user credentials and issues an access token.
/// </summary>
/// <param name="Request">The login payload.</param>
public sealed record LoginCommand(LoginRequest Request) : IRequest<AuthResultDto>;
