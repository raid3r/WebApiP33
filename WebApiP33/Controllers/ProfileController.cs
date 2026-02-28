using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        // TODO : реалізувати завантаження профілю користувача
        return new UserDto
        {
            Id = user.Id,
            Email = user.Email
        };
    }
}
