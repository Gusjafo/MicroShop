namespace MicroShop.Services.Identity.Application.Authentication.Models;

public sealed record AuthResultDto(string AccessToken, DateTimeOffset ExpiresAt, string TokenType = "Bearer");
