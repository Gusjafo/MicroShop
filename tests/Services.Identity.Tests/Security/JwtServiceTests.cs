using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MicroShop.BuildingBlocks.Abstractions.Time;
using MicroShop.Services.Identity.Domain.Users;
using MicroShop.Services.Identity.Infrastructure.Configuration;
using MicroShop.Services.Identity.Infrastructure.Security;
using Xunit;

namespace MicroShop.Services.Identity.Tests.Security;

public sealed class JwtServiceTests
{
  [Fact]
  public void CreateToken_ShouldGenerateSignedJwt()
  {
    using var rsa = RSA.Create(2048);
    using var tempDirectory = Directory.CreateTempSubdirectory();
    var privateKeyPath = Path.Combine(tempDirectory.FullName, "private.key");
    var publicKeyPath = Path.Combine(tempDirectory.FullName, "public.key");
    File.WriteAllText(privateKeyPath, rsa.ExportRSAPrivateKeyPem());
    File.WriteAllText(publicKeyPath, rsa.ExportRSAPublicKeyPem());

    var options = Options.Create(new JwtOptions
    {
      Issuer = "MicroShop.Identity",
      Audience = "MicroShop.Client",
      ExpiresMinutes = 30,
      PrivateKeyPath = privateKeyPath,
      PublicKeyPath = publicKeyPath
    });

    var now = DateTimeOffset.UtcNow;
    var clock = new FixedClock(now);
    var service = new JwtService(options, clock);

    var user = User.Create(Guid.NewGuid(), "testuser", "test@example.com", "temp", now);
    var role = Role.Create(Guid.NewGuid(), "Admin");
    user.AssignRole(role);

    var result = service.CreateToken(user, user.Roles.Select(r => r.Name));

    result.Should().NotBeNull();
    result.AccessToken.Should().NotBeNullOrWhiteSpace();
    result.ExpiresAt.Should().BeAfter(now);

    var handler = new JwtSecurityTokenHandler();
    var validationParameters = new TokenValidationParameters
    {
      ValidateAudience = true,
      ValidAudience = options.Value.Audience,
      ValidateIssuer = true,
      ValidIssuer = options.Value.Issuer,
      ValidateLifetime = true,
      ValidateIssuerSigningKey = true,
      IssuerSigningKey = LoadPublicKey(publicKeyPath),
      ClockSkew = TimeSpan.Zero
    };

    var principal = handler.ValidateToken(result.AccessToken, validationParameters, out _);
    principal.Claims.Should().Contain(claim => claim.Type == ClaimTypes.Name && claim.Value == "testuser");
    principal.Claims.Should().Contain(claim => claim.Type == ClaimTypes.Role && claim.Value == "Admin");
  }

  [Fact]
  public void Constructor_ShouldThrowWhenPrivateKeyIsMissing()
  {
    var options = Options.Create(new JwtOptions
    {
      PrivateKeyPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString(), "missing.key"),
      PublicKeyPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString(), "missing.pub")
    });

    var clock = new FixedClock(DateTimeOffset.UtcNow);
    var act = () => new JwtService(options, clock);

    act.Should().Throw<FileNotFoundException>();
  }

  private static SecurityKey LoadPublicKey(string path)
  {
    var pem = File.ReadAllText(path);
    var rsa = RSA.Create();
    rsa.ImportFromPem(pem);
    return new RsaSecurityKey(rsa);
  }

  private sealed class FixedClock : IClock
  {
    public FixedClock(DateTimeOffset now)
    {
      this.UtcNow = now;
    }

    public DateTimeOffset UtcNow { get; }
  }
}
