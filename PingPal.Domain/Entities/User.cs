namespace PingPal.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string NormalizedName { get; set; }
    public string PasswordHash { get; set; }
    public ICollection<UserRole> UserRoles { get; set; }
    public virtual ICollection<Chat> Chats { get; set; } = new List<Chat>();
	public bool IsAdmin => UserRoles.Any(ur => ur.Role.Name == "Admin");
}