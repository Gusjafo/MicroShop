using Microsoft.EntityFrameworkCore;
using MicroShop.Services.Identity.Domain.Users;

namespace MicroShop.Services.Identity.Infrastructure.Persistence;

/// <summary>
/// Represents the EF Core database context for the Identity service.
/// </summary>
public sealed class IdentityDbContext : DbContext
{
  public IdentityDbContext(DbContextOptions<IdentityDbContext> options)
      : base(options)
  {
  }

  public DbSet<User> Users => this.Set<User>();

  public DbSet<Role> Roles => this.Set<Role>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);
    modelBuilder.ApplyConfigurationsFromAssembly(typeof(IdentityDbContext).Assembly);
  }
}
