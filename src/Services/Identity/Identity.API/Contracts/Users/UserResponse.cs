namespace MicroShop.Services.Identity.API.Contracts.Users;

public sealed record UserResponse(Guid Id, string Username, string Email, bool Active, DateTimeOffset CreatedAt, IReadOnlyCollection<string> Roles);
