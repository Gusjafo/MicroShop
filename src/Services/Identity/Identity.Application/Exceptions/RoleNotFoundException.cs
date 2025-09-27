namespace MicroShop.Services.Identity.Application.Exceptions;

public sealed class RoleNotFoundException(string roleName) : IdentityException($"Role '{roleName}' was not found.");
