using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PingPal.Domain.Entities;

namespace PingPal.Database.Context;

public class ApplicationContext : DbContext
{
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<User> Users => Set<User>();
    public DbSet<UserRole> UserRoles => Set<UserRole>(); 
    public DbSet<UserChat> UserChats => Set<UserChat>();
    public DbSet<Chat> Chats => Set<Chat>();
    public DbSet<Message> Messages => Set<Message>();
	public DbSet<UserContact> UserContacts => Set<UserContact>();

    private readonly ILogger<ApplicationContext>? _logger;

	public ApplicationContext(
		ILogger<ApplicationContext>? logger = null)
	{
		_logger = logger;
	}

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		if (optionsBuilder.IsConfigured)
			return;

		optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=PingPal;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");
		optionsBuilder.LogTo(LogMessage, LogLevel.Information);
	}

    private void LogMessage(string message)
    {
        _logger?.LogInformation(message);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        #region Role

        modelBuilder.Entity<Role>()
            .HasIndex(x => x.NormalizedName)
            .IsUnique();

        #endregion

        #region User

        modelBuilder.Entity<User>()
            .HasIndex(x => x.NormalizedName)
            .IsUnique();

        #endregion

        #region UserRole

        modelBuilder.Entity<UserRole>()
            .HasKey(x => new { x.UserId, x.RoleId });

        modelBuilder.Entity<UserRole>()
            .HasOne(x => x.User)
            .WithMany(x => x.UserRoles)
            .HasForeignKey(x => x.UserId);

        modelBuilder.Entity<UserRole>()
            .HasOne(x => x.Role)
            .WithMany(x => x.UserRoles)
            .HasForeignKey(x => x.RoleId);

        #endregion

        #region Message

        modelBuilder.Entity<Message>()
            .HasOne(m => m.Chat)
            .WithMany(m => m.Messages)
            .HasForeignKey(m => m.ChatId);

        modelBuilder.Entity<Message>()
            .HasOne(m => m.User)
            .WithMany(m => m.Messages)
            .HasForeignKey(m => m.UserId);

        #endregion

        #region UserChats

        modelBuilder.Entity<UserChat>()
            .HasKey(uc => new { uc.UserId, uc.ChatId }); 

        modelBuilder.Entity<UserChat>()
            .HasOne(uc => uc.User)
            .WithMany(uc => uc.UserChats)
            .HasForeignKey(uc => uc.UserId);

        modelBuilder.Entity<UserChat>()
            .HasOne(uc => uc.Chat)
            .WithMany(uc => uc.UserChats)
            .HasForeignKey(uc => uc.ChatId);

        #endregion

        #region UserContact

		modelBuilder.Entity<UserContact>()
			.HasKey(uc => new { uc.UserId, uc.ContactId });

		modelBuilder.Entity<UserContact>()
			.HasOne(uc => uc.User)
			.WithMany(u => u.OwnedUserContacts)
			.HasForeignKey(uc => uc.UserId)
			.OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<UserContact>()
            .HasOne(uc => uc.Contact)
            .WithMany(uc => uc.AddedUserContacts)
            .HasForeignKey(uc => uc.ContactId)
            .OnDelete(DeleteBehavior.NoAction);

        #endregion

    }
}