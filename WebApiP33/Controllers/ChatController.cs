using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApiP33.Models.Dto;
using WebApiP33.Services;

namespace WebApiP33.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/chat")]
public class ChatController(IChatService chatService) : ControllerBase
{
    // Список чатів (з ким спілкується користувач)
    [HttpGet("chats")]
    public async Task<IEnumerable<UserDto>> GetChats()
    {
        return await chatService.GetChatsAsync(GetCurrentUserId());
    }

    // Завантажити повідомлення
    [HttpGet("messages/{recipientId:int}")]
    public async Task<IEnumerable<MessageDto>> GetMessages(int recipientId)
    {
        return await chatService.GetMessagesAsync(GetCurrentUserId(), recipientId);
    }


    // Надіслати повідомлення
    [HttpPost("send")]
    public async Task<SendMessageResultDto> SendMessage(SendMessageRequestDto request)
    {
        return await chatService.SendMessageAsync(GetCurrentUserId(), request);
    }



    // Пошук користувачів
    [HttpPost("find-users")]
    public async Task<IEnumerable<UserDto>> FindUsers(UserSearchDto search)
    {
        return await chatService.FindUsersAsync(GetCurrentUserId(), search);
    }

    private int GetCurrentUserId()
    {
        var currentUserId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        return int.Parse(currentUserId!);
    }

}
