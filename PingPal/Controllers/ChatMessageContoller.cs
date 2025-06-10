using Microsoft.AspNetCore.Mvc;
using PingPal.Convert;
using PingPal.Models.Chats;
using PingPal.Service.Chats;

namespace PingPal.Controllers;

public class ChatMessageController : Controller
{
    private readonly ApplicationContextChatService _chatService;

    public ChatMessageController(ApplicationContextChatService chatService)
    {
        _chatService = chatService;
	}

	public async Task<IActionResult> Index()
	{
		var chats = await _chatService.GetAllChatsAsync();
		var chatModels = chats.Select(chat => new ChatModel
		{
			Id = chat.Id,
			Name = chat.Name,
			OwnerUserId = chat.OwnerUserId,
			OwnerUserName = chat.OwnerUser.Name,
			IsDeleted = chat.IsDeleted
		}).ToList();

		return View(chatModels);
	}

	[HttpGet("chat/{chatId}")]
	public async Task<IActionResult> GetChatById(Guid chatId)
	{
		var chat = await _chatService.GetChatByIdAsync(chatId);
		if (chat == null)
			return NotFound();
		var chatModel = chat.ToChatModel();

        return View(chatModel);
	}

    [HttpPost("chat/{chatId}/users/add")]
    public async Task<IActionResult> AddUserToChat(Guid chatId, Guid userId)
    {
        var result = await _chatService.AddUserToChatAsync(userId, chatId);
        if (!result)
            return NotFound("Чат или пользователь не найден или пользователь уже в чате.");

        return RedirectToAction("GetChatById", new { chatId });
    }
    [HttpGet("users/{userId}/chats")]
	public async Task<IActionResult> GetUserChats(Guid userId)
	{
		if (userId == Guid.Empty)
			return BadRequest("Неверный идентификатор пользователя");

		var chats = await _chatService.GetChatsByUserIdAsync(userId);

		var chatModels = chats.Select(chat => chat.ToChatModel()).ToList();

        return View(chatModels);
	}
}