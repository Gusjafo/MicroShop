namespace MicroShop.Services.Identity.Application.DTOs;

/// <summary>
/// Represents the data required to authenticate a user.
/// </summary>
/// <param name="UsernameOrEmail">The username or email for the account.</param>
/// <param name="Password">The account password.</param>
public sealed record LoginRequest(string UsernameOrEmail, string Password);
