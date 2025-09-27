namespace MicroShop.Services.Identity.Infrastructure.Configuration;

/// <summary>
/// Represents messaging related configuration for the identity service.
/// </summary>
public sealed class MessagingOptions
{
  public RabbitMqOptions RabbitMq { get; set; } = new();
}

/// <summary>
/// RabbitMQ specific configuration options.
/// </summary>
public sealed class RabbitMqOptions
{
  public string Host { get; set; } = "localhost";

  public string VirtualHost { get; set; } = "/";

  public string Username { get; set; } = "guest";

  public string Password { get; set; } = "guest";
}
