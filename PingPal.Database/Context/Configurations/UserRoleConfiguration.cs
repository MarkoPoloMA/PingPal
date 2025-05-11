using PingPal.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PingPal.Database.Context.Configurations;
public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
	public void Configure(EntityTypeBuilder<UserRole> modelBuilder)
	{
        modelBuilder.HasKey(x => new { x.UserId, x.RoleId });

        modelBuilder.HasIndex(x => x.UserId);
        modelBuilder.HasIndex(x => x.RoleId);

        modelBuilder.HasOne(x => x.User)
            .WithMany(x => x.UserRoles)
            .HasForeignKey(x => x.UserId);

        modelBuilder.HasOne(x => x.Role)
            .WithMany(x => x.UserRoles)
            .HasForeignKey(x => x.RoleId);
    }
}