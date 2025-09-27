using System.Net.Http.Json;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using FluentAssertions;
using MicroShop.Services.Identity.IntegrationTests.Infrastructure;
using Xunit;

namespace MicroShop.Services.Identity.IntegrationTests;

public sealed class IdentityApiSmokeTests : IAsyncLifetime
{
    private readonly MsSqlTestcontainer _sqlContainer;
    private readonly RabbitMqTestcontainer _rabbitMqContainer;
    private IdentityApiFactory? _factory;
    private HttpClient? _client;

    private const string PrivateKeyPem = """
-----BEGIN RSA PRIVATE KEY-----
MIIEpAIBAAKCAQEAv1ISsf9kGYsyZVevZ4xQjYMNkj2r+2Z0smYZG5iGWl/8J6yl
r5hwC2sS+q1AV99nzbDYP4HRsl5cu8bSAoWlT+7NJo2TEHlnJbXu9j3YDsICsnoL
RjIptHbLLJg4IwXnJsv+bCIQyYrdPVV1scGV/Dc3fb9zdrBXAHYl1HuxDGxig4mA
UzLCuCSjCBNmdg9o3YkR44FUVnH+vypMDcqWBpCJbsxHyJauQF8P5S9lP2YkwS3R
BFpby47xS91Amkc0yFASv7tu+xK+lShAg/Ld4qOTjN0LqXuoYtIC9lIRq/XLwU81
UW41rS9CziXXg8xiXMJgJfZJrsKwjkwGwTU93QIDAQABAoIBAG/nCSrY5C9jxjK0
Jv17mqzADkTJ5XjuAy/vzFHc7Svdv2RUd+rh9rHk9R4CQ8R4ytAhBaPGnaPxh04U
KzJgq/NJPn4uWe6C2ixh9mX6jcth5rZ7+lwcpLxD2yWyv3vCG5v/Xu2k3SNP9i1D
YJChv7/9LPgK0DT0ioI23mdD5cbVd6LfTCrC0ftSpORP78Xo4IZM9RYEDu21y6Jx
2twGxirX9ju65boGqs2CvvSew2cEZeuv9bnvsrsSK92dFYOG2kjKqWp9CAQGH7dg
r2voYbcQF8Apt0SxRT1Fv6W1hDSOv8B9b4JjPWe6dJHVGs2p+kX+E0uKYEJep+eR
BDRffxECgYEA5miBq9b+HtrMXzALC6aSJy6o3czu2yeEcPT7x9pM2PjEQxMH32IF
h1m8MiQEmJvKFo0JZcg77u3xN6/A5uQBZXiHP0TgMyp9H6T4xRpRJnZ1wXk08r1i
P65ou0KtiI5mW0Nf+DyRo6+VhSC3ILg4F7+wWwYHMG4OuV0AOabQ0b0CgYEA0+Ov
FUKaPjRgW4FJqED1EuU+xiqmJjRdwG30X4ey0c3mVg9zMZZl35x5VDcS5JDkDkhB
6hFUmZXn8GQ5Cz7Qby4W/wIuF3eCeEr1X497q7bg8uIWAA0wBgOQhohpn4i7+8/y
GGu42SeVk7ohRg1Pz41t3uxo3gpKtXgKhXghMI0CgYEAnYUdVj4O0e4C/XC6F/Vz
4SxRPRGWsBiWX5oV7TFxq7iHS38inRrO6Z9BWaS7SxBGg+NG+96JIL6yTjOFXZyF
stIXPohV1Yrn3dR68CYgJk1p85fNCI3yMhJYof/YvmqtEf5KXG3H57Y3NM2O2gu3
uZFF+jndZm8mw7Q6xRIMjGUCgYB2/CdC7yTbwMnQuFUYwjje3cv1xwpPD6S1AZ0Z
kKOxCEPf1L8rS+T4zTtePZloCcLzK2ul6+aKuMNjCgnvQRfXCEyt6HndWjSxmpZk
9Pvy7vJ1b5wtmEeKCwtgmAHhBk7Xud6Chv90chP3CakL8/nexuPsPq+5SN4E7nDN
zvHZpQKBgQDUIuqnnGkgSxvPbJwsLRHL8cPzZLMndR6FZLfeYi7dBok/3UcH3KxR
2w8kgdJ+xjQBmXG1Mc9HuKvrAwXHXGP7JFrRo+/PIhH9Qz2p80qkVBg3pBR87z3J
zow5TsKQVMy9AHg1aGJ6XC3v1lAJt8iF9AuRb3gpgE2B/Qp77srI3A==
-----END RSA PRIVATE KEY-----
""";

    private const string PublicKeyPem = """
-----BEGIN PUBLIC KEY-----
MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAv1ISsf9kGYsyZVevZ4xQ
jYMNkj2r+2Z0smYZG5iGWl/8J6ylr5hwC2sS+q1AV99nzbDYP4HRsl5cu8bSAoWl
T+7NJo2TEHlnJbXu9j3YDsICsnoLRjIptHbLLJg4IwXnJsv+bCIQyYrdPVV1scGV
/Dc3fb9zdrBXAHYl1HuxDGxig4mAUzLCuCSjCBNmdg9o3YkR44FUVnH+vypMDcqW
BpCJbsxHyJauQF8P5S9lP2YkwS3RBFpby47xS91Amkc0yFASv7tu+xK+lShAg/Ld
4qOTjN0LqXuoYtIC9lIRq/XLwU81UW41rS9CziXXg8xiXMJgJfZJrsKwjkwGwTU9
3QIDAQAB
-----END PUBLIC KEY-----
""";

    public IdentityApiSmokeTests()
    {
        _sqlContainer = new TestcontainersBuilder<MsSqlTestcontainer>()
            .WithDatabase(new MsSqlTestcontainerConfiguration
            {
                Password = "Str0ngP@ssw0rd!"
            })
            .Build();

        _rabbitMqContainer = new TestcontainersBuilder<RabbitMqTestcontainer>()
            .WithImage("rabbitmq:3.13-management")
            .WithUsername("guest")
            .WithPassword("guest")
            .Build();
    }

    public async Task InitializeAsync()
    {
        await _sqlContainer.StartAsync();
        await _rabbitMqContainer.StartAsync();

        _factory = new IdentityApiFactory(
            _sqlContainer.ConnectionString,
            _rabbitMqContainer.Hostname,
            _rabbitMqContainer.Port,
            _rabbitMqContainer.Username,
            _rabbitMqContainer.Password,
            PrivateKeyPem,
            PublicKeyPem);

        _client = _factory.CreateClient();
    }

    public async Task DisposeAsync()
    {
        if (_client is not null)
        {
            _client.Dispose();
        }

        if (_factory is not null)
        {
            _factory.Dispose();
        }

        await _rabbitMqContainer.DisposeAsync();
        await _sqlContainer.DisposeAsync();
    }

    [Fact]
    public async Task RegisterAndLogin_ShouldReturnTokens()
    {
        var registerPayload = new
        {
            Username = "integration-user",
            Email = "integration@example.com",
            Password = "P@ssw0rd!"
        };

        var registerResponse = await _client!.PostAsJsonAsync("register", registerPayload);
        registerResponse.EnsureSuccessStatusCode();

        var loginPayload = new
        {
            UsernameOrEmail = "integration@example.com",
            Password = "P@ssw0rd!"
        };

        var loginResponse = await _client.PostAsJsonAsync("login", loginPayload);
        loginResponse.EnsureSuccessStatusCode();

        var tokens = await loginResponse.Content.ReadFromJsonAsync<AuthResponseDto>();
        tokens.Should().NotBeNull();
        tokens!.AccessToken.Should().NotBeNullOrWhiteSpace();
        tokens.RefreshToken.Should().NotBeNullOrWhiteSpace();
    }

    private sealed record AuthResponseDto(string AccessToken, string RefreshToken, DateTimeOffset ExpiresAt);
}
