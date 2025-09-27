using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace MicroShop.Services.Identity.Application.Extensions;

/// <summary>
/// Provides helpers for configuring the application layer dependencies.
/// </summary>
public static class ServiceCollectionExtensions
{
  public static IServiceCollection AddIdentityApplication(this IServiceCollection services)
  {
    var assembly = Assembly.GetExecutingAssembly();
    services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
    services.AddValidatorsFromAssembly(assembly);
    return services;
  }
}
