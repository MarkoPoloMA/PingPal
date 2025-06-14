using PingPal.Domain.Entities;

namespace PingPal.Models.Chats;

public class ChatModel
{
	public Guid Id { get; set; }
	public string Name { get; set; }
	public Guid? OwnerUserId { get; set; }
	public DateTime CreatedDate { get; set; }
    public string OwnerUserName { get; set; }
	public bool IsDeleted { get; set; }
    public ICollection<UserChat> UserChats { get; set; } = new List<UserChat>();
}