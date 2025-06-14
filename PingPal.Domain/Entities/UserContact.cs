namespace PingPal.Domain.Entities;

public class UserContact
{
	public Guid UserId { get; set; }
	public Guid ContactId { get; set; }
	public User User { get; set; }
	public User Contact { get; set; }
}