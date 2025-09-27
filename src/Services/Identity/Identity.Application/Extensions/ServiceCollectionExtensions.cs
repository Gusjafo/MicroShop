using Microsoft.Extensions.DependencyInjection;
using MicroShop.Services.Identity.Application.Abstractions;
using MicroShop.Services.Identity.Application.Services;

namespace MicroShop.Services.Identity.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddIdentityApplication(this IServiceCollection services)
    {
        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<IUserQueryService, UserQueryService>();
        return services;
    }
}
