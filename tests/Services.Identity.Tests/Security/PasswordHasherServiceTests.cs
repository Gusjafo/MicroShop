using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using MicroShop.Services.Identity.Application.Abstractions;
using MicroShop.Services.Identity.Domain.Entities;
using MicroShop.Services.Identity.Infrastructure.Security;
using Xunit;

namespace MicroShop.Services.Identity.Tests.Security;

public class PasswordHasherServiceTests
{
    [Fact]
    public void HashAndVerifyPassword_ShouldSucceed()
    {
        var passwordHasher = new PasswordHasher<User>();
        var service = new PasswordHasherService(passwordHasher);
        var user = new User("user-1", "alice", "alice@example.com", string.Empty, DateTimeOffset.UtcNow);
        var password = "Strong!Pass123";

        var hash = service.HashPassword(user, password);

        hash.Should().NotBeNullOrWhiteSpace();

        var result = service.VerifyHashedPassword(user, hash, password);

        result.Should().Be(PasswordVerificationResult.Success);
    }
}
