namespace MicroShop.Services.Identity.Application.Users.Models;

public sealed record UserDto(string Id, string Username, string Email, IReadOnlyCollection<string> Roles, DateTimeOffset CreatedAt);
