using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MicroShop.BuildingBlocks.Abstractions.Time;
using MicroShop.Services.Identity.Application.Abstractions;
using MicroShop.Services.Identity.Application.DTOs;
using MicroShop.Services.Identity.Domain.Users;
using MicroShop.Services.Identity.Infrastructure.Configuration;

namespace MicroShop.Services.Identity.Infrastructure.Security;

internal sealed class JwtService : IJwtService
{
  private readonly JwtOptions options;
  private readonly IClock clock;
  private readonly SigningCredentials signingCredentials;
  private readonly JwtSecurityTokenHandler tokenHandler = new();

  public JwtService(IOptions<JwtOptions> options, IClock clock)
  {
    this.options = options.Value;
    this.clock = clock;

    if (string.IsNullOrWhiteSpace(this.options.PrivateKeyPath))
    {
      throw new InvalidOperationException("JWT private key path is not configured.");
    }

    if (!File.Exists(this.options.PrivateKeyPath))
    {
      throw new FileNotFoundException("JWT private key file not found.", this.options.PrivateKeyPath);
    }

    if (!string.IsNullOrWhiteSpace(this.options.PublicKeyPath) && !File.Exists(this.options.PublicKeyPath))
    {
      throw new FileNotFoundException("JWT public key file not found.", this.options.PublicKeyPath);
    }

    var (securityKey, algorithm) = LoadSecurityKey(this.options.PrivateKeyPath);
    this.signingCredentials = new SigningCredentials(securityKey, algorithm);
  }

  public AuthResultDto CreateToken(User user, IEnumerable<string> roles)
  {
    var now = this.clock.UtcNow;
    var expires = now.AddMinutes(Math.Max(1, this.options.ExpiresMinutes));

    var claims = new List<Claim>
    {
      new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
      new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
      new(JwtRegisteredClaimNames.Iat, now.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
      new(JwtRegisteredClaimNames.Email, user.Email),
      new(ClaimTypes.Name, user.Username)
    };

    foreach (var role in roles.Distinct(StringComparer.OrdinalIgnoreCase))
    {
      claims.Add(new Claim(ClaimTypes.Role, role));
    }

    var token = new JwtSecurityToken(
        issuer: this.options.Issuer,
        audience: this.options.Audience,
        claims: claims,
        notBefore: now.UtcDateTime,
        expires: expires.UtcDateTime,
        signingCredentials: this.signingCredentials);

    var accessToken = this.tokenHandler.WriteToken(token);
    return new AuthResultDto(accessToken, expires, null);
  }

  private static (SecurityKey Key, string Algorithm) LoadSecurityKey(string privateKeyPath)
  {
    var privateKeyPem = File.ReadAllText(privateKeyPath);

    try
    {
      var rsa = RSA.Create();
      rsa.ImportFromPem(privateKeyPem);
      var securityKey = new RsaSecurityKey(rsa)
      {
        KeyId = ComputeKeyId(rsa.ExportSubjectPublicKeyInfo())
      };
      return (securityKey, SecurityAlgorithms.RsaSha256);
    }
    catch (CryptographicException)
    {
      // Try ECDSA
    }

    var ecdsa = ECDsa.Create();
    ecdsa.ImportFromPem(privateKeyPem);
    var ecdsaKey = new ECDsaSecurityKey(ecdsa)
    {
      KeyId = ComputeKeyId(ecdsa.ExportSubjectPublicKeyInfo())
    };
    return (ecdsaKey, SecurityAlgorithms.EcdsaSha256);
  }

  private static string ComputeKeyId(byte[] publicKeyBytes)
  {
    using var sha256 = SHA256.Create();
    var hash = sha256.ComputeHash(publicKeyBytes);
    return Base64UrlEncoder.Encode(hash);
  }
}
