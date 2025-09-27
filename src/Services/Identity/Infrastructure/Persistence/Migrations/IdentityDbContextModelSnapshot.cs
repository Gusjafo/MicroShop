using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using MicroShop.Services.Identity.Infrastructure.Persistence;

#nullable disable

namespace MicroShop.Services.Identity.Infrastructure.Persistence.Migrations
{
    [DbContext(typeof(IdentityDbContext))]
    partial class IdentityDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("MicroShop.Services.Identity.Domain.Entities.Role", b =>
            {
                b.Property<string>("Id")
                    .HasMaxLength(26)
                    .HasColumnType("nvarchar(26)");

                b.Property<string>("Name")
                    .HasMaxLength(64)
                    .HasColumnType("nvarchar(64)");

                b.HasKey("Id");

                b.HasIndex("Name")
                    .IsUnique();

                b.ToTable("Roles");
            });

            modelBuilder.Entity("MicroShop.Services.Identity.Domain.Entities.User", b =>
            {
                b.Property<string>("Id")
                    .HasMaxLength(26)
                    .HasColumnType("nvarchar(26)");

                b.Property<DateTimeOffset>("CreatedAt")
                    .HasColumnType("datetimeoffset");

                b.Property<string>("Email")
                    .HasMaxLength(256)
                    .HasColumnType("nvarchar(256)");

                b.Property<string>("PasswordHash")
                    .HasMaxLength(512)
                    .HasColumnType("nvarchar(512)");

                b.Property<string>("Username")
                    .HasMaxLength(64)
                    .HasColumnType("nvarchar(64)");

                b.HasKey("Id");

                b.HasIndex("Email")
                    .IsUnique();

                b.HasIndex("Username")
                    .IsUnique();

                b.ToTable("Users");
            });

            modelBuilder.Entity("UserRoles", b =>
            {
                b.Property<string>("UserId")
                    .HasColumnType("nvarchar(26)");

                b.Property<string>("RoleId")
                    .HasColumnType("nvarchar(26)");

                b.HasKey("UserId", "RoleId");

                b.HasIndex("RoleId");

                b.ToTable("UserRoles");

                b.HasOne("MicroShop.Services.Identity.Domain.Entities.Role", null)
                    .WithMany()
                    .HasForeignKey("RoleId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.HasOne("MicroShop.Services.Identity.Domain.Entities.User", null)
                    .WithMany()
                    .HasForeignKey("UserId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
            });
#pragma warning restore 612, 618
        }
    }
}
