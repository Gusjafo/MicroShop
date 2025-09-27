using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MicroShop.Services.Identity.Domain.Users;

namespace MicroShop.Services.Identity.Infrastructure.Persistence.Configurations;

internal sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
  public void Configure(EntityTypeBuilder<Role> builder)
  {
    builder.ToTable("Roles");

    builder.HasKey(role => role.Id);

    builder.Property(role => role.Name)
        .IsRequired()
        .HasMaxLength(64);

    builder.HasIndex(role => role.Name)
        .IsUnique();
  }
}
