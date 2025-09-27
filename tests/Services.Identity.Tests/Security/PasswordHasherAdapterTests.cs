using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using MicroShop.Services.Identity.Domain.Users;
using MicroShop.Services.Identity.Infrastructure.Security;
using Xunit;

namespace MicroShop.Services.Identity.Tests.Security;

public sealed class PasswordHasherAdapterTests
{
  [Fact]
  public void HashPassword_ShouldGenerateDifferentHashForSamePassword()
  {
    var adapter = new PasswordHasherAdapter(new PasswordHasher<User>());
    var user = User.Create(Guid.NewGuid(), "alice", "alice@example.com", "temp", DateTimeOffset.UtcNow);
    var firstHash = adapter.HashPassword(user, "Strong#Password1");
    var secondHash = adapter.HashPassword(user, "Strong#Password1");

    firstHash.Should().NotBeNullOrWhiteSpace();
    secondHash.Should().NotBe(firstHash);
  }

  [Fact]
  public void VerifyHashedPassword_ShouldReturnTrueForValidPassword()
  {
    var adapter = new PasswordHasherAdapter(new PasswordHasher<User>());
    var user = User.Create(Guid.NewGuid(), "bob", "bob@example.com", "temp", DateTimeOffset.UtcNow);
    var hash = adapter.HashPassword(user, "Strong#Password1");

    var result = adapter.VerifyHashedPassword(user, hash, "Strong#Password1");

    result.Should().BeTrue();
  }

  [Fact]
  public void VerifyHashedPassword_ShouldReturnFalseForInvalidPassword()
  {
    var adapter = new PasswordHasherAdapter(new PasswordHasher<User>());
    var user = User.Create(Guid.NewGuid(), "carol", "carol@example.com", "temp", DateTimeOffset.UtcNow);
    var hash = adapter.HashPassword(user, "Strong#Password1");

    var result = adapter.VerifyHashedPassword(user, hash, "WrongPassword1!");

    result.Should().BeFalse();
  }
}
