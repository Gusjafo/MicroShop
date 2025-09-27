using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using MicroShop.Services.Identity.Infrastructure.Options;

namespace MicroShop.Services.Identity.IntegrationTests.Infrastructure;

internal sealed class IdentityApiFactory : WebApplicationFactory<Program>
{
    private readonly string _sqlConnectionString;
    private readonly string _rabbitMqHost;
    private readonly string _rabbitMqPort;
    private readonly string _rabbitMqUsername;
    private readonly string _rabbitMqPassword;
    private readonly string _jwtPrivateKey;
    private readonly string _jwtPublicKey;

    public IdentityApiFactory(
        string sqlConnectionString,
        string rabbitMqHost,
        ushort rabbitMqPort,
        string rabbitMqUsername,
        string rabbitMqPassword,
        string jwtPrivateKey,
        string jwtPublicKey)
    {
        _sqlConnectionString = sqlConnectionString;
        _rabbitMqHost = rabbitMqHost;
        _rabbitMqPort = rabbitMqPort.ToString();
        _rabbitMqUsername = rabbitMqUsername;
        _rabbitMqPassword = rabbitMqPassword;
        _jwtPrivateKey = jwtPrivateKey;
        _jwtPublicKey = jwtPublicKey;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("IntegrationTesting");
        builder.ConfigureAppConfiguration((context, configurationBuilder) =>
        {
            var overrides = new Dictionary<string, string?>
            {
                [$"{SqlServerSettings.SectionName}:{nameof(SqlServerSettings.ConnectionString)}"] = _sqlConnectionString,
                [$"{RabbitMqSettings.SectionName}:Host"] = _rabbitMqHost,
                [$"{RabbitMqSettings.SectionName}:Port"] = _rabbitMqPort,
                [$"{RabbitMqSettings.SectionName}:Username"] = _rabbitMqUsername,
                [$"{RabbitMqSettings.SectionName}:Password"] = _rabbitMqPassword,
                [$"{RabbitMqSettings.SectionName}:VirtualHost"] = "/",
                [$"{JwtSettings.SectionName}:Issuer"] = "integration-tests",
                [$"{JwtSettings.SectionName}:Audience"] = "integration-client",
                [$"{JwtSettings.SectionName}:PrivateKeyPem"] = _jwtPrivateKey,
                [$"{JwtSettings.SectionName}:PublicKeyPem"] = _jwtPublicKey,
                [$"{JwtSettings.SectionName}:Algorithm"] = "RS256",
                [$"{JwtSettings.SectionName}:AccessTokenLifetime"] = "00:30:00",
                [$"{JwtSettings.SectionName}:RefreshTokenLifetime"] = "7.00:00:00"
            };

            configurationBuilder.AddInMemoryCollection(overrides);
        });
    }
}
