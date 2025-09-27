using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MicroShop.BuildingBlocks.Abstractions.Time;
using MicroShop.Services.Identity.Application.Abstractions;
using MicroShop.Services.Identity.Application.Authentication.Models;
using MicroShop.Services.Identity.Domain.Entities;

namespace MicroShop.Services.Identity.Infrastructure.Security;

internal sealed class JwtTokenService : ITokenService
{
    private readonly JwtSecurityTokenHandler _tokenHandler = new();
    private readonly JwtOptions _options;
    private readonly ILogger<JwtTokenService> _logger;
    private readonly IClock _clock;
    private readonly SemaphoreSlim _keySemaphore = new(1, 1);
    private SecurityKey? _signingKey;

    public JwtTokenService(IOptions<JwtOptions> options, ILogger<JwtTokenService> logger, IClock clock)
    {
        _options = options.Value;
        _logger = logger;
        _clock = clock;
    }

    public async Task<AuthResultDto> CreateTokenAsync(User user, CancellationToken cancellationToken = default)
    {
        await EnsureSigningKeyAsync(cancellationToken);

        if (_signingKey is null)
        {
            throw new InvalidOperationException("Signing key is not available.");
        }

        var expires = _clock.UtcNow.AddMinutes(_options.AccessTokenExpirationMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.UniqueName, user.Username),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        claims.AddRange(user.Roles.Select(role => new Claim(ClaimTypes.Role, role.Name)));

        var credentials = new SigningCredentials(_signingKey, SecurityAlgorithms.RsaSha256);

        var jwt = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            notBefore: _clock.UtcNow.UtcDateTime,
            expires: expires.UtcDateTime,
            signingCredentials: credentials);

        var token = _tokenHandler.WriteToken(jwt);

        return new AuthResultDto(token, expires);
    }

    private async Task EnsureSigningKeyAsync(CancellationToken cancellationToken)
    {
        if (_signingKey is not null)
        {
            return;
        }

        await _keySemaphore.WaitAsync(cancellationToken);
        try
        {
            if (_signingKey is not null)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(_options.PrivateKeyPath))
            {
                throw new InvalidOperationException("Jwt:PrivateKeyPath configuration is required.");
            }

            if (string.IsNullOrWhiteSpace(_options.PublicKeyPath))
            {
                throw new InvalidOperationException("Jwt:PublicKeyPath configuration is required.");
            }

            EnsureKeyDirectoryExists(_options.PrivateKeyPath);
            EnsureKeyDirectoryExists(_options.PublicKeyPath);

            if (!File.Exists(_options.PrivateKeyPath) || !File.Exists(_options.PublicKeyPath))
            {
                _logger.LogWarning("JWT key files not found. Generating new RSA key pair at {PrivateKeyPath} and {PublicKeyPath}.", _options.PrivateKeyPath, _options.PublicKeyPath);
                GenerateAndPersistKeys();
            }

            var privateKeyPem = await File.ReadAllTextAsync(_options.PrivateKeyPath, cancellationToken);
            using var rsa = RSA.Create();
            rsa.ImportFromPem(privateKeyPem);
            _signingKey = new RsaSecurityKey(rsa.ExportParameters(includePrivateParameters: true))
            {
                KeyId = Base64UrlEncoder.Encode(RandomNumberGenerator.GetBytes(32))
            };
        }
        finally
        {
            _keySemaphore.Release();
        }
    }

    private static void EnsureKeyDirectoryExists(string path)
    {
        var directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }

    private void GenerateAndPersistKeys()
    {
        using var rsa = RSA.Create(4096);
        var privateKey = ExportPrivateKeyPem(rsa);
        var publicKey = ExportPublicKeyPem(rsa);

        File.WriteAllText(_options.PrivateKeyPath, privateKey, Encoding.UTF8);
        File.WriteAllText(_options.PublicKeyPath, publicKey, Encoding.UTF8);
    }

    private static string ExportPrivateKeyPem(RSA rsa)
    {
        var builder = new StringBuilder();
        builder.AppendLine("-----BEGIN RSA PRIVATE KEY-----");
        builder.AppendLine(Convert.ToBase64String(rsa.ExportRSAPrivateKey(), Base64FormattingOptions.InsertLineBreaks));
        builder.AppendLine("-----END RSA PRIVATE KEY-----");
        return builder.ToString();
    }

    private static string ExportPublicKeyPem(RSA rsa)
    {
        var builder = new StringBuilder();
        builder.AppendLine("-----BEGIN RSA PUBLIC KEY-----");
        builder.AppendLine(Convert.ToBase64String(rsa.ExportRSAPublicKey(), Base64FormattingOptions.InsertLineBreaks));
        builder.AppendLine("-----END RSA PUBLIC KEY-----");
        return builder.ToString();
    }
}
