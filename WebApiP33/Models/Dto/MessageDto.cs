namespace WebApiP33.Models.Dto;

public class MessageDto
{
    public int Id { get; set; }
    public UserDto From { get; set; }
    public UserDto To { get; set; }
    public string Text { get; set; }
    public DateTime Timestamp { get; set; }
}
