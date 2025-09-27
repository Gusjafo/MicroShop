namespace MicroShop.Services.Identity.Application.Models;

public sealed record UserModel(Guid Id, string Username, string Email, bool Active, DateTimeOffset CreatedAt, IReadOnlyCollection<string> Roles);
