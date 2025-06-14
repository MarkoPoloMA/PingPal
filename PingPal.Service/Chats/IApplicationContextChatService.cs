using PingPal.Domain.Entities;

namespace PingPal.Service.Chats;

public interface IApplicationContextChatService
{
	Task<Chat> CreateChatAsync(string chatName, Guid? ownerId);
	Task<Chat?> GetChatByIdAsync(Guid chatId);
	Task<bool> AddUserToChatAsync(Guid userId, Guid chatId);
	Task<bool> RemoveUserFromChatAsync(Guid userId, Guid chatId);
	Task<Message> SendMessageAsync(Guid chatId, Guid userId, string text);
	Task<List<Message>> GetMessagesAsync(Guid chatId);
	Task<bool> DeleteMessageAsync(Guid messageId);
	Task<List<Chat>> GetChatsByUserIdAsync(Guid userId);
}


