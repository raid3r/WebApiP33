using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebApiP33.Models;
using WebApiP33.Models.DAL;
using WebApiP33.Models.Dto;

namespace WebApiP33.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/profile")]
public class ProfileController(ChatContext context) : ControllerBase
{
    // get profile
    [HttpGet]
    public async Task<UserDto> GetProfile()
    {
        var currentUserId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        var user = await context.Users.FindAsync(int.Parse(currentUserId!));
        var recipient = await context.Recipients
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.User.Id == user.Id);
        // TODO : реалізувати завантаження профілю користувача
        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            RecipientId = recipient?.Id ?? 0,
            Recipient = recipient == null
                ? null
                : new RecipientDto
                {
                    Id = recipient.Id,
                    Name = recipient.Name
                }
        };
    }
}
