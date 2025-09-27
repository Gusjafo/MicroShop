namespace MicroShop.Services.Identity.Application.DTOs;

/// <summary>
/// Represents the payload required to register a new user.
/// </summary>
/// <param name="Username">The desired username.</param>
/// <param name="Email">The email address.</param>
/// <param name="Password">The raw password.</param>
public sealed record RegisterUserRequest(string Username, string Email, string Password);
