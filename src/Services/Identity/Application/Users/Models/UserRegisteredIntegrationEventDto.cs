namespace MicroShop.Services.Identity.Application.Users.Models;

public sealed record UserRegisteredIntegrationEventDto(string UserId, string Username, string Email, IReadOnlyCollection<string> Roles, DateTimeOffset RegisteredAt);
