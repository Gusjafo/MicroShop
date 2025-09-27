namespace MicroShop.Services.Identity.Application.DTOs;

/// <summary>
/// Represents a user that can be exposed via the API.
/// </summary>
/// <param name="Id">The user identifier.</param>
/// <param name="Username">The username.</param>
/// <param name="Email">The email address.</param>
/// <param name="Roles">The roles associated with the user.</param>
/// <param name="CreatedAt">The creation timestamp.</param>
public sealed record UserDto(Guid Id, string Username, string Email, string[] Roles, DateTimeOffset CreatedAt);
