namespace MicroShop.Services.Identity.Infrastructure.Options;

public sealed class JwtSettings
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = string.Empty;

    public string Audience { get; set; } = string.Empty;

    public TimeSpan AccessTokenLifetime { get; set; } = TimeSpan.FromMinutes(30);

    public TimeSpan RefreshTokenLifetime { get; set; } = TimeSpan.FromDays(7);

    public string PrivateKeyPem { get; set; } = string.Empty;

    public string PublicKeyPem { get; set; } = string.Empty;

    public string Algorithm { get; set; } = "RS256";
}
