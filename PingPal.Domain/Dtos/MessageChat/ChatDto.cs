using PingPal.Domain.Dtos.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingPal.Domain.Dtos.MessageChat;
public class ChatDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public Guid? OwnerUserId { get; set; }
    public bool IsDeleted { get; set; }
}