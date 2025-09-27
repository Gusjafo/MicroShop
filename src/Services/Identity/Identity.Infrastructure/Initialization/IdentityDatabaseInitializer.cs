using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MicroShop.Services.Identity.Application.Abstractions;
using MicroShop.Services.Identity.Infrastructure.Persistence;
using Polly;

namespace MicroShop.Services.Identity.Infrastructure.Initialization;

internal sealed class IdentityDatabaseInitializer(
    IdentityDbContext dbContext,
    IRoleRepository roleRepository,
    ILogger<IdentityDatabaseInitializer> logger,
    ResiliencePipelineProvider<string> pipelineProvider)
{
    private readonly IdentityDbContext _dbContext = dbContext;
    private readonly IRoleRepository _roleRepository = roleRepository;
    private readonly ILogger<IdentityDatabaseInitializer> _logger = logger;
    private readonly ResiliencePipeline _pipeline = pipelineProvider.GetPipeline("identity-db");

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Ensuring database for Identity service is created");
        await _pipeline.ExecuteAsync(async ct => await _dbContext.Database.EnsureCreatedAsync(ct), cancellationToken);

        _logger.LogInformation("Ensuring default identity roles exist");
        await _pipeline.ExecuteAsync(async ct => await _roleRepository.EnsureCreatedAsync(ct), cancellationToken);
    }
}
