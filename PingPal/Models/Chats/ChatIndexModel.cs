using System.ComponentModel;
using PingPal.Models.Users;

namespace PingPal.Models.Chats;
public class ChatIndexModel : SortingPaginationModelBase
{
	[DisplayName("Имя или id")]
	public string? SearchString { get; set; }
    public ChatModel Chat { get; set; }
	public List<UserModel> Users { get; set; }
}

