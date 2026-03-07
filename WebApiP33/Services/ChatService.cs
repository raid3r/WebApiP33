using Microsoft.EntityFrameworkCore;
using WebApiP33.Models;
using WebApiP33.Models.DAL;
using WebApiP33.Models.Dto;

namespace WebApiP33.Services;

public class ChatService(ChatContext context) : IChatService
{
    public async Task<IEnumerable<UserDto>> GetChatsAsync(int currentUserId)
    {
        var currentRecipient = await GetRecipientByUserIdAsync(currentUserId);
        if (currentRecipient == null)
        {
            return Array.Empty<UserDto>();
        }

        var chatRecipientIds = await context.ChatMessages
            .AsNoTracking()
            .Where(m => m.FromId == currentRecipient.Id || m.ToId == currentRecipient.Id)
            .Select(m => m.FromId == currentRecipient.Id ? m.ToId : m.FromId)
            .Distinct()
            .ToListAsync();

        if (chatRecipientIds.Count == 0)
        {
            return Array.Empty<UserDto>();
        }

        var recipients = await context.Recipients
            .AsNoTracking()
            .Include(r => r.User)
            .Where(r => chatRecipientIds.Contains(r.Id))
            .ToListAsync();

        return recipients.Select(MapUserDto);
    }

    public async Task<IEnumerable<MessageDto>> GetMessagesAsync(int currentUserId, int recipientId)
    {
        var currentRecipient = await GetRecipientByUserIdAsync(currentUserId);
        if (currentRecipient == null)
        {
            return Array.Empty<MessageDto>();
        }

        var messages = await context.ChatMessages
            .AsNoTracking()
            .Include(m => m.From)
            .Include(m => m.To)
            .Where(m => (m.FromId == currentRecipient.Id && m.ToId == recipientId)
                || (m.FromId == recipientId && m.ToId == currentRecipient.Id))
            .OrderBy(m => m.Timestamp)
            .ToListAsync();

        return messages.Select(m => new MessageDto
        {
            Id = m.Id,
            From = MapRecipientDto(m.From),
            To = MapRecipientDto(m.To),
            Text = m.Text,
            Timestamp = m.Timestamp
        });
    }

    public async Task<SendMessageResultDto> SendMessageAsync(int currentUserId, SendMessageRequestDto request)
    {
        var currentRecipient = await GetRecipientByUserIdAsync(currentUserId);
        if (currentRecipient == null)
        {
            return new SendMessageResultDto { MessageId = 0 };
        }

        var targetExists = await context.Recipients
            .AsNoTracking()
            .AnyAsync(r => r.Id == request.RecipientId);
        if (!targetExists)
        {
            return new SendMessageResultDto { MessageId = 0 };
        }

        var message = new ChatMessage
        {
            FromId = currentRecipient.Id,
            ToId = request.RecipientId,
            Text = request.Text,
            Timestamp = DateTime.UtcNow
        };

        context.ChatMessages.Add(message);
        await context.SaveChangesAsync();

        return new SendMessageResultDto { MessageId = message.Id };
    }

    public async Task<IEnumerable<UserDto>> FindUsersAsync(int currentUserId, UserSearchDto search)
    {
        var currentRecipient = await GetRecipientByUserIdAsync(currentUserId);
        if (currentRecipient == null)
        {
            return Array.Empty<UserDto>();
        }

        if (string.IsNullOrWhiteSpace(search.Name))
        {
            return Array.Empty<UserDto>();
        }

        var recipients = await context.Recipients
            .AsNoTracking()
            .Include(r => r.User)
            .Where(r => r.Id != currentRecipient.Id && r.Name.Contains(search.Name))
            .ToListAsync();

        return recipients.Select(MapUserDto);
    }

    private Task<Recipient?> GetRecipientByUserIdAsync(int userId)
    {
        return context.Recipients
            .AsNoTracking()
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.User.Id == userId);
    }

    private static UserDto MapUserDto(Recipient recipient)
    {
        return new UserDto
        {
            Id = recipient.User.Id,
            Email = recipient.User.Email,
            RecipientId = recipient.Id,
            Recipient = MapRecipientDto(recipient)
        };
    }

    private static RecipientDto MapRecipientDto(Recipient recipient)
    {
        return new RecipientDto
        {
            Id = recipient.Id,
            Name = recipient.Name
        };
    }
}
