using System.ComponentModel.DataAnnotations;

namespace MicroShop.Services.Identity.API.Contracts.Auth;

public sealed class LoginRequest
{
    [Required]
    public string UsernameOrEmail { get; init; } = string.Empty;

    [Required]
    public string Password { get; init; } = string.Empty;
}
