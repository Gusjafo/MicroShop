using Microsoft.AspNetCore.Identity;
using MicroShop.Services.Identity.Application.Abstractions;
using MicroShop.Services.Identity.Domain.Entities;

namespace MicroShop.Services.Identity.Infrastructure.Security;

internal sealed class PasswordHasherService : IPasswordHasherService
{
    private readonly IPasswordHasher<User> _passwordHasher;

    public PasswordHasherService(IPasswordHasher<User> passwordHasher)
    {
        _passwordHasher = passwordHasher;
    }

    public string HashPassword(User user, string password) => _passwordHasher.HashPassword(user, password);

    public PasswordVerificationResult VerifyHashedPassword(User user, string hashedPassword, string providedPassword)
        => _passwordHasher.VerifyHashedPassword(user, hashedPassword, providedPassword) switch
        {
            Microsoft.AspNetCore.Identity.PasswordVerificationResult.Success => PasswordVerificationResult.Success,
            Microsoft.AspNetCore.Identity.PasswordVerificationResult.SuccessRehashNeeded => PasswordVerificationResult.SuccessRehashNeeded,
            _ => PasswordVerificationResult.Failed
        };
}
