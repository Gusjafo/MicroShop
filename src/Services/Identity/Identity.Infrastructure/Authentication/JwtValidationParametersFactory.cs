using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using MicroShop.Services.Identity.Infrastructure.Options;

namespace MicroShop.Services.Identity.Infrastructure.Authentication;

public static class JwtValidationParametersFactory
{
    public static TokenValidationParameters Create(JwtSettings settings)
    {
        var parameters = new TokenValidationParameters
        {
            ValidIssuer = settings.Issuer,
            ValidateIssuer = !string.IsNullOrWhiteSpace(settings.Issuer),
            ValidAudience = settings.Audience,
            ValidateAudience = !string.IsNullOrWhiteSpace(settings.Audience),
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.FromMinutes(1)
        };

        parameters.IssuerSigningKey = settings.Algorithm switch
        {
            "RS256" or "RS512" => new RsaSecurityKey(CreateRsa(settings.PublicKeyPem)),
            "ES256" or "ES384" or "ES512" => new ECDsaSecurityKey(CreateEcdsa(settings.PublicKeyPem)),
            _ => throw new InvalidOperationException($"Unsupported JWT algorithm '{settings.Algorithm}'.")
        };

        return parameters;
    }

    private static RSA CreateRsa(string pem)
    {
        var rsa = RSA.Create();
        rsa.ImportFromPem(pem);
        return rsa;
    }

    private static ECDsa CreateEcdsa(string pem)
    {
        var ecdsa = ECDsa.Create();
        ecdsa.ImportFromPem(pem);
        return ecdsa;
    }
}
