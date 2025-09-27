using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MicroShop.Services.Identity.Application.Abstractions;
using MicroShop.Services.Identity.Application.Models;
using MicroShop.Services.Identity.Domain.Entities;
using MicroShop.Services.Identity.Infrastructure.Options;

namespace MicroShop.Services.Identity.Infrastructure.Authentication;

internal sealed class JwtTokenService : IJwtTokenService
{
    private readonly JwtSettings _settings;
    private readonly SigningCredentials _signingCredentials;

    public JwtTokenService(IOptions<JwtSettings> options)
    {
        _settings = options.Value;
        _signingCredentials = CreateSigningCredentials(_settings);
    }

    public AuthTokens IssueTokens(User user, IReadOnlyCollection<string> roles, IReadOnlyDictionary<string, string>? claims = null)
    {
        var now = DateTimeOffset.UtcNow;
        var expires = now.Add(_settings.AccessTokenLifetime);

        var jwtClaims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.UniqueName, user.Username),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new("security_stamp", user.SecurityStamp)
        };

        foreach (var role in roles)
        {
            jwtClaims.Add(new Claim(ClaimTypes.Role, role));
        }

        if (claims is not null)
        {
            foreach (var pair in claims)
            {
                jwtClaims.Add(new Claim(pair.Key, pair.Value));
            }
        }

        var jwtToken = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: jwtClaims,
            notBefore: now.UtcDateTime,
            expires: expires.UtcDateTime,
            signingCredentials: _signingCredentials);

        var handler = new JwtSecurityTokenHandler();
        var accessToken = handler.WriteToken(jwtToken);
        var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        return new AuthTokens(accessToken, refreshToken, expires);
    }

    private static SigningCredentials CreateSigningCredentials(JwtSettings settings)
    {
        return settings.Algorithm switch
        {
            "RS256" => new SigningCredentials(new RsaSecurityKey(CreateRsa(settings.PrivateKeyPem)), SecurityAlgorithms.RsaSha256),
            "RS512" => new SigningCredentials(new RsaSecurityKey(CreateRsa(settings.PrivateKeyPem)), SecurityAlgorithms.RsaSha512),
            "ES256" => new SigningCredentials(new ECDsaSecurityKey(CreateEcdsa(settings.PrivateKeyPem, ECCurve.NamedCurves.nistP256)), SecurityAlgorithms.EcdsaSha256),
            "ES384" => new SigningCredentials(new ECDsaSecurityKey(CreateEcdsa(settings.PrivateKeyPem, ECCurve.NamedCurves.nistP384)), SecurityAlgorithms.EcdsaSha384),
            "ES512" => new SigningCredentials(new ECDsaSecurityKey(CreateEcdsa(settings.PrivateKeyPem, ECCurve.NamedCurves.nistP521)), SecurityAlgorithms.EcdsaSha512),
            _ => throw new InvalidOperationException($"Unsupported JWT algorithm '{settings.Algorithm}'.")
        };
    }

    private static RSA CreateRsa(string privateKeyPem)
    {
        var rsa = RSA.Create();
        rsa.ImportFromPem(privateKeyPem);
        return rsa;
    }

    private static ECDsa CreateEcdsa(string privateKeyPem, ECCurve curve)
    {
        var ecdsa = ECDsa.Create(curve);
        ecdsa.ImportFromPem(privateKeyPem);
        return ecdsa;
    }
}
