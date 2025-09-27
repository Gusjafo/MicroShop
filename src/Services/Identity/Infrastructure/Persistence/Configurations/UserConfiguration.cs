using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MicroShop.Services.Identity.Domain.Entities;

namespace MicroShop.Services.Identity.Infrastructure.Persistence.Configurations;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(user => user.Id);

        builder.Property(user => user.Id)
            .HasMaxLength(26)
            .IsRequired();

        builder.Property(user => user.Username)
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(user => user.Email)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(user => user.PasswordHash)
            .HasMaxLength(512)
            .IsRequired();

        builder.Property(user => user.CreatedAt)
            .IsRequired();

        builder.Metadata.FindNavigation(nameof(User.Roles))?.SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany<Role>("_roles")
            .WithMany()
            .UsingEntity<Dictionary<string, object>>(
                "UserRoles",
                right => right.HasOne<Role>().WithMany().HasForeignKey("RoleId").OnDelete(DeleteBehavior.Cascade),
                left => left.HasOne<User>().WithMany().HasForeignKey("UserId").OnDelete(DeleteBehavior.Cascade),
                join =>
                {
                    join.HasKey("UserId", "RoleId");
                    join.ToTable("UserRoles");
                });

        builder.HasIndex(user => user.Username).IsUnique();
        builder.HasIndex(user => user.Email).IsUnique();
    }
}
