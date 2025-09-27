namespace MicroShop.Services.Identity.Application.Exceptions;

public sealed class InvalidCredentialsException() : IdentityException("Invalid username/email or password.");
