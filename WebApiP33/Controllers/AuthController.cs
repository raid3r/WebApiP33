using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApiP33.Models;
using WebApiP33.Models.DAL;
using WebApiP33.Models.Dto;
using WebApiP33.Models.Dto.Auth;

namespace WebApiP33.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController(
    UserManager<User> userManager, 
    IConfiguration configuration,
    ChatContext chatContext
    ) : ControllerBase
{
    // register
    [HttpPost("register")]
    public async Task<AuthResultDto> Register([FromBody] RegisterRequestDto request)
    {
        // Перевірка, чи користувач з таким email вже існує
        var existingUser = await userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            return new AuthResultDto
            {
                Success = false,
                Error = "User with this email already exists."
            };
        }

        var newUser = new User
        {
            UserName = request.Email,
            Email = request.Email,
        };
        var createResult = await userManager.CreateAsync(newUser, request.Password);

        if (!createResult.Succeeded)
        {
            return new AuthResultDto
            {
                Success = false,
                Error = string.Join("; ", createResult.Errors.Select(e => e.Description))
            };
        }

        chatContext.Users.Attach(newUser);
        chatContext.Recipients.Add(new Recipient
        {
            Name = request.Email,
            User = newUser
        });
        await chatContext.SaveChangesAsync();

        var token = GenerateJwtToken(newUser);
        return new AuthResultDto
        {
            Success = true,
            Token = token
        };
    }

    // login
    [HttpPost("login")]
    public async Task<AuthResultDto> Login([FromBody] LoginRequestDto request)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user == null || !await userManager.CheckPasswordAsync(user, request.Password))
        {
            return new AuthResultDto
            {
                Success = false,
                Error = "Invalid email or password."
            };
        }
        var token = GenerateJwtToken(user);
        return new AuthResultDto
        {
            Success = true,
            Token = token
        };
    }

    // delete account


    private string GenerateJwtToken(User user)
    {
        var key = Encoding.UTF8.GetBytes(configuration["JwtSettings:SecretKey"]!);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Sub, user.Email!),
            new Claim(JwtRegisteredClaimNames.Name, user.Email!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!)
        };

        var token = new JwtSecurityToken
        (
            issuer: null,
            audience: null,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(72),
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature
                )
        );

        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.WriteToken(token);
    }


}
