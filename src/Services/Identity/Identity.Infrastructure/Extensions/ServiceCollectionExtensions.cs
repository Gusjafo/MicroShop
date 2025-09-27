using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MicroShop.Services.Identity.Application.Abstractions;
using MicroShop.Services.Identity.Domain.Entities;
using MicroShop.Services.Identity.Infrastructure.Authentication;
using MicroShop.Services.Identity.Infrastructure.Initialization;
using MicroShop.Services.Identity.Infrastructure.Messaging;
using MicroShop.Services.Identity.Infrastructure.Options;
using MicroShop.Services.Identity.Infrastructure.Persistence;
using MicroShop.Services.Identity.Infrastructure.Repositories;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;

namespace MicroShop.Services.Identity.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddIdentityInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<SqlServerSettings>(configuration.GetSection(SqlServerSettings.SectionName));
        services.Configure<RabbitMqSettings>(configuration.GetSection(RabbitMqSettings.SectionName));
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));

        var sqlSettings = configuration.GetSection(SqlServerSettings.SectionName).Get<SqlServerSettings>() ?? new SqlServerSettings();
        if (string.IsNullOrWhiteSpace(sqlSettings.ConnectionString))
        {
            throw new InvalidOperationException("SQL Server connection string is not configured.");
        }
        services.AddDbContext<IdentityDbContext>(options =>
        {
            options.UseSqlServer(sqlSettings.ConnectionString, builder =>
            {
                builder.EnableRetryOnFailure();
            });
        });

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IIdentityIntegrationEventPublisher, IdentityIntegrationEventPublisher>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IdentityDatabaseInitializer>();
        services.AddHostedService<IdentityDatabaseInitializerHostedService>();

        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

        services.AddMassTransit(configure =>
        {
            configure.SetKebabCaseEndpointNameFormatter();
            configure.UsingRabbitMq((context, cfg) =>
            {
                var mqSettings = configuration.GetSection(RabbitMqSettings.SectionName).Get<RabbitMqSettings>() ?? new RabbitMqSettings();
                cfg.Host(mqSettings.Host, mqSettings.Port, mqSettings.VirtualHost, host =>
                {
                    host.Username(mqSettings.Username);
                    host.Password(mqSettings.Password);
                });

                cfg.ConfigureEndpoints(context);
            });
        });

        services.AddResiliencePipeline("identity-db", builder =>
        {
            builder.AddRetry(new RetryStrategyOptions
            {
                MaxRetryAttempts = 3,
                Delay = TimeSpan.FromSeconds(2),
                BackoffType = DelayBackoffType.Exponential
            });

            builder.AddCircuitBreaker(new CircuitBreakerStrategyOptions
            {
                SamplingDuration = TimeSpan.FromSeconds(30),
                FailureRatio = 0.5,
                MinimumThroughput = 10,
                BreakDuration = TimeSpan.FromSeconds(30)
            });
        });

        return services;
    }
}
