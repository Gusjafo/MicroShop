using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MicroShop.Services.Identity.Domain.Users;

namespace MicroShop.Services.Identity.Infrastructure.Persistence.Configurations;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
  public void Configure(EntityTypeBuilder<User> builder)
  {
    builder.ToTable("Users");

    builder.HasKey(user => user.Id);

    builder.Property(user => user.Username)
        .IsRequired()
        .HasMaxLength(64);

    builder.Property(user => user.Email)
        .IsRequired()
        .HasMaxLength(256);

    builder.Property(user => user.PasswordHash)
        .IsRequired()
        .HasMaxLength(512);

    builder.Property(user => user.CreatedAt)
        .IsRequired();

    builder.HasIndex(user => user.Username)
        .IsUnique();

    builder.HasIndex(user => user.Email)
        .IsUnique();

    builder.Navigation(nameof(User.Roles))
        .UsePropertyAccessMode(PropertyAccessMode.Field);

    builder
        .HasMany<Role>(nameof(User.Roles))
        .WithMany()
        .UsingEntity<Dictionary<string, object>>(
            "UserRoles",
            right => right.HasOne<Role>().WithMany().HasForeignKey("RoleId"),
            left => left.HasOne<User>().WithMany().HasForeignKey("UserId"),
            join =>
            {
              join.ToTable("UserRoles");
              join.HasKey("UserId", "RoleId");
            });
  }
}
