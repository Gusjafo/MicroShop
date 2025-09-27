using Microsoft.AspNetCore.Identity;
using MicroShop.Services.Identity.Domain.Entities;
using Xunit;

namespace MicroShop.Services.Identity.UnitTests.Authentication;

public class PasswordHasherTests
{
    [Fact]
    public void HashPassword_ShouldProduceVerifiableHash()
    {
        var user = new User("jane", "jane@example.com");
        var passwordHasher = new PasswordHasher<User>();

        var hash = passwordHasher.HashPassword(user, "P@ssw0rd!");

        var verificationResult = passwordHasher.VerifyHashedPassword(user, hash, "P@ssw0rd!");

        Assert.NotEqual(PasswordVerificationResult.Failed, verificationResult);
    }
}
