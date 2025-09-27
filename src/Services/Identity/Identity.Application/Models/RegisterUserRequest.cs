namespace MicroShop.Services.Identity.Application.Models;

public sealed record RegisterUserRequest(string Username, string Email, string Password, IReadOnlyCollection<string>? Roles = null);
