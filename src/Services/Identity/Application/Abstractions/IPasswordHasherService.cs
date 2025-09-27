using MicroShop.Services.Identity.Domain.Entities;

namespace MicroShop.Services.Identity.Application.Abstractions;

public interface IPasswordHasherService
{
    string HashPassword(User user, string password);

    PasswordVerificationResult VerifyHashedPassword(User user, string hashedPassword, string providedPassword);
}

public enum PasswordVerificationResult
{
    Failed = 0,
    Success = 1,
    SuccessRehashNeeded = 2
}
