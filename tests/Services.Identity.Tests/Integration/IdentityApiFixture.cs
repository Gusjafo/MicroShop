using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Testcontainers.RabbitMq;
using Testcontainers.SqlEdge;

namespace MicroShop.Services.Identity.Tests.Integration;

public sealed class IdentityApiFixture : IAsyncLifetime
{
    private readonly SqlEdgeContainer _sqlContainer;
    private readonly RabbitMqContainer _rabbitContainer;
    private readonly string _keysDirectory;

    public IdentityApiFixture()
    {
        _sqlContainer = new SqlEdgeBuilder()
            .WithPassword("yourStrong(!)Password")
            .WithEnvironment("ACCEPT_EULA", "Y")
            .WithImage("mcr.microsoft.com/azure-sql-edge:latest")
            .WithCleanUp(true)
            .Build();

        _rabbitContainer = new RabbitMqBuilder()
            .WithImage("rabbitmq:3.13-management")
            .WithCleanUp(true)
            .Build();

        _keysDirectory = Path.Combine(Path.GetTempPath(), "identity-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_keysDirectory);
    }

    public HttpClient Client { get; private set; } = default!;

    public WebApplicationFactory<Program> Factory { get; private set; } = default!;

    public async Task InitializeAsync()
    {
        await _sqlContainer.StartAsync();
        await _rabbitContainer.StartAsync();

        var connectionString = _sqlContainer.GetConnectionString();
        var jwtPrivateKey = Path.Combine(_keysDirectory, "identity_rsa_private.pem");
        var jwtPublicKey = Path.Combine(_keysDirectory, "identity_rsa_public.pem");

        Factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.UseSetting("ConnectionStrings:Identity", connectionString);
            builder.UseSetting("Jwt:PrivateKeyPath", jwtPrivateKey);
            builder.UseSetting("Jwt:PublicKeyPath", jwtPublicKey);
            builder.UseSetting("Jwt:Issuer", "MicroShop.Identity.Tests");
            builder.UseSetting("Jwt:Audience", "MicroShop.Tests");
            builder.UseSetting("RabbitMQ:Host", _rabbitContainer.Hostname);
            builder.UseSetting("RabbitMQ:Username", _rabbitContainer.Username);
            builder.UseSetting("RabbitMQ:Password", _rabbitContainer.Password);
            builder.UseSetting("DOTNET_ENVIRONMENT", "Development");
            builder.ConfigureAppConfiguration((context, config) =>
            {
                var overrides = new Dictionary<string, string?>
                {
                    ["ConnectionStrings:Identity"] = connectionString,
                    ["Jwt:PrivateKeyPath"] = jwtPrivateKey,
                    ["Jwt:PublicKeyPath"] = jwtPublicKey,
                    ["Jwt:Issuer"] = "MicroShop.Identity.Tests",
                    ["Jwt:Audience"] = "MicroShop.Tests",
                    ["RabbitMQ:Host"] = _rabbitContainer.Hostname,
                    ["RabbitMQ:Username"] = _rabbitContainer.Username,
                    ["RabbitMQ:Password"] = _rabbitContainer.Password
                };

                config.AddInMemoryCollection(overrides!);
            });
        });

        Client = Factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("http://localhost"),
            AllowAutoRedirect = false
        });
    }

    public async Task DisposeAsync()
    {
        Factory?.Dispose();

        if (_sqlContainer.IsRunning)
        {
            await _sqlContainer.StopAsync();
        }

        if (_rabbitContainer.IsRunning)
        {
            await _rabbitContainer.StopAsync();
        }

        await _sqlContainer.DisposeAsync();
        await _rabbitContainer.DisposeAsync();

        if (Directory.Exists(_keysDirectory))
        {
            Directory.Delete(_keysDirectory, true);
        }
    }
}
