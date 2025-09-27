namespace MicroShop.Services.Identity.Application.Models;

public sealed record AuthTokens(string AccessToken, string RefreshToken, DateTimeOffset ExpiresAt);
