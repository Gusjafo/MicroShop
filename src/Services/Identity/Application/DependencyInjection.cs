using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using MicroShop.Services.Identity.Application.Abstractions;
using MicroShop.Services.Identity.Application.Authentication.Commands;
using MicroShop.Services.Identity.Application.Authentication.Validators;
using MicroShop.Services.Identity.Application.Users.Queries;

namespace MicroShop.Services.Identity.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddIdentityApplication(this IServiceCollection services)
    {
        services.AddScoped<RegisterUserCommandHandler>();
        services.AddScoped<LoginCommandHandler>();
        services.AddScoped<UserQueries>();

        services.AddScoped<IValidator<RegisterUserCommand>, RegisterUserCommandValidator>();
        services.AddScoped<IValidator<LoginCommand>, LoginCommandValidator>();

        return services;
    }
}
