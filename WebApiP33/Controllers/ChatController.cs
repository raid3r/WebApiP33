using Microsoft.AspNetCore.Mvc;
using WebApiP33.Models;
using WebApiP33.Models.Dto;

namespace WebApiP33.Controllers;

[ApiController]
[Route("[controller]")] // /calculator
public class ChatController : ControllerBase
{
    // Список чатів (з ким спілкується користувач)
    [HttpGet("chats")]
    public async Task<IEnumerable<UserDto>> GetChats()
    {
        // TODO : реалізувати завантаження списку чатів для користувача
        
        return [
            new UserDto { RecipientId = 2, Email = "jane.smith@test.com" },
            new UserDto { RecipientId = 3, Email = "bob.johnson@test.com" }
        ];
    }

    // Завантажити повідомлення
    [HttpGet("messages/{recipientId:int}")]
    public async Task<IEnumerable<MessageDto>> GetMessages(int recipientId)
    {
        // int page = 1, int pageSize = 20 
        // TODO : реалізувати завантаження повідомлень для отримувача

        var messages = new List<MessageDto>
        {
            new MessageDto
            {
                Id = 1,
                From = new UserDto { RecipientId = 2, Email = "jane.smith@test.com" },
                To = new UserDto { RecipientId = recipientId, Email = "john.doe@test.com" },
                Text = "Привіт!",
                Timestamp = DateTime.UtcNow.AddMinutes(-10)
            },
            new MessageDto
            {
                Id = 2,
                From = new UserDto { RecipientId = recipientId, Email = "john.doe@test.com" },
                To = new UserDto { RecipientId = 2, Email = "jane.smith@test.com" },
                Text = "Привіт, як справи?",
                Timestamp = DateTime.UtcNow.AddMinutes(-5)
            }
        };
        
        return messages;
    }


    // Надіслати повідомлення
    [HttpPost("send")]
    public async Task<SendMessageResultDto> SendMessage(SendMessageRequestDto request)
    {
        // TODO : реалізувати надсилання повідомлення
        return new SendMessageResultDto { MessageId = 123 };
    }



    // Пошук користувачів
    [HttpPost("find-users")]
    public async Task<IEnumerable<UserDto>> FindUsers(UserSearchDto search)
    {
        // TODO : реалізувати пошук користувачів за іменем

        return [
            new UserDto { RecipientId = 1, Email = "john.doe@test.com" },
            new UserDto { RecipientId = 2, Email = "jane.smith@test.com" }
        ];
    }

}
