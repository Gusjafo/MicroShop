namespace MicroShop.Services.Identity.Application.Models;

public sealed record LoginRequest(string UsernameOrEmail, string Password);
