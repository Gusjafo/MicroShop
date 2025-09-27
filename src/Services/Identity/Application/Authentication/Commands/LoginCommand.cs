namespace MicroShop.Services.Identity.Application.Authentication.Commands;

public sealed record LoginCommand(string UsernameOrEmail, string Password);
