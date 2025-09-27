using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MicroShop.BuildingBlocks.Abstractions.Common;
using MicroShop.Services.Identity.Application.DTOs;
using MicroShop.Services.Identity.Application.Extensions;
using MicroShop.Services.Identity.Application.Users.Commands;
using MicroShop.Services.Identity.Application.Users.Queries;
using MicroShop.Services.Identity.Application.Users.Validators;
using MicroShop.Services.Identity.Infrastructure.Configuration;
using MicroShop.Services.Identity.Infrastructure.Extensions;
using Serilog;
using FluentValidation.Results;
using OpenTelemetry.Exporter;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) =>
{
  configuration
      .ReadFrom.Configuration(context.Configuration)
      .ReadFrom.Services(services)
      .Enrich.FromLogContext()
      .WriteTo.Console();
});

builder.Services.AddProblemDetails();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddIdentityApplication();
builder.Services.AddIdentityInfrastructure(builder.Configuration);

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing =>
    {
      tracing.AddAspNetCoreInstrumentation();
      tracing.AddHttpClientInstrumentation();
      tracing.AddJaegerExporter();
    });

builder.Services.Configure<JaegerExporterOptions>(builder.Configuration.GetSection("OpenTelemetry:Jaeger"));

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.Clear();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
      var jwtOptions = builder.Configuration.GetSection("Jwt").Get<JwtOptions>() ?? new JwtOptions();
      options.TokenValidationParameters = new TokenValidationParameters
      {
        ValidIssuer = jwtOptions.Issuer,
        ValidateIssuer = true,
        ValidAudience = jwtOptions.Audience,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = LoadPublicKey(jwtOptions),
        ClockSkew = TimeSpan.FromMinutes(1)
      };
    });

builder.Services.AddAuthorization(options =>
{
  options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
});

var app = builder.Build();

app.UseSerilogRequestLogging();
app.UseCorrelationContext();
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/ping", () => Results.Ok(new { status = "ok" }));

app.MapPost("/register", async Task<Results<Created<UserDto>, ValidationProblem, ConflictHttpResult>> (
        RegisterUserRequest request,
        IMediator mediator,
        IValidator<RegisterUserRequest> validator) =>
    {
      var validationResult = await validator.ValidateAsync(request);
      if (!validationResult.IsValid)
      {
        return TypedResults.ValidationProblem(ToDictionary(validationResult));
      }

      try
      {
        var user = await mediator.Send(new RegisterUserCommand(request));
        return TypedResults.Created($"/users/{user.Id}", user);
      }
      catch (InvalidOperationException ex)
      {
        return TypedResults.Conflict(new ProblemDetails
        {
          Title = "User already exists",
          Detail = ex.Message,
          Status = StatusCodes.Status409Conflict
        });
      }
    })
    .WithName("RegisterUser")
    .Produces<UserDto>(StatusCodes.Status201Created)
    .ProducesProblem(StatusCodes.Status409Conflict)
    .ProducesValidationProblem()
    .WithOpenApi();

app.MapPost("/login", async Task<Results<Ok<AuthResultDto>, ValidationProblem, UnauthorizedHttpResult>> (
        LoginRequest request,
        IMediator mediator,
        IValidator<LoginRequest> validator) =>
    {
      var validationResult = await validator.ValidateAsync(request);
      if (!validationResult.IsValid)
      {
        return TypedResults.ValidationProblem(ToDictionary(validationResult));
      }

      try
      {
        var result = await mediator.Send(new LoginCommand(request));
        return TypedResults.Ok(result);
      }
      catch (UnauthorizedAccessException)
      {
        return TypedResults.Unauthorized();
      }
    })
    .WithName("Login")
    .Produces<AuthResultDto>(StatusCodes.Status200OK)
    .ProducesValidationProblem()
    .Produces(StatusCodes.Status401Unauthorized)
    .WithOpenApi();

app.MapGet("/users/{id:guid}", async Task<Results<Ok<UserDto>, NotFound>> (Guid id, IMediator mediator) =>
    {
      var user = await mediator.Send(new GetUserByIdQuery(id));
      return user is not null ? TypedResults.Ok(user) : TypedResults.NotFound();
    })
    .RequireAuthorization("Admin")
    .WithName("GetUserById")
    .Produces<UserDto>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound)
    .WithOpenApi();

app.MapGet("/users", async Task<Results<Ok<PagedResult<UserDto>>, ValidationProblem>> (
        int page = 1,
        int size = 20,
        IMediator mediator,
        IValidator<ListUsersQuery> validator) =>
    {
      var query = new ListUsersQuery(page, size);
      var validationResult = await validator.ValidateAsync(query);
      if (!validationResult.IsValid)
      {
        return TypedResults.ValidationProblem(ToDictionary(validationResult));
      }

      var result = await mediator.Send(query);
      return TypedResults.Ok(result);
    })
    .RequireAuthorization("Admin")
    .WithName("ListUsers")
    .Produces<PagedResult<UserDto>>(StatusCodes.Status200OK)
    .ProducesValidationProblem()
    .WithOpenApi();

app.Run();

static IDictionary<string, string[]> ToDictionary(ValidationResult validationResult)
{
  return validationResult.Errors
      .GroupBy(error => error.PropertyName ?? string.Empty)
      .ToDictionary(group => group.Key, group => group.Select(error => error.ErrorMessage).ToArray());
}

static SecurityKey LoadPublicKey(JwtOptions options)
{
  if (!File.Exists(options.PublicKeyPath))
  {
    throw new FileNotFoundException("JWT public key file not found.", options.PublicKeyPath);
  }

  var pem = File.ReadAllText(options.PublicKeyPath);
  try
  {
    var rsa = RSA.Create();
    rsa.ImportFromPem(pem);
    return new RsaSecurityKey(rsa);
  }
  catch (CryptographicException)
  {
    var ecdsa = ECDsa.Create();
    ecdsa.ImportFromPem(pem);
    return new ECDsaSecurityKey(ecdsa);
  }
}
