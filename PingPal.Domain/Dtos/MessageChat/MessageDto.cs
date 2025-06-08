using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingPal.Domain.Dtos.MessageChat;
public class MessageDto
{
    public Guid Id { get; set; }
    public string Text { get; set; }
    public DateTime CreationTime { get; set; }
    public Guid ChatId { get; set; }
    public Guid UserId { get; set; }
    public bool IsDeleted { get; set; }
}