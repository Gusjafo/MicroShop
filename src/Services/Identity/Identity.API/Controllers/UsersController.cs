using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MicroShop.Services.Identity.API.Contracts.Users;
using MicroShop.Services.Identity.Application.Abstractions;

namespace MicroShop.Services.Identity.API.Controllers;

[ApiController]
[Authorize]
[Route("users")]
public sealed class UsersController(IUserQueryService userQueryService) : ControllerBase
{
    private readonly IUserQueryService _userQueryService = userQueryService;

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<UserResponse>>> GetUsers(CancellationToken cancellationToken)
    {
        var users = await _userQueryService.GetAllAsync(cancellationToken);
        var response = users.Select(u => new UserResponse(u.Id, u.Username, u.Email, u.Active, u.CreatedAt, u.Roles)).ToList();
        return Ok(response);
    }

    [HttpGet("{id:guid}", Name = "GetUserById")]
    public async Task<ActionResult<UserResponse>> GetUserById(Guid id, CancellationToken cancellationToken)
    {
        var user = await _userQueryService.GetByIdAsync(id, cancellationToken);
        if (user is null)
        {
            return NotFound();
        }

        return Ok(new UserResponse(user.Id, user.Username, user.Email, user.Active, user.CreatedAt, user.Roles));
    }
}
