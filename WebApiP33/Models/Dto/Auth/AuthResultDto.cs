namespace WebApiP33.Models.Dto.Auth;

public class AuthResultDto
{
    public string? Token { get; set; } 
    public bool Success { get; set; }
    public string? Error { get; set; }
}
