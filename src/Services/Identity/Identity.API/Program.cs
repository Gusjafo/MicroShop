using System.IO;
using System.Reflection;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using MicroShop.Services.Identity.API.ExceptionHandling;
using MicroShop.Services.Identity.Application.Extensions;
using MicroShop.Services.Identity.Infrastructure.Authentication;
using MicroShop.Services.Identity.Infrastructure.Extensions;
using MicroShop.Services.Identity.Infrastructure.Options;
using OpenTelemetry.Metrics;
using OpenTelemetry.Metrics.AspNetCore;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration);
});

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<IdentityExceptionHandler>();
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "MicroShop Identity API", Version = "v1" });
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

builder.Services.AddIdentityApplication();
builder.Services.AddIdentityInfrastructure(builder.Configuration);

var jwtSettings = builder.Configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>() ?? new JwtSettings();
var sqlSettings = builder.Configuration.GetSection(SqlServerSettings.SectionName).Get<SqlServerSettings>() ?? new SqlServerSettings();
var sqlConnectionString = builder.Configuration.GetConnectionString("IdentityDatabase") ?? sqlSettings.ConnectionString;

if (string.IsNullOrWhiteSpace(sqlConnectionString))
{
    throw new InvalidOperationException("SQL Server connection string is not configured.");
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = JwtValidationParametersFactory.Create(jwtSettings);
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdmin", policy => policy.RequireRole("Admin"));
});

builder.Services.AddHealthChecks()
    .AddSqlServer(sqlConnectionString, name: "sqlserver");

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService("MicroShop.Identity"))
    .WithMetrics(metrics =>
    {
        metrics.AddAspNetCoreInstrumentation();
        metrics.AddHttpClientInstrumentation();
        metrics.AddRuntimeInstrumentation();
        metrics.AddPrometheusExporter();
    })
    .WithTracing(tracing =>
    {
        tracing.AddAspNetCoreInstrumentation();
        tracing.AddHttpClientInstrumentation();
        tracing.AddEntityFrameworkCoreInstrumentation();
        tracing.AddJaegerExporter();
    });

var app = builder.Build();

app.UseSerilogRequestLogging();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/ping");
app.MapPrometheusScrapingEndpoint("/metrics");

await app.RunAsync();

public partial class Program;
