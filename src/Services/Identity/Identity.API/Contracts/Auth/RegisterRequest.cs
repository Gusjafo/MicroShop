using System.ComponentModel.DataAnnotations;

namespace MicroShop.Services.Identity.API.Contracts.Auth;

public sealed class RegisterRequest
{
    [Required]
    [MinLength(3)]
    public string Username { get; init; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; init; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string Password { get; init; } = string.Empty;

    public IReadOnlyCollection<string>? Roles { get; init; }
}
