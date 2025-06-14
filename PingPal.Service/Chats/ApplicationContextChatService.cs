using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PingPal.Database.Context.Factory;
using PingPal.Domain.Entities;

namespace PingPal.Service.Chats;

public class ApplicationContextChatService : IApplicationContextChatService
{
    private readonly IApplicationContextFactory _applicationContextFactory;

    public ApplicationContextChatService(IApplicationContextFactory context)
    {
        _applicationContextFactory = context;
    }
    public async Task<List<Chat>> CreateChatDataBase(Guid ownerId)
    {
        var _context = _applicationContextFactory.Create();

        var existsChats = await _context.Chats
            .Where(x => x.OwnerUserId == ownerId && !x.IsDeleted)
            .ToListAsync();
            
        if(existsChats.Any())
            return existsChats;
        
        var chatListTest = new List<Chat>();

        foreach (var i in Enumerable.Range(0, 10))
        {
            var chat = new Chat
            {
                Id = Guid.NewGuid(),
                CreatedDate = DateTime.UtcNow,
                Name = $"Test {i}",
                IsDeleted = false,
                OwnerUserId = ownerId
            };
            chatListTest.Add(chat);
        }
        _context.Chats.AddRange(chatListTest);
        await _context.SaveChangesAsync();

        return chatListTest;
    }
    public async Task<Chat> CreateChatAsync(string chatName, Guid? ownerId)
    {
        var _context = _applicationContextFactory.Create();
        var chat = new Chat
        {
            Id = Guid.NewGuid(),
            Name = chatName,
            OwnerUserId = ownerId,
            IsDeleted = false
        };

        _context.Chats.Add(chat);
        await _context.SaveChangesAsync();

        return chat;
    }
    public async Task<Chat?> GetChatByIdAsync(Guid chatId)
    {
        var _context = _applicationContextFactory.Create();

        return await _context.Chats
            .Include(c => c.OwnerUser)
            .Include(c => c.UserChats)
			.ThenInclude(uc => uc.User)
            .FirstOrDefaultAsync(c => c.Id == chatId && !c.IsDeleted);
    }
	public async Task<List<Chat>> GetAllChatsAsync()
	{
		var _context = _applicationContextFactory.Create();

        return await _context.Chats
			.Include(chat => chat.OwnerUser)
			.ToListAsync();
	}
    public async Task<bool> AddUserToChatAsync(Guid userId, Guid chatId)
    {
        var _context = _applicationContextFactory.Create();

        var chat = await _context.Chats.FindAsync(chatId);
        var user = await _context.Users.FindAsync(userId);

        if (chat == null || user == null || chat.IsDeleted)
        {
            return false;
        }

        var exists = await _context.UserChats.AnyAsync(uc => uc.UserId == userId && uc.ChatId == chatId);
        if (exists)
        {
            return false;
        }

        var userChat = new UserChat
        {
            UserId = userId,
            ChatId = chatId
        };

        _context.UserChats.Add(userChat);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> RemoveUserFromChatAsync(Guid userId, Guid chatId)
    {
        var _context = _applicationContextFactory.Create();

        var userChat = await _context.UserChats
            .FirstOrDefaultAsync(uc => uc.UserId == userId && uc.ChatId == chatId);

        if (userChat == null)
        {
            return false;
        }

        _context.UserChats.Remove(userChat);
        await _context.SaveChangesAsync();

        return true;
    }
    public async Task<Message> SendMessageAsync(Guid chatId, Guid userId, string text)
    {
        var _context = _applicationContextFactory.Create();

        var message = new Message
        {
            Id = Guid.NewGuid(),
            ChatId = chatId,
            UserId = userId,
            Text = text,
            CreationTime = DateTime.UtcNow
        };

        _context.Messages.Add(message);
        await _context.SaveChangesAsync();

        return message;
    }

    public async Task<List<Message>> GetMessagesAsync(Guid chatId)
    {
        var _context = _applicationContextFactory.Create();

        return await _context.Messages
            .Where(m => m.ChatId == chatId)
            .OrderBy(m => m.CreationTime)
            .ToListAsync();
    }

    public async Task<bool> DeleteMessageAsync(Guid messageId)
    {
        var _context = _applicationContextFactory.Create();

        var message = await _context.Messages.FindAsync(messageId);
        if (message == null)
        {
            return false;
        }

        _context.Messages.Remove(message);
        await _context.SaveChangesAsync();

        return true;
    }

	public async Task<List<Chat>> GetChatsByUserIdAsync(Guid userId)
	{
		var _context = _applicationContextFactory.Create();

        return await _context.Chats
			.Where(uc => uc.OwnerUserId == userId)
			.ToListAsync();
    }
    public async Task<List<Chat>> GetGroupChatsByUserIdAsync(Guid chatId)
    {
        var _context = _applicationContextFactory.Create();

        return await _context.UserChats
            .Where(uc => uc.ChatId == chatId)
            .Select(x => x.Chat)
            .ToListAsync();
    }
    public async Task<List<UserContact>> GetUserContactsAsync(Guid userId)
	{
		var _context = _applicationContextFactory.Create();

        return await _context.UserContacts
			.Where(uc => uc.UserId == userId)
			.Include(uc => uc.Contact) 
			.ToListAsync();
	}
	public async Task<List<Chat>> GetChatsAndMessageByUserIdAsync(Guid userId)
	{
		var _context = _applicationContextFactory.Create();

        return await _context.Chats
			.Include(c => c.Messages)
			.Where(c => c.UserChats.Any(uc => uc.UserId == userId && !c.IsDeleted))
			.ToListAsync();
	}
}