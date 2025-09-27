namespace MicroShop.Services.Identity.Application.Authentication.Commands;

public sealed record RegisterUserCommand(string Username, string Email, string Password);
