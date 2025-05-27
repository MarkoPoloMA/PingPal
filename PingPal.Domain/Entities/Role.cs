
namespace PingPal.Domain.Entities
{
    public class Role
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? NormalizedName { get; set; }
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}
