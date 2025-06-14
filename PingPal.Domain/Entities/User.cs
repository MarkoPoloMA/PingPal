namespace PingPal.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string NormalizedName { get; set; }
    public string PasswordHash { get; set; }
    public ICollection<Message> Messages { get; set; } = new List<Message>();
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<UserChat> UserChats { get; set; } = new List<UserChat>();
    public ICollection<UserContact> OwnedUserContacts { get; set; } = new List<UserContact>();
    public ICollection<UserContact> AddedUserContacts { get; set; } = new List<UserContact>();
}