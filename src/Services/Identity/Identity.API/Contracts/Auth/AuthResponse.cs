namespace MicroShop.Services.Identity.API.Contracts.Auth;

public sealed record AuthResponse(string AccessToken, string RefreshToken, DateTimeOffset ExpiresAt);
