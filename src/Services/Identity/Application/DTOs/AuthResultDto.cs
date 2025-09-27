namespace MicroShop.Services.Identity.Application.DTOs;

/// <summary>
/// Represents the result of a successful authentication request.
/// </summary>
/// <param name="AccessToken">The generated JSON Web Token.</param>
/// <param name="ExpiresAt">The expiration moment of the token.</param>
/// <param name="RefreshToken">An optional refresh token placeholder.</param>
public sealed record AuthResultDto(string AccessToken, DateTimeOffset ExpiresAt, string? RefreshToken);
