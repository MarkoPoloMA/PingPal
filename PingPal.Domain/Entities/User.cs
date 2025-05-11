namespace PingPal.Domain.Entities;
public class User
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? NormalizedName { get; set; }
    public string? PasswordHash { get; set; }
    public DateTime RegistrationDate { get; set; }
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
