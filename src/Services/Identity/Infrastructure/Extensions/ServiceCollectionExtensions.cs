using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MicroShop.BuildingBlocks.Abstractions.Correlation;
using MicroShop.BuildingBlocks.Abstractions.Messaging;
using MicroShop.BuildingBlocks.Abstractions.Time;
using MicroShop.BuildingBlocks.Abstractions.Persistence;
using MicroShop.Services.Identity.Application.Abstractions;
using MicroShop.Services.Identity.Domain.Users;
using MicroShop.Services.Identity.Infrastructure.Configuration;
using MicroShop.Services.Identity.Infrastructure.Messaging;
using MicroShop.Services.Identity.Infrastructure.Persistence;
using MicroShop.Services.Identity.Infrastructure.Repositories;
using MicroShop.Services.Identity.Infrastructure.Security;

namespace MicroShop.Services.Identity.Infrastructure.Extensions;

/// <summary>
/// Provides helpers for registering the identity infrastructure services.
/// </summary>
public static class ServiceCollectionExtensions
{
  public static IServiceCollection AddIdentityInfrastructure(this IServiceCollection services, IConfiguration configuration)
  {
    services.AddCorrelationContext();
    services.AddSingleton<IClock, SystemClock>();

    services.Configure<JwtOptions>(configuration.GetSection("Jwt"));
    services.Configure<MessagingOptions>(configuration.GetSection("Messaging"));

    services.AddDbContext<IdentityDbContext>(options =>
        options.UseSqlServer(configuration.GetConnectionString("IdentityDb")));

    services.AddScoped<IUserRepository, UserRepository>();
    services.AddScoped<IRoleRepository, RoleRepository>();
    services.AddScoped<IUnitOfWork, UnitOfWork>();
    services.AddScoped<IEventBus, MassTransitEventBus>();
    services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
    services.AddScoped<IPasswordHasher, PasswordHasherAdapter>();
    services.AddScoped<IJwtService, JwtService>();

    services.AddMassTransit(busConfigurator =>
    {
      busConfigurator.SetKebabCaseEndpointNameFormatter();
      busConfigurator.UsingRabbitMq((context, cfg) =>
      {
        var messagingOptions = context.GetRequiredService<IOptions<MessagingOptions>>().Value.RabbitMq;
        cfg.Host(messagingOptions.Host, messagingOptions.VirtualHost, hostConfigurator =>
        {
          hostConfigurator.Username(messagingOptions.Username);
          hostConfigurator.Password(messagingOptions.Password);
        });

        cfg.ConfigureEndpoints(context);
      });
    });

    return services;
  }
}
