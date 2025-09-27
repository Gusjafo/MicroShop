using System.Net;
using System.Net.Http.Json;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using MicroShop.Services.Identity.Application.Authentication.Commands;
using MicroShop.Services.Identity.Application.Authentication.Models;
using MicroShop.Services.Identity.Application.Users.Models;
using Xunit;

namespace MicroShop.Services.Identity.Tests.Integration;

[CollectionDefinition(nameof(IdentityApiCollection))]
public sealed class IdentityApiCollection : ICollectionFixture<IdentityApiFixture>;

public sealed class IdentityApiTests(IdentityApiFixture fixture)
{
    private readonly HttpClient _client = fixture.Client;

    [Fact]
    public async Task Register_Login_ListUsers_ShouldWork()
    {
        var registerCommand = new RegisterUserCommand("integration-user", "integration@example.com", "Strong!Pass123");
        var registerResponse = await _client.PostAsJsonAsync("/register", registerCommand);

        registerResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var registerResult = await registerResponse.Content.ReadFromJsonAsync<AuthResultDto>();
        registerResult.Should().NotBeNull();
        registerResult!.AccessToken.Should().NotBeNullOrWhiteSpace();

        var loginCommand = new LoginCommand(registerCommand.Username, registerCommand.Password);
        var loginResponse = await _client.PostAsJsonAsync("/login", loginCommand);
        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<AuthResultDto>();
        loginResult.Should().NotBeNull();
        loginResult!.AccessToken.Should().NotBeNullOrWhiteSpace();

        var users = await _client.GetFromJsonAsync<IReadOnlyCollection<UserDto>>("/users");
        users.Should().NotBeNull();
        var registeredUser = users!.Single(user => user.Username == registerCommand.Username);

        var user = await _client.GetFromJsonAsync<UserDto>($"/users/{registeredUser.Id}");
        user.Should().NotBeNull();
        user!.Email.Should().Be(registerCommand.Email);
    }
}
