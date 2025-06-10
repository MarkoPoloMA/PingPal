using PingPal.Domain.Entities;
using PingPal.Models.Chats;

namespace PingPal.Convert
{
	public static class ChatExtensions
	{
		public static ChatModel ToChatModel(this Chat chat)
		{
			if (chat == null)
				return null;

			return new ChatModel
			{
				Id = chat.Id,
				Name = chat.Name,
				OwnerUserName = chat.OwnerUser?.Name,
				CreatedDate = chat.CreatedDate,
				IsDeleted = chat.IsDeleted
			};
		}
	}
}
