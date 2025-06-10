using PingPal.Domain.Entities;

namespace PingPal.Models.Chats;
public class Message
{
    public Guid Id { get; set; }
    public string? Text { get; set; }
    public DateTime CreationTime { get; set; }
    public DateTime EditTime { get; set; }
    public Guid ChatId { get; set; }
    public Chat Chat { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; }
    public bool IsDeleted { get; set; }
}
