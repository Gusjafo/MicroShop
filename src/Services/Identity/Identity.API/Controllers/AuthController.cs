using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MicroShop.Services.Identity.API.Contracts.Auth;
using MicroShop.Services.Identity.API.Contracts.Users;
using MicroShop.Services.Identity.Application.Abstractions;
using MicroShop.Services.Identity.Application.Models;
using ApplicationLoginRequest = MicroShop.Services.Identity.Application.Models.LoginRequest;

namespace MicroShop.Services.Identity.API.Controllers;

[ApiController]
[Route("")]
public sealed class AuthController(IIdentityService identityService) : ControllerBase
{
    private readonly IIdentityService _identityService = identityService;

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<UserResponse>> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        var model = new RegisterUserRequest(request.Username, request.Email, request.Password, request.Roles);
        var user = await _identityService.RegisterAsync(model, cancellationToken);
        return CreatedAtRoute("GetUserById", new { id = user.Id }, ToResponse(user));
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var model = new ApplicationLoginRequest(request.UsernameOrEmail, request.Password);
        var tokens = await _identityService.LoginAsync(model, cancellationToken);
        return Ok(new AuthResponse(tokens.AccessToken, tokens.RefreshToken, tokens.ExpiresAt));
    }

    private static UserResponse ToResponse(UserModel user)
    {
        return new UserResponse(user.Id, user.Username, user.Email, user.Active, user.CreatedAt, user.Roles);
    }
}
