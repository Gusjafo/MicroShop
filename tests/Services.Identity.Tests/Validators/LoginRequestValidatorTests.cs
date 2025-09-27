using FluentAssertions;
using MicroShop.Services.Identity.Application.DTOs;
using MicroShop.Services.Identity.Application.Users.Validators;
using Xunit;

namespace MicroShop.Services.Identity.Tests.Validators;

public sealed class LoginRequestValidatorTests
{
  private readonly LoginRequestValidator validator = new();

  [Fact]
  public void Validate_ShouldSucceedForValidPayload()
  {
    var request = new LoginRequest("validuser", "Password123!");

    var result = this.validator.Validate(request);

    result.IsValid.Should().BeTrue();
  }

  [Theory]
  [InlineData("", "Password123!")]
  [InlineData("validuser", "")]
  public void Validate_ShouldFailForMissingFields(string identifier, string password)
  {
    var request = new LoginRequest(identifier, password);

    var result = this.validator.Validate(request);

    result.IsValid.Should().BeFalse();
  }
}
