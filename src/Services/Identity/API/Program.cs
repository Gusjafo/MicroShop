using System.Reflection;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using MicroShop.BuildingBlocks.Abstractions.Correlation;
using MicroShop.Services.Identity.Application.Authentication.Commands;
using MicroShop.Services.Identity.Application.Users.Queries;
using MicroShop.Services.Identity.Infrastructure.DependencyInjection;
using MicroShop.Services.Identity.Infrastructure.Persistence;
using OpenTelemetry.Logs;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console();
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

builder.Services.AddCorrelationContext();

builder.Services.AddIdentityInfrastructure(builder.Configuration);

builder.Services.AddOpenTelemetry().WithLogging(logging =>
{
    logging.AddConsoleExporter();
});

builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseSerilogRequestLogging();
app.UseCorrelationContext();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapHealthChecks("/health", new HealthCheckOptions());
app.MapGet("/ping", () => Results.Ok(new { status = "ok" }));

app.MapPost("/register", async (
    RegisterUserCommand command,
    RegisterUserCommandHandler handler,
    CancellationToken cancellationToken) =>
{
    var result = await handler.HandleAsync(command, cancellationToken);
    return Results.Ok(result);
});

app.MapPost("/login", async (
    LoginCommand command,
    LoginCommandHandler handler,
    CancellationToken cancellationToken) =>
{
    var result = await handler.HandleAsync(command, cancellationToken);
    return Results.Ok(result);
});

app.MapGet("/users", async (UserQueries queries, CancellationToken cancellationToken) =>
{
    var users = await queries.GetAllAsync(cancellationToken);
    return Results.Ok(users);
});

app.MapGet("/users/{id}", async (string id, UserQueries queries, CancellationToken cancellationToken) =>
{
    var user = await queries.GetByIdAsync(id, cancellationToken);
    return user is null ? Results.NotFound() : Results.Ok(user);
});

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
    var idGenerator = scope.ServiceProvider.GetRequiredService<MicroShop.BuildingBlocks.Abstractions.IdGeneration.IIdGenerator>();
    await IdentityDbContextSeeder.SeedAsync(dbContext, idGenerator);
}

app.Run();

public partial class Program;
