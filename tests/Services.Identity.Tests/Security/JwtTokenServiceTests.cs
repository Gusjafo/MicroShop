using System.IdentityModel.Tokens.Jwt;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using MicroShop.BuildingBlocks.Abstractions.Time;
using MicroShop.Services.Identity.Application.Abstractions;
using MicroShop.Services.Identity.Domain.Entities;
using MicroShop.Services.Identity.Infrastructure.Security;
using Xunit;

namespace MicroShop.Services.Identity.Tests.Security;

public class JwtTokenServiceTests
{
    [Fact]
    public async Task CreateTokenAsync_ShouldEmitSignedToken()
    {
        using var tempDirectory = new TempDirectory();
        var options = Options.Create(new JwtOptions
        {
            Issuer = "MicroShop.Identity",
            Audience = "MicroShop",
            AccessTokenExpirationMinutes = 5,
            PrivateKeyPath = Path.Combine(tempDirectory.Path, "private.pem"),
            PublicKeyPath = Path.Combine(tempDirectory.Path, "public.pem")
        });

        IClock clock = new SystemClock();
        var service = new JwtTokenService(options, NullLogger<JwtTokenService>.Instance, clock);
        var user = new User("user-1", "alice", "alice@example.com", "hash", clock.UtcNow);
        user.AssignRole(Role.Create("role-1", DefaultRoles.Customer));

        var result = await service.CreateTokenAsync(user);

        result.AccessToken.Should().NotBeNullOrWhiteSpace();

        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(result.AccessToken);
        jwt.Issuer.Should().Be(options.Value.Issuer);
        jwt.Audiences.Should().Contain(options.Value.Audience);
        jwt.Claims.Should().Contain(claim => claim.Type == JwtRegisteredClaimNames.Sub && claim.Value == user.Id);
        jwt.Claims.Should().Contain(claim => claim.Type == "role" && claim.Value == DefaultRoles.Customer);
    }

    private sealed class TempDirectory : IDisposable
    {
        public TempDirectory()
        {
            Path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(Path);
        }

        public string Path { get; }

        public void Dispose()
        {
            if (Directory.Exists(Path))
            {
                Directory.Delete(Path, recursive: true);
            }
        }
    }
}
