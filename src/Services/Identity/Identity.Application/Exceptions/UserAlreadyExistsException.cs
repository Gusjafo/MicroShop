namespace MicroShop.Services.Identity.Application.Exceptions;

public sealed class UserAlreadyExistsException : IdentityException
{
    public UserAlreadyExistsException(string identifier)
        : base($"User already exists with identifier '{identifier}'.")
    {
    }
}
