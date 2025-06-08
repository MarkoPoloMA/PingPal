using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PingPal.Domain.Entities;

namespace PingPa.Database.Context.Configurations;
public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
	public void Configure(EntityTypeBuilder<Role> modelBuilder)
	{
        modelBuilder.HasIndex(x => x.NormalizedName)
           .IsUnique();

        modelBuilder.ToTable(t => t.HasCheckConstraint(
                "CK_Role_Name",
                "LEN(Name) > 0"));
    }
}