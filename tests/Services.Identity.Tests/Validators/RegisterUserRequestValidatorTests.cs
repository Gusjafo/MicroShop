using FluentAssertions;
using MicroShop.Services.Identity.Application.DTOs;
using MicroShop.Services.Identity.Application.Users.Validators;
using Xunit;

namespace MicroShop.Services.Identity.Tests.Validators;

public sealed class RegisterUserRequestValidatorTests
{
  private readonly RegisterUserRequestValidator validator = new();

  [Fact]
  public void Validate_ShouldSucceedForValidPayload()
  {
    var request = new RegisterUserRequest("validuser", "user@example.com", "Str0ng!Pass");

    var result = this.validator.Validate(request);

    result.IsValid.Should().BeTrue();
  }

  [Theory]
  [InlineData("us", "user@example.com", "Str0ng!Pass", "Username")] 
  [InlineData("user name", "user@example.com", "Str0ng!Pass", "Username")]
  [InlineData("validuser", "invalid-email", "Str0ng!Pass", "Email")]
  [InlineData("validuser", "user@example.com", "weakpass", "Password")]
  public void Validate_ShouldFailForInvalidPayload(string username, string email, string password, string expectedProperty)
  {
    var request = new RegisterUserRequest(username, email, password);

    var result = this.validator.Validate(request);

    result.IsValid.Should().BeFalse();
    result.Errors.Should().Contain(error => error.PropertyName == expectedProperty);
  }
}
