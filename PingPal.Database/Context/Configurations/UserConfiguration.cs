using PingPal.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PingPal.Database.Context.Configurations;
public class UserConfiguration : IEntityTypeConfiguration<User>
{
	public void Configure(EntityTypeBuilder<User> modelBuilder)
	{
		modelBuilder.Property(login => login.NormalizedName)
			.IsRequired()
			.HasMaxLength(100);

		modelBuilder.Property(name => name.Name)
			.IsRequired()
			.HasMaxLength(100);

        modelBuilder.HasIndex(login => login.NormalizedName)
			.IsUnique();

		modelBuilder.HasIndex(name => name.Name)
			.IsUnique();

        modelBuilder.Property(pass => pass.PasswordHash)
			.IsRequired()
			.HasMaxLength(50);	
	}
}