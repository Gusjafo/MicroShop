using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MicroShop.BuildingBlocks.Abstractions.IdGeneration;
using MicroShop.BuildingBlocks.Abstractions.Time;
using MicroShop.Services.Identity.Application;
using MicroShop.Services.Identity.Application.Abstractions;
using MicroShop.Services.Identity.Infrastructure.Messaging;
using MicroShop.Services.Identity.Infrastructure.Persistence;
using MicroShop.Services.Identity.Infrastructure.Persistence.Repositories;
using MicroShop.Services.Identity.Infrastructure.Security;

namespace MicroShop.Services.Identity.Infrastructure.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddIdentityInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddIdentityApplication();

        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));

        services.AddDbContext<IdentityDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("Identity")
                ?? throw new InvalidOperationException("Connection string 'Identity' is not configured.");

            options.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure();
                sqlOptions.MigrationsAssembly(typeof(IdentityDbContext).Assembly.FullName);
            });
        });

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IPasswordHasherService, PasswordHasherService>();
        services.AddScoped<ITokenService, JwtTokenService>();
        services.AddScoped<IUserIntegrationEventPublisher, UserIntegrationEventPublisher>();

        services.AddScoped<IPasswordHasher<Domain.Entities.User>, PasswordHasher<Domain.Entities.User>>();

        services.AddMassTransit(busConfigurator =>
        {
            busConfigurator.SetKebabCaseEndpointNameFormatter();
            busConfigurator.UsingRabbitMq((context, cfg) =>
            {
                var rabbitMqOptions = configuration.GetSection("RabbitMQ");
                var host = rabbitMqOptions.GetValue<string>("Host") ?? "rabbitmq";
                var username = rabbitMqOptions.GetValue<string>("Username") ?? "guest";
                var password = rabbitMqOptions.GetValue<string>("Password") ?? "guest";

                cfg.Host(host, h =>
                {
                    h.Username(username);
                    h.Password(password);
                });

                cfg.ConfigureEndpoints(context);
            });
        });

        services.AddOptions<JwtBearerOptions>().Configure(options => { });

        services.AddHealthChecks().AddDbContextCheck<IdentityDbContext>(name: "IdentityDb");

        services.AddOpenTelemetry()
            .WithTracing(builder =>
            {
                builder.AddAspNetCoreInstrumentation();
                builder.AddHttpClientInstrumentation();
                builder.AddSqlClientInstrumentation();
                builder.AddSource("MassTransit");
            });

        services.AddSingleton<IClock, SystemClock>();
        services.AddSingleton<IIdGenerator, UlidIdGenerator>();

        return services;
    }
}
