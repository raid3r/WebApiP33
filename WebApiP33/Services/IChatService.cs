using WebApiP33.Models.Dto;

namespace WebApiP33.Services;

public interface IChatService
{
    Task<IEnumerable<UserDto>> GetChatsAsync(int currentUserId);
    Task<IEnumerable<MessageDto>> GetMessagesAsync(int currentUserId, int recipientId);
    Task<SendMessageResultDto> SendMessageAsync(int currentUserId, SendMessageRequestDto request);
    Task<IEnumerable<UserDto>> FindUsersAsync(int currentUserId, UserSearchDto search);
}
