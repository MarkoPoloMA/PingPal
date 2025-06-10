using Microsoft.AspNetCore.Mvc;
using PingPal.Domain.Dtos.MessageChat;
using PingPal.Service.Chats;

namespace PingPal.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public class ChatMessageApiController : ControllerBase
{
    private readonly ApplicationContextChatService _chatService;

    public ChatMessageApiController(ApplicationContextChatService chatService)
    {
        _chatService = chatService;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateChat([FromBody] ChatDto request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Name) || request.OwnerUserId == Guid.Empty)
        {
            return BadRequest("Неверный запрос на создание чата");
        }

        var chat = await _chatService.CreateChatAsync(request.Name, request.OwnerUserId);
        return CreatedAtAction(nameof(GetChatById), new { chatId = chat.Id }, chat);
    }

    [HttpGet("{chatId}")]
    public async Task<IActionResult> GetChatById(Guid chatId)
    {
        var chat = await _chatService.GetChatByIdAsync(chatId);
        if (chat == null)
			return NotFound();

		return Ok(chat);
    }

    [HttpPost("{chatId}/users/{userId}/add")]
    public async Task<IActionResult> AddUserToChat(Guid chatId, Guid userId)
    {
        var result = await _chatService.AddUserToChatAsync(userId, chatId);
        if (!result)
			return NotFound("Чат или пользователь не найден или пользователь уже в чате.");

		return NoContent();
    }

    [HttpPost("{chatId}/users/{userId}/remove")]
    public async Task<IActionResult> RemoveUserFromChat(Guid chatId, Guid userId)
    {
        var result = await _chatService.RemoveUserFromChatAsync(userId, chatId);
        if (!result)
			return NotFound("Пользователь не найден в чате");

		return NoContent();
    }

    [HttpPost("{chatId}/messages")]
    public async Task<IActionResult> SendMessage(Guid chatId, [FromBody] MessageDto request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Text) || request.UserId == Guid.Empty)
			return BadRequest("Неверный запрос сообщения");

		var message = await _chatService.SendMessageAsync(chatId, request.UserId, request.Text);
        return CreatedAtAction(nameof(GetMessages), new { chatId }, message);
    }

    [HttpGet("{chatId}/messages")]
    public async Task<IActionResult> GetMessages(Guid chatId)
    {
        var messages = await _chatService.GetMessagesAsync(chatId);
        return Ok(messages);
    }

    [HttpDelete("messages/{messageId}")]
    public async Task<IActionResult> DeleteMessage(Guid messageId)
    {
        var result = await _chatService.DeleteMessageAsync(messageId);
        if (!result)
			return NotFound("Сообщение не найдено");

		return NoContent();
    }

	[HttpGet("users/{userId}/chats")]
	public async Task<IActionResult> GetUserChats(Guid userId)
	{
		if (userId == Guid.Empty)
			return BadRequest("Неверный идентификатор пользователя");

		var chats = await _chatService.GetChatsByUserIdAsync(userId);

		return Ok(chats);
	}

	[HttpGet("users/{userId}/contacts")]
	public async Task<IActionResult> GetUserContacts(Guid userId)
	{
		if (userId == Guid.Empty)
			return BadRequest("Неверный идентификатор пользователя");

		var contacts = await _chatService.GetUserContactsAsync(userId);
		return Ok(contacts);
	}
}