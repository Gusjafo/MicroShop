using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MicroShop.Services.Identity.Domain.Entities;

namespace MicroShop.Services.Identity.Infrastructure.Persistence.Configurations;

internal sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles");

        builder.HasKey(role => role.Id);

        builder.Property(role => role.Id)
            .HasMaxLength(26)
            .IsRequired();

        builder.Property(role => role.Name)
            .HasMaxLength(64)
            .IsRequired();

        builder.HasIndex(role => role.Name).IsUnique();
    }
}
