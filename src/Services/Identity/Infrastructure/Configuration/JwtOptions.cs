namespace MicroShop.Services.Identity.Infrastructure.Configuration;

/// <summary>
/// Represents JWT configuration values for the identity service.
/// </summary>
public sealed class JwtOptions
{
  public string Issuer { get; set; } = string.Empty;

  public string Audience { get; set; } = string.Empty;

  public int ExpiresMinutes { get; set; } = 60;

  public string PrivateKeyPath { get; set; } = "identity-secrets/private.key";

  public string PublicKeyPath { get; set; } = "identity-secrets/public.key";
}
